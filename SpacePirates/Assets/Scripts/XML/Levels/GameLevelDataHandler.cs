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

	public void LoadGameLevelData()
	{
		//Get file name from UI
		string fileName = input_fileName.text;
		//Correct file name with appropriate prefix
		fileName = GameLevelDataNameGen.getName(fileName);
		//Set file name to the XML serializer for loading
		GameLevelDataSerializer.setFileName(fileName);

		//Load from file
		GameLevelData = GameLevelDataSerializer.DeserializeLevelData();

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

			//Get file name from UI
			string fileName = input_fileName.text;
			//Correct file name with appropriate prefix
			fileName = GameLevelDataNameGen.getName(fileName);
			//Set file name to the XML serializer for saving
			GameLevelDataSerializer.setFileName(fileName);

			//Save to file
			GameLevelDataSerializer.SerializeLevelData(GameLevelData);
		}
	}
}
