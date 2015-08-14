using UnityEngine;
using System.Collections;

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
