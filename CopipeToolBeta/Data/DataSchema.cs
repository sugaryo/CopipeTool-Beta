using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CopipeToolBeta.Data
{
	public static class DataSchema
	{
		public static IEnumerable<CopipeData> Parse( string xml )
		{
			var serializer = new XmlSerializer( typeof(Copipe) );

			var copipe = serializer.Deserialize( new StringReader(xml) ) as Copipe;

			return copipe.Data.AsEnumerable();
		}


		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute( "code" )]
		[System.Xml.Serialization.XmlTypeAttribute( AnonymousType = true )]
		[System.Xml.Serialization.XmlRootAttribute( Namespace = "", IsNullable = false )]
		public partial class Copipe
		{

			/// <remarks/>
			[System.Xml.Serialization.XmlElementAttribute( "Data" )]
			public CopipeData[] Data{ get; set; }
		}



		/// <remarks/>
		[System.SerializableAttribute()]
		[System.ComponentModel.DesignerCategoryAttribute( "code" )]
		[System.Xml.Serialization.XmlTypeAttribute( AnonymousType = true )]
		public partial class CopipeData
		{
			/// <remarks/>
			[System.Xml.Serialization.XmlAttributeAttribute()]
			public string title { get; set; }

			/// <remarks/>
			[System.Xml.Serialization.XmlTextAttribute()]
			public string Value{ get; set; }
		}



	}
}
