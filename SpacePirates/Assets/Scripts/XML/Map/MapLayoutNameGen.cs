using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;

public static class MapLayoutNameGen
{
	//hard-coded, map layout file name prefix
	private static string _s_namePrefix = "maplayout_";

	public static string getName(int whatMap)
	{
		string retName = _s_namePrefix;

		//do not allow negative
		if(whatMap < 0)
		{
			whatMap = 0;
		}

		if(whatMap < 100)
		{
			//force 2 digits
			retName += whatMap.ToString("00");
		}
		else
		{
			//100 or larger is just pure number, currently unused
			retName += whatMap.ToString();
		}

		return retName;
	}

	public static string getName(string whatMap)
	{
		string retName = _s_namePrefix;

		//Remove non-alphanumeric characters, and spaces
		whatMap = Regex.Replace(whatMap, "[^A-Za-z0-9]+", "");
		whatMap.Replace(" ", "");

		retName += whatMap.ToString();
				
		return retName;
	}
}
