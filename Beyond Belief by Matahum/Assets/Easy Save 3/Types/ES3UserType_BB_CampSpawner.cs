using System;
using UnityEngine;

namespace ES3Types
{
	[UnityEngine.Scripting.Preserve]
	[ES3PropertiesAttribute()]
	public class ES3UserType_BB_CampSpawner : ES3ComponentType
	{
		public static ES3Type Instance = null;

		public ES3UserType_BB_CampSpawner() : base(typeof(BB_CampSpawner)){ Instance = this; priority = 1;}


		protected override void WriteComponent(object obj, ES3Writer writer)
		{
			var instance = (BB_CampSpawner)obj;
			
		}

		protected override void ReadComponent<T>(ES3Reader reader, object obj)
		{
			var instance = (BB_CampSpawner)obj;
			foreach(string propertyName in reader.Properties)
			{
				switch(propertyName)
				{
					
					default:
						reader.Skip();
						break;
				}
			}
		}
	}


	public class ES3UserType_BB_CampSpawnerArray : ES3ArrayType
	{
		public static ES3Type Instance;

		public ES3UserType_BB_CampSpawnerArray() : base(typeof(BB_CampSpawner[]), ES3UserType_BB_CampSpawner.Instance)
		{
			Instance = this;
		}
	}
}