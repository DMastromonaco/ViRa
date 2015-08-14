using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Buildings : Singleton<Buildings>
{
	//STATE
	public BuildingPlacement buildingPlacement = new BuildingPlacement();

	//Prefabs
	public List<GameObject> prefabs_Buildings = new List<GameObject>();

	//== PUBLIC inspector vars
	public GameObject GO_Root;

	//TEMP GOs

	public GameObject tempBuilding = null;

	//SPAWNED BUILDINGS

	public List<GameObject> allBuildings = new List<GameObject>();

	// BUILDING PLACMENT

	public void SetPlacementBuilding(int whatBuilding)
	{
		//Only set as valid enum values
		if(Enum.IsDefined(typeof(BuildingType), whatBuilding))
		{
			buildingPlacement.currentBuilding = (BuildingType)whatBuilding;
		}
		else
		{
			//otherwise default to off
			buildingPlacement.currentBuilding = BuildingType.off;
		}
	}

	public void StartPlacement()
	{
		buildingPlacement.isOn = true;
	}

	private iBuildingReceiver _currentTile = null;

	public void PlaceBuildingOnCurrent()
	{
		if(buildingPlacement.currentBuilding != BuildingType.off)
		{
			if(_currentTile != null)
			{
				GameObject tempBuilding = GameObject.Instantiate(GetPrefab_forType(buildingPlacement.currentBuilding));

				if(tempBuilding.gameObject.GetComponent<iBuildingPlacer>().PlaceBuilding(_currentTile))
				{
					//building was successfully placed onto the receiver

					//Parent new tile to root
					tempBuilding.transform.parent = GO_Root.transform;

					//add to internal list
					allBuildings.Add(tempBuilding);
				}
				else
				{
					//fail to build, destroy temp
					Destroy(tempBuilding);
				}
			}
		}
	}

	public void StopPlacement()
	{
		buildingPlacement.isOn = false;
	}

	public void ClearTempBuilding()
	{
		Destroy(tempBuilding);
	}

	public void PlaceTempBuilding(iBuildingReceiver tile)
	{
		if(tempBuilding == null)
		{
			tempBuilding = Instantiate(GetPrefab_forType(buildingPlacement.currentBuilding));
		}

		if(tempBuilding != null)
		{
			tempBuilding.GetComponent<iBuildingPlacer>().ChangeLoc(tile.GetPlacementLocation());
			_currentTile = tile;
		}
	}

	///////////////////////////////////

	public GameObject GetPrefab_forType(BuildingType whatBuilding)
	{
		switch(whatBuilding)
		{
		case BuildingType.off:
			return null;
			break;

		case BuildingType.house_00:
			return prefabs_Buildings[0];
			break;

		case BuildingType.house_01:
			return prefabs_Buildings[1];
			break;

		case BuildingType.house_02:
			return prefabs_Buildings[2];
			break;

		case BuildingType.house_03:
			return prefabs_Buildings[3];
			break;
			
		default:
			return null;
			break;
		}
	}
}
