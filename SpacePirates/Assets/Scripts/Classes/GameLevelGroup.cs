using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameLevelGroup
{
	public bool isOn = false;
	public GameObject GO_LevelScript = null;

	public void Level_Start()
	{
		if(GO_LevelScript)
		{
			if(!isOn)
			{
				GameLevelProcessor processor = GO_LevelScript.GetComponent<GameLevelProcessor>();
				
				if(processor)
				{
					//Reset internal game processing flag and time
					//   for fresh game level start
					processor.Reset();
				}

				GO_LevelScript.SetActive(true);

				isOn = true;
			}
		}
	}
	
	public void Level_Stop()
	{
		if(GO_LevelScript)
		{
			if(isOn)
			{
				GO_LevelScript.SetActive(false);
				
				isOn = false;
			}
		}
	}

	public void Level_Resume(float gameTime)
	{
		if(GO_LevelScript)
		{
			GameLevelProcessor processor = GO_LevelScript.GetComponent<GameLevelProcessor>();

			if(processor)
			{
				//Set resume flag and time
				processor.Resume(gameTime);
			}

			//and start
			GO_LevelScript.SetActive(true);
			
			isOn = true;
		}
	}
	
	public void Level_Toggle()
	{
		//Toggle actions
		if( isOn )
		{
			Level_Stop();
		}
		else
		{
			Level_Start();
		}
	}
}
