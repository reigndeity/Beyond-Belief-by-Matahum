using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute("isInteracted")]
	public class ES3UserType_BananaTreeEasterEgg : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_BananaTreeEasterEgg() : base(typeof(BananaTreeEasterEgg)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (BananaTreeEasterEgg)obj;
			
			writer.WriteProperty("isInteracted", instance.isInteracted, ES3Type_bool.Instance);
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (BananaTreeEasterEgg)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					case "isInteracted":
						instance.isInteracted = reader.Read<System.Boolean>(ES3Type_bool.Instance);
						break;
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_BananaTreeEasterEggArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_BananaTreeEasterEggArray() : base(typeof(BananaTreeEasterEgg[]), ES3UserType_BananaTreeEasterEgg.Instance)
		{
			Instance = this;
		}
	}
}