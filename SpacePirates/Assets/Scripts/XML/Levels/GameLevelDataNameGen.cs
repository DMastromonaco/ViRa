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
}
