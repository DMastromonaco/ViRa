using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelControl : Singleton<LevelControl>
{
	public List<GameLevelGroup> gameLevels;

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
	}
}
