using UnityEngine;
using System.Collections;

public static class BuildingCosts
{
	//Return hard coded costs of buildings
	//TBD : Load buildings and costs from XML?
	public static int GetCost(BuildingType whatBuilding)
	{
		switch(whatBuilding)
		{
		case BuildingType.off:
			return 0;
			break;
			
		case BuildingType.bld_govMansion:
			return 0;
			break;
			
		case BuildingType.bld_workerHouse:
			return 5;
			break;
			
		case BuildingType.bld_bar:
			return 10;
			break;
			
		case BuildingType.bld_security:
			return 15;
			break;
			
		case BuildingType.bld_hotel:
			return 10;
			break;

		case BuildingType.bld_mine:
			return 5;
			break;
			
		default:
			return 0;
			break;
		}
	}
}
