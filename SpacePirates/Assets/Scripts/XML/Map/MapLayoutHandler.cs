using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MapLayoutHandler : Singleton<MapLayoutHandler>
{
	public C_MapLayout MapLayout;

	//////////////////////////////////////////

	void Start()
	{
		//Set default map file
		setMapFileName(0);
	}

	//////////////////////////////////////////

	//Called from UI buttons from Unity Event System
	public void setMapFileName(int whatMap)
	{
		//change the file that gets passed to the the XML serialization
		setFileName(MapLayoutNameGen.getName(whatMap));
	}

	public void setMapFileName(string whatMap)
	{
		//change the file that gets passed to the the XML serialization
		setFileName(MapLayoutNameGen.getName(whatMap));
	}

	//////////////////////////////////////////

	//File Name
	private string _fullFileNamePath = ""; //Default set from Start
	private string _filePath = "maps/";
	private string _fileName = ""; //Default set from Start
	private string _fileExtension = ".xml";
	
	private void setFileName(string newname)
	{
		_fileName = newname;
		_fullFileNamePath = _filePath + _fileName + _fileExtension;
	}

	//////////////////////////////////////////

	public void LoadMapLayout()
	{
		//Load from XML file
		MapLayout = XMLSerialization.Deserialize<C_MapLayout>(_fullFileNamePath);
	}

	//////////////////////////////////////////
	
	//Get settings, set to MapLayout, and save
	public void SaveMapLayout()
	{
		MapLayout = new C_MapLayout();
		MapLayout.tileLocations = Board.instance.GetMarkedTiles();

		MapLayout.buildingTypeIDs = Buildings.instance.GetBuildingTypes();

		MapLayout.buildingLocations = Buildings.instance.GetBuildingLocations();

		//Save to XML file
		XMLSerialization.Serialize<C_MapLayout>(MapLayout, _fullFileNamePath);
	}

	//////////////////////////////////////////

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
