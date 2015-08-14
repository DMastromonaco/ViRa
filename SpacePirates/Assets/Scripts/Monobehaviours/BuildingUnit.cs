using UnityEngine;
using System.Collections;

public class BuildingUnit : MonoBehaviour, iBuildingPlacer
{
	//Does not need to be assigned in inspector
	public GameObject GO_building;
	public int ID = -1;
	public iBuildingReceiver myReceiverTile = null;

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

	public bool PlaceBuilding(iBuildingReceiver tile)
	{
		if(tile.canReceiveBuilding())
		{
			this.transform.position = tile.GetPlacementLocation();
			tile.ReceiveBuilding(this as iBuildingPlacer);
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

	#endregion
}
