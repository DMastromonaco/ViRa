using UnityEngine;
using System.Collections;

public static class StringExtensions 
{
	private static int _i_pathLength = 0;
	private static bool _isInit = false;
	
	// Use this for initialization
	private static void InitPathLength() 
	{
		_i_pathLength = FilePaths.GetResourcePath().Length;

		_isInit = true;
	}

	public static string RemoveDataAndFolderPath(this string s_name, string folderPath)
	{
		s_name = s_name.RemoveDataPath();

		s_name = s_name.RemoveFolderPath(folderPath);

		return s_name;
	}
		
	public static string RemoveDataPath(this string s_name)
	{
		if(!_isInit){InitPathLength();}

		//If there is no folder, return empty string
		if(s_name.Length < _i_pathLength)
		{
			return "";
		}

		return s_name.Substring(_i_pathLength);
	}

	public static string RemoveFolderPath(this string s_name, string folderPath)
	{
		if(s_name.Length < folderPath.Length ? false : s_name.Contains(folderPath))
		{
			//the requested name DOES contain the folder path

			//so remove it's length from the begining of the name

			int tempLen = folderPath.Length;

			return s_name.Substring(tempLen);
		}
		else
		{
			//the path isn't path of this name, just return it
			return s_name;
		}
	}
}
