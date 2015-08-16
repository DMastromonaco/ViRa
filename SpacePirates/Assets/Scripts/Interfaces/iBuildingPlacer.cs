using UnityEngine;
using System.Collections;

public interface iBuildingPlacer
{
	void Init(int index);

	int getID();

	int getTypeID();

	Vector2 getLocation();

	//returns if the building was placed or not
	bool PlaceBuilding(iBuildingReceiver receiver);

	void DestroyBuilding();

	void ChangeLoc(Vector3 newLoc);

	//Returns the sounds stored for this building
	SoundType getPlacementSound();
	SoundType getRemovalSound();
}
