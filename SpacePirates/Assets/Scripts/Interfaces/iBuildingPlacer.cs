using UnityEngine;
using System.Collections;

public interface iBuildingPlacer
{
	void Init(int index);

	int getID();

	//returns if the building was placed or not
	bool PlaceBuilding(iBuildingReceiver receiver);

	void DestroyBuilding();

	void ChangeLoc(Vector3 newLoc);
}
