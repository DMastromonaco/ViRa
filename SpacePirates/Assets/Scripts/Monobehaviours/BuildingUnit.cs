using UnityEngine;
using System.Collections;

public class BuildingUnit : MonoBehaviour, iBuildingPlacer
{
	//Does not need to be assigned in inspector
	public GameObject GO_building;
	public int ID = -1;

	#region Public Methods
	
	//INIT MUST BE CALLED ! when this prefab is spawned
	public void Init(int index)
	{
		GO_building = this.gameObject;

		//Store indentifying information
		ID = index;
	}


	#endregion

	#region iBuildingPlacer interfaces

	public bool PlaceBuilding(iBuildingReceiver tile)
	{
		if(tile.canReceiveBuilding())
		{
			this.transform.position = tile.GetPlacementLocation();
			tile.ReceiveBuilding(this as iBuildingPlacer);
			return true;
		}
		else
		{
			return false;
		}
	}

	public void ChangeLoc(Vector3 newLoc)
	{
		this.transform.position = newLoc;
	}

	#endregion
}
