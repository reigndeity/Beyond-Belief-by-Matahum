using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("isCreditsDone")]
	public class ES3UserType_PortalInteractable : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_PortalInteractable() : base(typeof(PortalInteractable)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (PortalInteractable)obj;
			
			writer.WritePrivateField("isCreditsDone", instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (PortalInteractable)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "isCreditsDone":
					instance = (PortalInteractable)reader.SetPrivateField("isCreditsDone", reader.Read<System.Boolean>(), instance);
					break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_PortalInteractableArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_PortalInteractableArray() : base(typeof(PortalInteractable[]), ES3UserType_PortalInteractable.Instance)
		{
			Instance = this;
		}
	}
}