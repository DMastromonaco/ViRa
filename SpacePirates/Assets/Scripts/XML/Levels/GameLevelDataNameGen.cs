using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public static class GameLevelDataNameGen
{
	//hard-coded, game level data name prefix
	private static string _s_namePrefix = "leveldata_";

	public static string getName(string fileName)
	{
		string retName = _s_namePrefix;

		//Remove non-alphanumeric characters, and spaces
		fileName = Regex.Replace(fileName, "[^A-Za-z0-9]+", "");
		fileName.Replace(" ", "");

		retName += fileName;

		return retName;
	}

	public static string getName(int fileName)
	{
		string retName = _s_namePrefix;
		
		//do not allow negative
		if(fileName < 0)
		{
			//default
			fileName = 0;
		}
		
		if(fileName < 100)
		{
			//force 2 digits
			retName += fileName.ToString("00");
		}
		else
		{
			//100 or larger is just pure number, currently unused
			retName += fileName.ToString();
		}
		
		return retName;
	}
}
