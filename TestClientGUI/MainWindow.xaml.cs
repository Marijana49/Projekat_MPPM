using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TelventDMS.Services.NetworkModelService.TestClient.Tests;

namespace TestClientGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static TestGDA testGDA;
        public static ModelResourcesDesc resourcesDesc;
        public static Dictionary<ModelCode, List<ModelCode>> propertyIDsByModelCode;
        public static List<long> gids;
        public static ModelCode modelCode;
        public MainWindow()
        {
            InitializeComponent();
            testGDA = new TestGDA();
            resourcesDesc = new ModelResourcesDesc();
            propertyIDsByModelCode = Enum.GetValues(typeof(ModelCode)).Cast<ModelCode>().ToDictionary(mc => mc, mc => resourcesDesc.GetAllPropertyIds(mc));
            gids = testGDA.TestGetExtentValuesAllTypes();
            InputComboBox.ItemsSource = gids.Select(gid => $"0x{gid:x16}").ToList();
        }

        private void getValues_Click(object sender, RoutedEventArgs e)
        {
            MainWindow window = new MainWindow();
            this.Close();
            window.Show();
        }

        private void getExtentValues_Click(object sender, RoutedEventArgs e)
        {
            GetExtentValues getExtentValuesWindow = new GetExtentValues();
            this.Close();
            getExtentValuesWindow.Show();
        }

        private void getRelatedValues_Click(object sender, RoutedEventArgs e)
        {
            GetRelatedValues getRelatedValuesWindow = new GetRelatedValues();
            this.Close();
            getRelatedValuesWindow.Show();
        }

        private List<ModelCode> GetPropertiesForCheckedBoxes()
        {
            if (SelectAllCheckBox.IsChecked == true)
                return propertyIDsByModelCode[modelCode];

            var selectedProps = new List<ModelCode>();

            foreach (var child in CheckBoxGrid.Children)
            {
                if (child is CheckBox cb && cb.IsChecked == true)
                {
                    if (Enum.TryParse(cb.Content.ToString(), out ModelCode prop))
                    {
                        selectedProps.Add(prop);
                    }
                }
            }

            return selectedProps;
        }

        private void WriteResultsToTextBox(ResourceDescription rd)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("ResourceDescription:");
            sb.AppendLine($"Gid = 0x{rd.Id:x16}");
            sb.AppendLine($"Type = {(DMSType)ModelCodeHelper.ExtractTypeFromGlobalId(rd.Id)}");
            sb.AppendLine("Properties:");

            foreach (Property p in rd.Properties)
            {
                sb.Append($"\t{p.Id} = ");

                switch (p.Type)
                {
                    case PropertyType.Float:
                        sb.Append(p.AsFloat());
                        break;
                    case PropertyType.Bool:
                        sb.Append(p.PropertyValue.FloatValue == 1 ? "True" : "False");
                        break;
                    case PropertyType.Byte:
                    case PropertyType.Int32:
                    case PropertyType.Int64:
                    case PropertyType.TimeSpan:
                    case PropertyType.DateTime:
                        sb.Append(p.Id == ModelCode.IDOBJ_GID ? $"0x{p.AsLong():x16}" : p.AsLong().ToString());
                        break;
                    case PropertyType.Reference:
                        sb.Append($"0x{p.AsReference():x16}");
                        break;
                    case PropertyType.String:
                        sb.Append(p.AsString() ?? string.Empty);
                        break;
                    case PropertyType.Int64Vector:
                    case PropertyType.ReferenceVector:
                        var longs = p.AsLongs();
                        sb.Append(longs.Any()
                            ? string.Join(", ", longs.Select(val => $"0x{val:x16}"))
                            : "empty long/reference vector");
                        break;
                    case PropertyType.Enum:
                        try
                        {
                            sb.Append(new EnumDescs().GetStringFromEnum(p.Id, p.AsEnum()));
                        }
                        catch
                        {
                            sb.Append(p.AsEnum());
                        }
                        break;
                    default:
                        throw new Exception("Invalid property type.");
                }

                sb.AppendLine();
            }

            ResultsTextBox.Text = sb.ToString();
        }

        private void SelectAllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            bool isChecked = SelectAllCheckBox.IsChecked == true;

            foreach (var child in CheckBoxGrid.Children)
            {
                if (child is CheckBox cb)
                    cb.IsChecked = isChecked;
            }
        }

        private long GetGID() => gids[InputComboBox.SelectedIndex];
        private void CreateCheckBoxes()
        {
            CheckBoxGrid.Children.Clear();

            var props = propertyIDsByModelCode[modelCode];
            getValuesButton.Visibility = Visibility.Visible;
            SelectAllCheckBox.Visibility = Visibility.Visible;
            SelectAllCheckBox.IsChecked = false;

            foreach (var prop in props)
            {
                var cb = new CheckBox
                {
                    Content = prop.ToString(),
                    FontSize = 14,
                    FontFamily = new FontFamily("Trebuchet MS"),
                    Margin = new Thickness(0, 2, 0, 2),
                    Visibility = Visibility.Visible
                };

                CheckBoxGrid.Children.Add(cb);
            }
        }

        private void getValuesButton_Click(object sender, RoutedEventArgs e)
        {
            List<ModelCode> properties = GetPropertiesForCheckedBoxes();

            ResourceDescription rd = testGDA.GetValues(GetGID(), properties);
            WriteResultsToTextBox(rd);
        }

        private void InputComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (InputComboBox.SelectedIndex >= 0)
            {
                modelCode = resourcesDesc.GetModelCodeFromId(GetGID());
                CreateCheckBoxes();
            }
        }
    }
}
