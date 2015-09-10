using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public static class MapLayoutSerializer
{
	//File Name
	private static string _s_filePrefix = "maps/";
	private static string _s_fileName = "maplayout_00";
	private static string _s_fileExtension = ".xml";

	public static void ChangeSaveName(string newname)
	{
		_s_fileName = newname;
	}

	// ====================================================================================================
	/// <summary>
	/// Serializes the MapLayout to store the data into an xml file.
	/// </summary>
	public static void SerializeMapLayout(C_MapLayout MapLayout)
	{        
		XmlSerializer serializer = new XmlSerializer( typeof( C_MapLayout ) );
		System.IO.StreamWriter streamWriter = System.IO.File.CreateText( FilePaths.GetResourcePath() + 
		                                                                _s_filePrefix + 
		                                                                _s_fileName + _s_fileExtension ) ;
		
		// object is type of C_MapLayout    
		serializer.Serialize( streamWriter, MapLayout );
		
		streamWriter.Close() ;
		streamWriter.Dispose() ;
	}

	// ====================================================================================================
	/// <summary>
	/// Deserializes the MapLayout if an xml file exists. 
	/// </summary>
	public static C_MapLayout DeserializeMapLayout()
	{    
		C_MapLayout MapLayout = new C_MapLayout();
		string mapLayoutFileName = FilePaths.GetResourcePath() + _s_filePrefix + _s_fileName + _s_fileExtension;

		System.IO.FileStream fileStream ;
		XmlReader reader ;
		XmlSerializer serializer = new XmlSerializer( typeof( C_MapLayout ) );
		
		// Check if file exists before opening a stream reader on it
		if( !System.IO.File.Exists( mapLayoutFileName ) )
		{
			//Map file did not exist, creating one

			fileStream = System.IO.File.Create( mapLayoutFileName ) ;
			fileStream.Close() ;
			fileStream.Dispose() ;
			SerializeMapLayout(MapLayout) ;
		}
		else
		{
			// Open the data file and read it so as to fill the MapLayout structure
			fileStream = new System.IO.FileStream( mapLayoutFileName, System.IO.FileMode.Open );
			reader = new XmlTextReader(fileStream) ;

			try
			{
				if( serializer.CanDeserialize( reader ) )
				{
					MapLayout = serializer.Deserialize( reader ) as C_MapLayout;
				}

			}
			finally
			{
				// Don't forget to close the readers and streams !
				reader.Close() ;
				fileStream.Close() ;
				fileStream.Dispose() ;
			}
		}

		return MapLayout;
	}
}