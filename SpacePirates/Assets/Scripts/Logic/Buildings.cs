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

	//== PRIVATE vars
	private iBuildingReceiver _currentTile = null;	

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

	public void PlaceBuildingOnCurrent()
	{
		if(buildingPlacement.currentBuilding != BuildingType.off)
		{
			if(_currentTile != null)
			{
				//Handle building removal mode
				if(buildingPlacement.currentBuilding == BuildingType.remove)
				{
					//Store the building and it's removal sound from this tile in temp
					iBuildingPlacer _tempBuilding = _currentTile.getPlacedBuilding();
					SoundType _tempSound = SoundType.None;
					if(_tempBuilding != null)
					{
						_tempSound = _tempBuilding.getRemovalSound();
					}

					//Try to remove building from the current tile building receiver
					if(_currentTile.RemoveBuilding())
					{
						//Building was cleared from receiver

						//Play building remove sound, if we have one
						if(SoundType.None != _tempSound)
						{
							Sounds.instance.Play(_tempSound);
						}
					}
					else
					{
						//no building to clear
					}
				}
				else
				{
					//Handle placement of all other buildings
					GameObject tempBuilding = GameObject.Instantiate(GetPrefab_forType(buildingPlacement.currentBuilding));

					//Try to place building on the current tile
					if(tempBuilding.gameObject.GetComponent<iBuildingPlacer>().PlaceBuilding(_currentTile))
					{
						//building was successfully placed onto the receiver

						//Parent new tile to root
						tempBuilding.transform.parent = GO_Root.transform;

						//add to internal list
						allBuildings.Add(tempBuilding);

						//Initiate the new building (with it's index, etc)
						tempBuilding.gameObject.GetComponent<iBuildingPlacer>().Init(allBuildings.Count - 1);

						//Play building place sound
						Sounds.instance.Play(
							tempBuilding.gameObject.GetComponent<iBuildingPlacer>().getPlacementSound());
					}
					else
					{
						//fail to build, destroy temp
						Destroy(tempBuilding);
					}
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

	public void SetCurrentHoverTileForBuilding(iBuildingReceiver tile)
	{
		//Store this tile as the current tile (has a click down but no click up yet)
		_currentTile = tile;
	}

	public void PlaceTempBuilding(iBuildingReceiver tile)
	{
		if(tempBuilding == null)
		{
			//There is no temp building for remove mode
			if(buildingPlacement.currentBuilding != BuildingType.remove)
			{
				tempBuilding = Instantiate(GetPrefab_forType(buildingPlacement.currentBuilding));
			}
		}

		if(tempBuilding != null)
		{
			tempBuilding.GetComponent<iBuildingPlacer>().ChangeLoc(tile.GetPlacementLocation());
		}

		//Store this tile as the current tile (has a click down but no click up yet)
		_currentTile = tile;
	}

	public void DestroyAndRemoveAllBuildings()
	{
		for(int x = 0; x < allBuildings.Count; x++)
		{
			if(allBuildings[x] != null)
			{
				allBuildings[x].gameObject.GetComponent<iBuildingPlacer>().DestroyBuilding();
			}
		}

		allBuildings.Clear();
	}

	public void RemoveBuildingFromList(int index)
	{
		if(allBuildings.Count > index)
		{
			allBuildings[index] = null;
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
