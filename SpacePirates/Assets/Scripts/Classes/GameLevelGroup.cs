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
