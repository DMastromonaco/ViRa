using UnityEngine;
using System.Collections;

public static class GameLevelDataNameGen
{
	//hard-coded, game level data name prefix
	private static string _s_namePrefix = "leveldata_";

	public static string getName(string fileName)
	{
		string retName = _s_namePrefix;

		//TBD : parse out spaces and other characters

		retName += fileName;

		return retName;
	}
}
