using UnityEngine;
using System.Collections;

public interface iBuildingReceiver
{
	iBuildingPlacer getPlacedBuilding();

	void ReceiveBuilding(iBuildingPlacer building);

	bool RemoveBuilding();

	bool hasBuilding();

	bool canReceiveBuilding();

	Vector3 getPlacementLocation();

	Vector2 getRowCol();

	void ClearReceivedBuilding();
}
