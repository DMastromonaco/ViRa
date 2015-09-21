using UnityEngine;
using System.Collections;

public static class BuildingDisplayName
{
	private static string off = "off";

	private static string bld_govMansion = "Governor Mansion";
	private static string bld_workerHouse = "Worker House";
	private static string bld_bar = "Bar";
	private static string bld_security = "Security Center";
	private static string bld_hotel = "Hotel";
	private static string bld_mine = "Mine";

	private static string remove = "remove";

	public static string getName(BuildingType whatBuilding)
	{
		switch(whatBuilding)
		{
		case BuildingType.off:
			return off;
			break;
			
		case BuildingType.bld_govMansion:
			return bld_govMansion;
			break;
			
		case BuildingType.bld_workerHouse:
			return bld_workerHouse;
			break;
			
		case BuildingType.bld_bar:
			return bld_bar;
			break;
			
		case BuildingType.bld_security:
			return bld_security;
			break;
			
		case BuildingType.bld_hotel:
			return bld_hotel;
			break;
			
		case BuildingType.bld_mine:
			return bld_mine;
			break;

		case BuildingType.remove:
			return remove;
			break;
			
		default:
			return "";
			break;
		}
	}
}

