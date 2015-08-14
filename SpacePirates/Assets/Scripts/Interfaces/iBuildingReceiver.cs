using UnityEngine;
using System.Collections;

public interface iBuildingReceiver
{
	void ReceiveBuilding(iBuildingPlacer building);

	bool canReceiveBuilding();

	Vector3 GetPlacementLocation();
}
