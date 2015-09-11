﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLevelProcessor : MonoBehaviour
{
	//XML game level data, including level name and triggers
	public C_GameLevelData gameLevelData;

	private int curTrigger = 0;

	//When the game object for levels are turned on, the FoC for the level begins
	void OnEnable()
	{
		//Debugging TBD : remove
		Debug.Log("+++ starting level : " + gameLevelData.m_levelName);

		//Set game time to 0
		GameTime.instance.ResetGameTime();

		//Unpause the game
		TimeController.instance.Unpause();

		//Set trigger index to start
		curTrigger = 0;

		//Start loop to check when events trigger
		StartCoroutine(Do_CheckTriggers());
	}

	//The level has stopped
	void OnDisable()
	{
		//Stop loop checking event triggers
		StopCoroutine(Do_CheckTriggers());
		
		//Pause the game
		TimeController.instance.Pause();
	}

	//Will check each fixed update if the game time has elapsed past a certain trigger, and process that trigger
	IEnumerator Do_CheckTriggers()
	{
		//Prevent running of empty level
		if(gameLevelData.triggerTimes.Count == 0)
		{
			yield break;
		}

		while(true)
		{
			//Check all triggers for this time with while
			while(GameTime.instance.getGameTime() > gameLevelData.triggerTimes[curTrigger])
			{
				//TBD : Process the current trigger

				//Debugging TBD : remove
				Debug.Log("Process game trigger : " + gameLevelData.triggerTypes[curTrigger].ToString());
				Debug.Log("  with params : " + gameLevelData.triggerParams[curTrigger].ToString());
				Debug.Log("    at time : " + GameTime.instance.getGameTime());

				//Move to next
				curTrigger++;

				//See if we passed the last trigger
				if(curTrigger >=  gameLevelData.triggerTimes.Count)
				{
					//Stop checking triggers
					yield break;
				}
			}

			//Move to waiting for next trigger
			yield return new WaitForFixedUpdate();
		}
	}
}
