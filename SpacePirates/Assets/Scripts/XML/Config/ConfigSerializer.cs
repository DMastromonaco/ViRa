using System.Xml;
using System.Xml.Serialization;

public static class ConfigSerializer
{
	//File Name
	private static string _s_configFileName = "config.xml";

	// ====================================================================================================
	/// <summary>
	/// Serializes the Config to store the data into an xml file.
	/// </summary>
	public static void SerializeConfig(C_ConfigSettings ConfigData)
	{        
		XmlSerializer serializer = new XmlSerializer( typeof( C_ConfigSettings ) );
		System.IO.StreamWriter streamWriter = System.IO.File.CreateText( FilePaths.GetResourcePath() + _s_configFileName ) ;
		
		// scultureData is type of C_ConfigSettings    
		serializer.Serialize( streamWriter, ConfigData );
		
		streamWriter.Close() ;
		streamWriter.Dispose() ;
	}

	// ====================================================================================================
	/// <summary>
	/// Deserializes the Config if an xml file exists. 
	/// </summary>
	public static C_ConfigSettings DeserializeConfig()
	{    
		C_ConfigSettings ConfigData = new C_ConfigSettings();

		System.IO.FileStream fileStream ;
		XmlReader reader ;
		XmlSerializer serializer = new XmlSerializer( typeof( C_ConfigSettings ) );
		
		// Check if file exists before opening a stream reader on it
		if( !System.IO.File.Exists( FilePaths.GetResourcePath() + _s_configFileName ) )
		{
			fileStream = System.IO.File.Create( FilePaths.GetResourcePath() + _s_configFileName ) ;
			fileStream.Close() ;
			fileStream.Dispose() ;
			SerializeConfig(ConfigData) ;
		}
		else
		{
			// Open the data file and read it so as to fill the ConfigData structure
			fileStream = new System.IO.FileStream( FilePaths.GetResourcePath() + _s_configFileName, System.IO.FileMode.Open );
			reader = new XmlTextReader(fileStream) ;
			
			try
			{
				if( serializer.CanDeserialize( reader ) )
					ConfigData = serializer.Deserialize( reader ) as C_ConfigSettings;
			}
			finally
			{
				// Don't forget to close the readers and streams !
				reader.Close() ;
				fileStream.Close() ;
				fileStream.Dispose() ;
			}
		}

		return ConfigData;
	}
}