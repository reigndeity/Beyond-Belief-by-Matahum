using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("isOpened")]
	public class ES3UserType_Chest : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_Chest() : base(typeof(Chest)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (Chest)obj;
			
			writer.WriteProperty("isOpened", instance.isOpened, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (Chest)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "isOpened":
						instance.isOpened = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_ChestArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_ChestArray() : base(typeof(Chest[]), ES3UserType_Chest.Instance)
		{
			Instance = this;
		}
	}
}