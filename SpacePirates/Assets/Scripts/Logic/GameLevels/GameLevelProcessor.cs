using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameLevelProcessor : MonoBehaviour
{
	//XML game level data, including level name and triggers
	public C_GameLevelData gameLevelData;

	private int curTrigger = 0;

	private bool resume = false;
	private float resumeGameTime = 0f;

	//When the game object for levels are turned on, the FoC for the level begins
	void OnEnable()
	{
		if(!resume)
		{
			// START LEVEL FROM BEGINNING

			//Debugging TBD : remove
			Debug.Log("+++ starting level : " + gameLevelData.m_levelName);

			//Set game time to 0
			GameTime.instance.ResetGameTime();

			//Set trigger index to start
			curTrigger = 0;
		}
		else
		{
			// RESUME GAME IN PROGRESS

			//Set game time to resume time
			GameTime.instance.setGameTime(resumeGameTime);

			//Determine the trigger we should be at based on time
			curTrigger = 0;
			for(int x = 0; x < gameLevelData.triggerTimes.Count; x++)
			{
				if(resumeGameTime > gameLevelData.triggerTimes[x])
				{
					//Add one for each trigger we have already passed
					curTrigger++;
				}
				else
				{
					//If we hit a trigger that is at a future gameTime, drop out of loop
					x = gameLevelData.triggerTimes.Count + 1;
				}
			}
		}

		//Start loop to check when events trigger
		StartCoroutine(Do_CheckTriggers());

		//Unpause the game time
		TimeController.instance.Unpause();
	}

	//The level has stopped
	void OnDisable()
	{
		//Stop loop checking event triggers
		StopCoroutine(Do_CheckTriggers());
		
		//Pause the game
		TimeController.instance.Pause();

		//flag for resume when object is re-enabled
		resume = true;
		resumeGameTime = GameTime.instance.getGameTime();
	}

	//Reset level that was pending resume
	public void Reset()
	{
		resume = false;
		resumeGameTime = 0f;
	}

	//Resume level in progress
	public void Resume(float gameTime)
	{
		//flag for resume when object is enabled
		resume = true;
		resumeGameTime = gameTime;
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
				//Debugging TBD : remove
				Debug.Log("Process game trigger : " + gameLevelData.triggerTypes[curTrigger].ToString());
				Debug.Log("  with params : " + gameLevelData.triggerParams[curTrigger].ToString());
				Debug.Log("    at time : " + GameTime.instance.getGameTime());

				//Process the current trigger
				Events.instance.ProcessTrigger(gameLevelData.triggerTypes[curTrigger],
				                               gameLevelData.triggerParams[curTrigger]);

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
