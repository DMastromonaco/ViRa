﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapLayoutHandler : MonoBehaviour
{
	public C_MapLayout MapLayout;

	//////////////////////////////////////////

	public void LoadMapLayout()
	{
		//Load from file
		MapLayout = MapLayoutSerializer.DeserializeMapLayout();
	}

	//////////////////////////////////////////
	
	//Get settings, set to MapLayout, and save
	public void SaveMapLayout()
	{
		MapLayout = new C_MapLayout();
		MapLayout.tileLocations = Board.instance.GetMarkedTiles();

		//Save to file
		MapLayoutSerializer.SerializeMapLayout(MapLayout);
	}

	public void SpawnBoard_fromLayout()
	{
		//Load saved XML
		LoadMapLayout();

		//Spawn tiles from the layout
		Board.instance.Spawn (MapLayout.tileLocations);
	}

	public void MarkBoard_fromLayout()
	{
		//Load saved XML
		LoadMapLayout();

		//Clear current marks
		Board.instance.BoardTiles_ClearAllMarks();

		//Mark from the loaded layout
		Board.instance.BoardTiles_MarkFromList (MapLayout.tileLocations);
	}
}
