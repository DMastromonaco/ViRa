using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapLayoutHandler : Singleton<MapLayoutHandler>
{
	public C_MapLayout MapLayout;

	//////////////////////////////////////////

	//Called from UI buttons from Unity Event System
	public void ChangeMapSaveName(int whatMap)
	{
		string newLayoutName = MapLayoutNameGen.getName(whatMap);
		
		//change the file that the XML serializer is pointing to
		MapLayoutSerializer.ChangeSaveName(newLayoutName);
	}

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

		MapLayout.buildingTypeIDs = Buildings.instance.GetBuildingTypes();

		MapLayout.buildingLocations = Buildings.instance.GetBuildingLocations();

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

	public void SpawnBuildings_fromLayout()
	{
		//Load saved XML
		LoadMapLayout();
		
		//Spawn buildings from the layout
		Buildings.instance.Spawn (MapLayout.buildingTypeIDs, MapLayout.buildingLocations);
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
