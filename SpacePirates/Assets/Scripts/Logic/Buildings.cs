using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Prime31.MessageKit;

public class Buildings : Singleton<Buildings>
{
	//STATE - Dev Buildings
	public BuildingPlacement buildingPlacement = new BuildingPlacement();

	//STATE - In Game Building placement
	public BuildingPlacement buildInGame = new BuildingPlacement();

	//== PUBLIC inspector vars
	//Prefabs - regular buildings
	public List<GameObject> prefabs_Buildings = new List<GameObject>();

	//Prefabs - 'ghost' placement transparent buildings
	public List<GameObject> prefabs_transBuildings = new List<GameObject>();

	//== PUBLIC inspector vars
	public GameObject GO_Root;

	//The right click mask for In-Game building mode
	public GameObject GO_rightClickQuad;

	//The color of the selected building button
	public Color color_ActiveBuildingButton;

	public Vector3 v3_OffScreenLocation;

	//== PRIVATE vars
	private iBuildingReceiver _currentTile = null;
	private iBuildingReceiver _currentTile_buildMode = null;

	//To track if the shift key is held down or not
	private keyTracker kt_Shift = new keyTracker("Shift");

	//TEMP GOs
	public GameObject tempBuilding = null;
	public GameObject transparentBuilding = null;
	
	//SPAWNED BUILDINGS
	public List<GameObject> allBuildings = new List<GameObject>();



	//All building buttons, so their color can be controlled
	public List<BuildingButton> allBuildingButtons = new List<BuildingButton>();

	///////////////////////////////////

	void Start ()
	{			
		//===== Add Key input handlers
		MessageKit<keyTracker>.addObserver((int)InputMsg.key_shift, ShiftKey_keyPress);

		//Building right click mask starts disabled
		GO_rightClickQuad.SetActive(false);
	}

	///////////////////////////////////

	// BUILDING PLACMENT - Dev

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
					GameObject tempBuilding = GameObject.Instantiate(GetBuildingPrefab_forType(buildingPlacement.currentBuilding));

					//Try to place building on the current tile
					if(tempBuilding.gameObject.GetComponent<iBuildingPlacer>().PlaceBuilding(_currentTile))
					{
						//building was successfully placed onto the receiver

						//Parent new building to root
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
				tempBuilding = Instantiate(GetBuildingPrefab_forType(buildingPlacement.currentBuilding));
			}
		}

		if(tempBuilding != null)
		{
			tempBuilding.GetComponent<iBuildingPlacer>().ChangeLoc(tile.getPlacementLocation());
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

	public void AddBuildingToList(GameObject GOBuilding)
	{
		allBuildings.Add(GOBuilding);
	}

	///////////////////////////////////
	
	//===== BUILDING PLACMENT - In Game

	public void TryBeginBuildingPurchase(BuildingType buildType, BuildingButton buildingbutton)
	{
		//Only set for valid buildable building types
		if(buildType != BuildingType.off && buildType != BuildingType.remove)
		{
			int buildCost = BuildingCosts.GetCost(buildType);

			//Assume off state until purchase cost is validated
			//This will clear any previous button highlights or transparent buildings from previous build mode
			StopBuildingPurchase();

			//Check if player can afford to begin purchase of this building
			if(Resources.instance.CanAfford(buildCost))
			{
				//CAN afford building

				StartBuildingPurchase(buildType, buildingbutton);
			}
			else
			{
				//CANNOT afford building

				//TBD : Play cannot afford sound
				Debug.LogError("nope");
			}
		}
	}

	//After a valid building type and cost have been validated, set the in-game build state
	private void StartBuildingPurchase(BuildingType buildType, BuildingButton buildingbutton)
	{
		//Flag on
		buildInGame.isOn = true;

		//Set the color of the building button, since it was valid
		buildingbutton.SetButtonColor(color_ActiveBuildingButton);
		
		//Set the current building which the player is attempting to place and purchase
		buildInGame.currentBuilding = buildType;

		//Turn on the right click quad, so that cancel building will be processed
		GO_rightClickQuad.SetActive(true);
		
		//Spawn the transparent building on the current tile, if valid
		SpawnTransparentBuilding_onCurrentTile();
	}

	public void StopBuildingPurchase()
	{
		//Flag off
		buildInGame.isOn = false;
		buildInGame.keepOn = false;
		buildInGame.autoPlaceBuilding = false;

		//Clear building type
		buildInGame.currentBuilding = BuildingType.off;

		//Turn off the right click quad for cancels
		GO_rightClickQuad.SetActive(false);

		//Destroy temp ghost building, if it is spawned
		ClearTransparentBuilding();

		//Make sure none of the building buttons are highlighted
		ResetAllBuildingButtonColors();
	}

	public void PurchaseAndPlaceBuilding(iBuildingReceiver tile)
	{
		if(buildInGame.currentBuilding != BuildingType.off)
		{
			//Check cost and make sure the player can afford
			int buildCost = BuildingCosts.GetCost(buildInGame.currentBuilding);
			if(Resources.instance.CanAfford(buildCost))
			{
				//Handle purchase and placement of buildings
				GameObject tempBuilding = GameObject.Instantiate(GetBuildingPrefab_forType(buildInGame.currentBuilding));
				
				//Try to place building on the current tile
				if(tempBuilding.gameObject.GetComponent<iBuildingPlacer>().PlaceBuilding(tile))
				{
					//building was successfully placed onto the receiver

					//Subtract the cost of the building
					Resources.instance.subMoney(buildCost);

					//Parent new building to root
					tempBuilding.transform.parent = GO_Root.transform;
					
					//add to internal list
					allBuildings.Add(tempBuilding);
					
					//Initiate the new building (with it's index, etc)
					tempBuilding.gameObject.GetComponent<iBuildingPlacer>().Init(allBuildings.Count - 1);
					
					//Play building place sound
					Sounds.instance.Play(
						tempBuilding.gameObject.GetComponent<iBuildingPlacer>().getPlacementSound());

					//Destroy the transparent building temp game object
					ClearTransparentBuilding();
				}
				else
				{
					//fail to build, destroy temp
					Destroy(tempBuilding);
				}
			}

			//See if we need to drop out of build mode, if they can no longer afford the placements
			//Check cost and make sure the player can afford
			if(!Resources.instance.CanAfford(buildCost))
			{
				StopBuildingPurchase();
			}
		}
	}

	//Called on hover start of tiles
	public void SetCurrentTile_buildMode(iBuildingReceiver tile)
	{
		_currentTile_buildMode = tile;
	}
	
	//Called on hover stop of tiles
	public void ClearCurrentTile_buildMode(iBuildingReceiver tile)
	{
		//If we have stopped hovering the tile that was ref'ed as the current build mode tile, set to null
		if(_currentTile_buildMode == tile)
		{
			_currentTile_buildMode = null;
		}
	}
	
	//===== BUTTONs for in-game building

	//All building buttons will call this on awake so they can register with the list
	public void RegisterBuildingButton(BuildingButton buildingbutton)
	{
		allBuildingButtons.Add(buildingbutton);
	}

	//Colors

	public void ResetAllBuildingButtonColors()
	{
		for(int x = 0; x < allBuildingButtons.Count; x++)
		{
			allBuildingButtons[x].ResetButtonColor();
		}
	}

	//===== Transparent building handling

	public void PlaceTransparentBuilding(iBuildingReceiver tile)
	{
		//Spawn transparent building if it's not already
		if(transparentBuilding == null)
		{
			//There is no temp building for remove mode
			if(buildInGame.currentBuilding != BuildingType.remove)
			{
				transparentBuilding = Instantiate(GetTransparentBuildingPrefab_forType(buildInGame.currentBuilding));
			}
		}

		//Place the transparent building on the requesting receiver (board tile)
		if(transparentBuilding != null)
		{
			transparentBuilding.GetComponent<iBuildingPlacer>().ChangeLoc(tile.getPlacementLocation());
		}
	}

	public void MoveTransparentBuildingOffScreen()
	{
		if(transparentBuilding != null)
		{
			transparentBuilding.GetComponent<iBuildingPlacer>().ChangeLoc(v3_OffScreenLocation);
		}
	}

	public void SpawnTransparentBuilding_onCurrentTile()
	{
		//If there is a currently hovered tile, and build mode has been activated
		//Spawn a transparent building on it
		if(_currentTile_buildMode != null)
		{
			//Only place if the tile can receive a building (doesn't have one already)
			if(_currentTile_buildMode.canReceiveBuilding())
			{
				PlaceTransparentBuilding(_currentTile_buildMode);
			}
		}
	}

	public void ClearTransparentBuilding()
	{
		DestroyImmediate(transparentBuilding);
	}


	///////////////////////////////////

	/// Prefab references by type

	public GameObject GetBuildingPrefab_forType(BuildingType whatBuilding)
	{
		switch(whatBuilding)
		{
		case BuildingType.off:
			return null;
			break;

		case BuildingType.bld_govMansion:
			return prefabs_Buildings[0];
			break;

		case BuildingType.bld_workerHouse:
			return prefabs_Buildings[1];
			break;

		case BuildingType.bld_bar:
			return prefabs_Buildings[2];
			break;

		case BuildingType.bld_security:
			return prefabs_Buildings[3];
			break;

		case BuildingType.bld_hotel:
			return prefabs_Buildings[4];
			break;

		case BuildingType.bld_mine:
			return prefabs_Buildings[5];
			break;

		default:
			return null;
			break;
		}
	}

	public GameObject GetTransparentBuildingPrefab_forType(BuildingType whatBuilding)
	{
		switch(whatBuilding)
		{
		case BuildingType.off:
			return null;
			break;
			
		case BuildingType.bld_govMansion:
			return null;
			break;
			
		case BuildingType.bld_workerHouse:
			return prefabs_transBuildings[1];
			break;
			
		case BuildingType.bld_bar:
			return prefabs_transBuildings[2];
			break;
			
		case BuildingType.bld_security:
			return prefabs_transBuildings[3];
			break;
			
		case BuildingType.bld_hotel:
			return prefabs_transBuildings[4];
			break;

		case BuildingType.bld_mine:
			return prefabs_transBuildings[5];
			break;
			
		default:
			return null;
			break;
		}
	}

	///////////////////////////////////

	/// Building Save Handling
	
	public List<int> GetBuildingTypes()
	{
		iBuildingPlacer _tempBuilding;

		List<int> typeList = new List<int>();

		//Loop non-null buildings and build list of their types
		for(int j = 0; j < allBuildings.Count; j++)
		{
			if(allBuildings[j] != null)
			{
				_tempBuilding = allBuildings[j].GetComponent<iBuildingPlacer>();

				typeList.Add(_tempBuilding.getTypeID());
			}
		}
		
		return typeList;
	}

	public List<Vector2> GetBuildingLocations()
	{
		iBuildingPlacer _tempBuilding;
		
		List<Vector2> locList = new List<Vector2>();
		
		//Loop non-null buildings and build list of their locations
		for(int j = 0; j < allBuildings.Count; j++)
		{
			if(allBuildings[j] != null)
			{
				_tempBuilding = allBuildings[j].GetComponent<iBuildingPlacer>();
				
				locList.Add(_tempBuilding.getLocation());
			}
		}
		
		return locList;
	}

	///////////////////////////////////

	/// Building Load Handling

	public void Spawn(List<int> buildingTypes, List<Vector2> buildingLocations)
	{
		GameObject tempGO = null;
		iBuildingReceiver _tempTile;

		//Match counts of types and locations
		if(buildingTypes.Count == buildingLocations.Count)
		{
			for(int j = 0; j < buildingTypes.Count; j++)
			{
				//Spawn prefab corresponding to this type
				tempGO = GameObject.Instantiate(GetBuildingPrefab_forType((BuildingType)buildingTypes[j]));

				//Find the tile it should go on
				_tempTile = Board.instance.getTileAtLoc(buildingLocations[j]);

				//And place it on that tile
				if(tempGO.GetComponent<iBuildingPlacer>().PlaceBuilding(_tempTile))
				{
					//Successfully placed, add to list
					allBuildings.Add(tempGO);

					//Set ID of building
					tempGO.GetComponent<iBuildingPlacer>().Init(j);

					//Parent new building to root
					tempGO.transform.parent = GO_Root.transform;
				}
				else
				{
					//Failed to place on the tile
					DestroyImmediate(tempGO);
				}
			}
		}
	}

	///////////////////////////////////

	/// Key Input tracking - checks if the shift key is held down

	public void ShiftKey_keyPress(keyTracker kt)
	{
		if(kt.is_KeyDown)
		{ 
			buildInGame.keepOn = true;
		}
		else
		{
			//Key has been released

			buildInGame.keepOn = false;
		}
	}
}
