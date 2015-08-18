using UnityEngine;
using System.Collections;

//Declare a variable of this type to use as a state machine for the enum BuildingType
[System.Serializable]
public class BuildingPlacement
{
	public bool isOn = false;
	
	public BuildingType currentBuilding = BuildingType.off;

	public BuildingPlacement()
	{
		isOn = false;
		currentBuilding = BuildingType.off;
	}
}
