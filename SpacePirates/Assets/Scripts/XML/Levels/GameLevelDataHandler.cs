using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameLevelDataHandler : MonoBehaviour
{
	public C_GameLevelData GameLevelData;

	public InputField input_fileName;
	public InputField input_levelName;

	public GameObject GO_GameLevelProcessor;
	private GameLevelProcessor script_gameLevelData = null;

	//////////////////////////////////////////
	
	void Start()
	{
		//Set default game data file
		setFileName(GameLevelDataNameGen.getName(0));
	}

	//////////////////////////////////////////

	public void LoadGameLevelData()
	{
		//Get file name from UI, parse, and set to file
		setFileName(GameLevelDataNameGen.getName(input_fileName.text));

		//Load from XML file
		GameLevelData = XMLSerialization.Deserialize<C_GameLevelData>(_fullFileNamePath);

		//Set level name to UI
		input_levelName.text = GameLevelData.m_levelName;

		//Pass to game level data to the processor
		script_gameLevelData = null;
		script_gameLevelData = GO_GameLevelProcessor.GetComponent<GameLevelProcessor>();
		if(script_gameLevelData != null)
		{
			script_gameLevelData.gameLevelData = GameLevelData;
		}
	}

	//////////////////////////////////////////
	
	//Get settings, set to GameLevelData, and save
	public void SaveGameLevelData()
	{
		GameLevelData = new C_GameLevelData();

		//Pull game level data from the processor
		script_gameLevelData = null;
		script_gameLevelData = GO_GameLevelProcessor.GetComponent<GameLevelProcessor>();
		if(script_gameLevelData != null)
		{
			//Get the settings from the various trigger arrays on the processor object
			GameLevelData = script_gameLevelData.gameLevelData;

			//Get Level Name from UI
			string levelName = input_levelName.text;
			GameLevelData.m_levelName = levelName;

			//Get file name from UI, parse, and set to file
			setFileName(GameLevelDataNameGen.getName(input_fileName.text));

			//Save to XML file
			XMLSerialization.Serialize<C_GameLevelData>(GameLevelData, _fullFileNamePath);
		}
	}

	//////////////////////////////////////////

	//File Name
	private string _fullFileNamePath = ""; //Default set from Start
	private string _filePath = "leveldata/";
	private string _fileName = ""; //Default set from Start
	private string _fileExtension = ".xml";

	//Pass name already parsed from GameLevelDataNameGen
	private void setFileName(string newname)
	{
		_fileName = newname;
		_fullFileNamePath = _filePath + _fileName + _fileExtension;
	}
}
