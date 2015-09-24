using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

public static class XMLSerialization
{
	// ====================================================================================================
	/// <summary>
	/// Serializes the XML class of type T to store the data into an xml file.
	/// </summary>
	public static void Serialize<T>(T C_Data, string fileName)
	{        
		XmlSerializer serializer = new XmlSerializer( typeof( T ) );
		System.IO.StreamWriter streamWriter = System.IO.File.CreateText( FilePaths.GetResourcePath() + fileName ) ;
		
		// C_Data is type of T    
		serializer.Serialize( streamWriter, C_Data );
		
		streamWriter.Close() ;
		streamWriter.Dispose() ;
	}

	// ====================================================================================================
	/// <summary>
	/// Deserializes the XML class of type T if an xml file exists. 
	/// Or creates a new() copy of the class, saves and load that if file does not exist
	/// </summary>
	public static T Deserialize<T>(string fileName) where T : class, new()
	{    
		T C_Data = new T();

		System.IO.FileStream fileStream ;
		XmlReader reader ;
		XmlSerializer serializer = new XmlSerializer( typeof( T ) );
		
		// Check if file exists before opening a stream reader on it
		if( !System.IO.File.Exists( FilePaths.GetResourcePath() + fileName ) )
		{
			fileStream = System.IO.File.Create( FilePaths.GetResourcePath() + fileName ) ;
			fileStream.Close() ;
			fileStream.Dispose() ;
			Serialize<T>(C_Data, fileName) ;
		}
		else
		{
			// Open the data file and read it so as to fill the C_Data structure
			fileStream = new System.IO.FileStream( FilePaths.GetResourcePath() + fileName, System.IO.FileMode.Open );
			reader = new XmlTextReader(fileStream) ;
			
			try
			{
				if( serializer.CanDeserialize( reader ) )
					C_Data = serializer.Deserialize( reader ) as T;
			}
			finally
			{
				// Don't forget to close the readers and streams !
				reader.Close() ;
				fileStream.Close() ;
				fileStream.Dispose() ;
			}
		}

		return C_Data;
	}
}