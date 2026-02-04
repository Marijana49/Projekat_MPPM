using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;


namespace FTN.Services.NetworkModelService.DataModel.Wires
{
	public class SeriesCompensator : ConductingEquipment
	{
		private float r;

		private float r0;

		private float x;

		private float x0;

		public SeriesCompensator(long globalId)
			: base(globalId)
		{
		}

		public float R
		{
			get { return r;}
			set { r = value;}
		}

		public float R0
		{
			get { return r0; }
			set { r0 = value;}
		}

		public float X
		{
			get { return x; }
			set { x = value;}
		}

		public float X0
		{
			get { return x0; }
			set { x0 =  value;}
		}

		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				SeriesCompensator x = (SeriesCompensator)obj;
				return (x.r == this.r && x.r0 == this.r0 &&
						x.x == this.x && x.x0 == this.x0);
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
				case ModelCode.SC_R:
				case ModelCode.SC_R0:
				case ModelCode.SC_X:				
				case ModelCode.SC_X0:				
					return true;

				default:
					return base.HasProperty(t);
			}
		}

		public override void GetProperty(Property prop)
		{
			switch (prop.Id)
			{

				case ModelCode.SC_R:
					prop.SetValue(r);
					break;				

				case ModelCode.SC_R0:
					prop.SetValue(r0);
					break;

				case ModelCode.SC_X:
					prop.SetValue(x);
					break;
                case ModelCode.SC_X0:
                    prop.SetValue(x0);
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
				case ModelCode.SC_R:
					r = property.AsFloat();
					break;

				case ModelCode.SC_R0:					
					r0 = property.AsFloat();
					break;
				case ModelCode.SC_X:
					x = property.AsFloat();
					break;
				case ModelCode.SC_X0:
					x0 = property.AsFloat();
					break;
				default:
					base.SetProperty(property);
					break;
			}
		}

		#endregion IAccess implementation
	}
}
