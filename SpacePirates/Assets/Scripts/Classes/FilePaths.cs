using UnityEngine;
using System.Collections;
using System.IO;

public static class FilePaths
{
	//File Paths
	private static string _s_FilePath = "";
	private static string _s_PartialFilePath = "/dir/";
	private static string _s_PartialFilePath_editor = "/StreamingAssets/dir/";
	//private string _s_PartialFilePath_editor = "/"; // for Application.persistentDataPath in EDITOR

	//Any call to this class should ensure that it initialized itself
	private static bool _isInit = false;
	
	// Used this for initialization
	public static void Init() 
	{
		if(!_isInit)
		{
			//Set file paths correctly
			#if UNITY_EDITOR
			_s_FilePath = Application.dataPath + _s_PartialFilePath_editor;
			#else
			_s_FilePath = Application.persistentDataPath + _s_PartialFilePath; //persistent path used on devices
			#endif

			//Make sure that we have a directory to save files in
			if(!(Directory.Exists(_s_FilePath)))
			{
				Directory.CreateDirectory(_s_FilePath);
			}

			_isInit = true;
		}
	}


	/// ///////////////////////////////////////////////
	// Get/Sets

	public static string GetResourcePath()
	{
		Init();//ensure class is initialized

		return _s_FilePath;
	}

	//If the requested folder does not exist, it is created
	//This method appends the res directory automatically
	public static void MakeFolder(string folderName)
	{
		Init();//ensure class is initialized

		if(folderName.Length > 0)
		{
			string tempDir = _s_FilePath + folderName;

			if(!(Directory.Exists(tempDir)))
			{
				Directory.CreateDirectory(tempDir);
			}
		}
	}

	/// ///////////////////////////////////////////////
	// File Functions

	//returns a comma separated string of all files in all directories
	public static string GetAllFileNames()
	{
		Init();//ensure class is initialized

		DirectoryInfo dirInfo = new DirectoryInfo(_s_FilePath);
		FileInfo[] fileInfo = dirInfo.GetFiles("*.*", SearchOption.AllDirectories);

		string retStr = "";

		bool _isValidFile = true;

		foreach (FileInfo file in fileInfo)
		{
			_isValidFile = true;

			if(file.Extension == ".meta" || file.Extension == ".DS_Store")
			{
				_isValidFile = false;
			}

				//  how to get just folder of file
				//_tempFolderName = file.Directory.ToString().RemoveDataPath_FromFolderName();
				
			if(_isValidFile)
			{
				retStr += file.FullName;
				retStr += ",";
			}
		}

		//strip final comma
		if(retStr.Length > 0)
		{
			retStr = retStr.Substring(0, retStr.Length - 1);
		}

		return retStr;
	}


	//Gets folders from the res path
	public static string GetFolderNames()
	{
		Init();//ensure class is initialized

		DirectoryInfo dirInfo = new DirectoryInfo(_s_FilePath);
		DirectoryInfo[] folderInfo = dirInfo.GetDirectories();

		string retStr = "";

		foreach (DirectoryInfo folder in folderInfo)
		{
			retStr += folder.FullName;
			retStr += ",";
		}

		//strip final comma
		if(retStr.Length > 0)
		{
			retStr = retStr.Substring(0, retStr.Length - 1);
		}
		
		return retStr;
	}

	/// ///////////////////////////////////////////////
	// File Name Get Functions
	
	public static string GetFileNames_fromFolder(string s_extension, string folderPath)
	{
		Init();//ensure class is initialized

		//If this is a valid folder
		if((Directory.Exists(_s_FilePath + folderPath)))
		{
			DirectoryInfo dirInfo = new DirectoryInfo(_s_FilePath + folderPath);

			FileInfo[] fileInfo = dirInfo.GetFiles("*" + s_extension, SearchOption.TopDirectoryOnly);
			
			string retStr = "";
			
			foreach (FileInfo file in fileInfo)
			{
				retStr += file.FullName;
				retStr += ",";
			}
			
			//strip final comma
			if(retStr.Length > 0)
			{
				retStr = retStr.Substring(0, retStr.Length - 1);
			}
			
			return retStr;
		}

		return ""; //the requested folder was no good
	}



	/// ///////////////////////////////////////////////
	// EXAMPLE
	/*
			//This is the most efficient way to check if one string contains another
			if(CHECKTHISSTRING.Length < FORTHISNAME.Length ? false : CHECKTHISSTRING.Contains(FORTHISNAME))
			{
				contains = true;
			}
	*/
}
