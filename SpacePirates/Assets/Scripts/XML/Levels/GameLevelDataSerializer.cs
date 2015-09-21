using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

public static class GameLevelDataSerializer
{
	//File Name
	private static string _s_filePrefix = "leveldata/";
	private static string _s_fileName = "leveldata_00";
	private static string _s_fileExtension = ".xml";

	//Clean name generated from GameLevelDataNameGen.getName()
	public static void setFileName(string newname)
	{
		_s_fileName = newname;
	}

	// ====================================================================================================
	/// <summary>
	/// Serializes the LevelData to store the data into an xml file.
	/// </summary>
	public static void SerializeLevelData(C_GameLevelData LevelData)
	{        
		XmlSerializer serializer = new XmlSerializer( typeof( C_GameLevelData ) );
		System.IO.StreamWriter streamWriter = System.IO.File.CreateText( FilePaths.GetResourcePath() + 
		                                                                _s_filePrefix + 
		                                                                _s_fileName + _s_fileExtension ) ;
		
		// object is type of C_GameLevelData    
		serializer.Serialize( streamWriter, LevelData );
		
		streamWriter.Close() ;
		streamWriter.Dispose() ;
	}

	// ====================================================================================================
	/// <summary>
	/// Deserializes the LevelData if an xml file exists. 
	/// </summary>
	public static C_GameLevelData DeserializeLevelData()
	{    
		C_GameLevelData LevelData = new C_GameLevelData();
		string LevelDataFileName = FilePaths.GetResourcePath() + _s_filePrefix + _s_fileName + _s_fileExtension;

		System.IO.FileStream fileStream ;
		XmlReader reader ;
		XmlSerializer serializer = new XmlSerializer( typeof( C_GameLevelData ) );
		
		// Check if file exists before opening a stream reader on it
		if( !System.IO.File.Exists( LevelDataFileName ) )
		{
			//Map file did not exist, creating one

			fileStream = System.IO.File.Create( LevelDataFileName ) ;
			fileStream.Close() ;
			fileStream.Dispose() ;
			SerializeLevelData(LevelData) ;
		}
		else
		{
			// Open the data file and read it so as to fill the LevelData structure
			fileStream = new System.IO.FileStream( LevelDataFileName, System.IO.FileMode.Open );
			reader = new XmlTextReader(fileStream) ;

			try
			{
				if( serializer.CanDeserialize( reader ) )
				{
					LevelData = serializer.Deserialize( reader ) as C_GameLevelData;
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

		return LevelData;
	}
}