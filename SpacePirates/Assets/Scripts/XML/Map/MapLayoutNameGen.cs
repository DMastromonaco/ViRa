using UnityEngine;
using System.Collections;

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
}
