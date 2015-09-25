using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelControl : Singleton<LevelControl>
{
	//Processor which is used when loading or saving game level data XMLs
	public GameLevelGroup gameLevelProc;

	//"Hard Coded" level objects
	public List<GameLevelGroup> gameLevels;

	////////////////////////////////////////////

	public void StartProcessorLevel()
	{
		gameLevelProc.Level_Start();
	}

	public void ResumeProcessorLevel(float resumeTime)
	{
		gameLevelProc.Level_Resume(resumeTime);
	}

	////////////////////////////////////////////

	public void StartLevel(int lvl)
	{
		if(gameLevels.Count > lvl)
		{
			gameLevels[lvl].Level_Start();
		}
	}

	public void StopLevel(int lvl)
	{
		if(gameLevels.Count > lvl)
		{
			gameLevels[lvl].Level_Stop();
		}
	}

	public void ToggleLevel(int lvl)
	{
		if(gameLevels.Count > lvl)
		{
			gameLevels[lvl].Level_Toggle();
		}
	}

	public void StopAll()
	{
		for(int x = 0; x < gameLevels.Count; x++)
		{
			gameLevels[x].Level_Stop();
		}

		gameLevelProc.Level_Stop();
	}
}
