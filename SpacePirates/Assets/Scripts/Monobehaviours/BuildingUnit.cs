using UnityEngine;
using System.Collections;

public class BuildingUnit : MonoBehaviour, iBuildingPlacer
{
	//Does not need to be assigned in inspector
	public GameObject GO_building;
	public int ID = -1;
	public iBuildingReceiver myReceiverTile = null;
	public Vector2 placedRowCol = Vector2.zero;

	//== PUBLIC inspector vars
	public BuildingType myType;
	public SoundType sound_placement;
	public SoundType sound_removal;

	#region Public Methods

	#endregion

	#region iBuildingPlacer interfaces

	//INIT MUST BE CALLED ! when this prefab is spawned
	public void Init(int index)
	{
		GO_building = this.gameObject;
		
		//Store indentifying information
		ID = index;
	}

	public int getID()
	{
		return ID;
	}

	public int getTypeID()
	{
		return (int)myType;
	}

	public Vector2 getLocation()
	{
		return placedRowCol;
	}

	public bool PlaceBuilding(iBuildingReceiver tile)
	{
		if(tile == null)
		{
			return false;
		}

		if(tile.canReceiveBuilding())
		{
			this.transform.position = tile.getPlacementLocation();
			tile.ReceiveBuilding(this as iBuildingPlacer);

			placedRowCol = tile.getRowCol();

			myReceiverTile = tile;
			return true;
		}
		else
		{
			return false;
		}
	}

	public void DestroyBuilding()
	{
		//Have the tile remove it's reference to this building
		myReceiverTile.ClearReceivedBuilding();

		//Remove from the buildings list
		Buildings.instance.RemoveBuildingFromList(ID);

		//And destroy instantiated prefab
		DestroyImmediate(GO_building);
	}

	public void ChangeLoc(Vector3 newLoc)
	{
		this.transform.position = newLoc;
	}

	public SoundType getPlacementSound()
	{
		return sound_placement;
	}

	public SoundType getRemovalSound()
	{
		return sound_removal;
	}

	#endregion
}
