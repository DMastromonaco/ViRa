using UnityEngine;
using System.Collections;

public interface iBuildingPlacer
{
	//returns if the building was placed or not
	bool PlaceBuilding(iBuildingReceiver receiver);

	void ChangeLoc(Vector3 newLoc);
}
