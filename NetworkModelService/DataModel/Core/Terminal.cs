using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Wires;

namespace FTN.Services.NetworkModelService.DataModel.Core
{	
	public class Terminal : IdentifiedObject
	{
	
		private long connectivityNode = 0;				
		private long conductingEquipment = 0;				

		public Terminal(long globalId) : base(globalId) 
		{
		}
		
		public long ConnectivityNode
		{
			get
			{
				return connectivityNode;
			}

			set
			{
                connectivityNode = value;
			}
		}

        public long ConductingEquipment
        {
            get
            {
                return conductingEquipment;
            }

            set
            {
                conductingEquipment = value;
            }
        }

        public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				Terminal x = (Terminal)obj;
				return (x.conductingEquipment == this.conductingEquipment) && (x.connectivityNode == this.connectivityNode);
			}
			else
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		#region IAccess implementation
		public override bool HasProperty(ModelCode t)
		{
			switch (t)
			{
				case ModelCode.TERMINAL_CN:
				case ModelCode.TERMINAL_CE:
					return true;

				default:
					return base.HasProperty(t);
			}
		}

		public override void GetProperty(Property prop)
		{
			switch (prop.Id)
			{
				case ModelCode.TERMINAL_CN:
					prop.SetValue(connectivityNode);
					break;

				case ModelCode.TERMINAL_CE:
					prop.SetValue(conductingEquipment);
					break;

				default:
					base.GetProperty(prop);
					break;
			}
		}

		public override void SetProperty(Property property)
		{
			switch (property.Id)
			{
				case ModelCode.TERMINAL_CN:
					connectivityNode = property.AsReference();
					break;
				case ModelCode.TERMINAL_CE:
					conductingEquipment = property.AsReference();
					break;
				default:
					base.SetProperty(property);
					break;
			}
		}
		
		#endregion IAccess implementation

		#region IReference implementation

		public override bool IsReferenced
		{
			get
			{
				return base.IsReferenced;
			}
		}
	
		public override void GetReferences(Dictionary<ModelCode, List<long>> references, TypeOfReference refType)
		{
            if(connectivityNode != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_CN] = new List<long>();
                references[ModelCode.TERMINAL_CN].Add(connectivityNode);
            }

            if (conductingEquipment != 0 && (refType == TypeOfReference.Reference || refType == TypeOfReference.Both))
            {
                references[ModelCode.TERMINAL_CE] = new List<long>();
                references[ModelCode.TERMINAL_CE].Add(conductingEquipment);
            }

			base.GetReferences(references, refType);

        }
	
		#endregion IReference implementation	
	}
}