﻿using UnityEngine;
using System;
using System.Collections;

//Events called by game trigger processing
public class Events : Singleton<Events>
{
	public void ProcessTrigger(GameTriggerType trigger, string css_args)
	{
		//Split the string and put the parts into the args array
		string[] args = css_args.Split(',');

		//switch on the trigger type and call appropriate processing function
		switch(trigger)
		{
		case GameTriggerType.None:
			break;

		case GameTriggerType.showMessage:

			break;

		case GameTriggerType.spawnMap_tiles:
			SpawnMap_Tiles(args);
			break;

		case GameTriggerType.spawnMap_buildings:
			SpawnMap_Buildings(args);
			break;

		case GameTriggerType.spawnEnemy:

			break;

		case GameTriggerType.resource_setAll:
			Resources_SetAll(args);
			break;
		}
	}

	// =========== MESSAGES ===========



	// =========== MAP LAYOUTS ===========

	private void SpawnMap_Tiles(string[] args)
	{
		//Clear any current map tiles and buildings before spawning a map
		Buildings.instance.DestroyAndRemoveAllBuildings();
		Board.instance.Despawn();

		//Set map layout name from args
		setMap(args[0]);

		//Spawn the board tiles from the assigned layout file
		MapLayoutHandler.instance.SpawnBoard_fromLayout();
	}

	private void SpawnMap_Buildings(string[] args)
	{
		//Set map layout name from args
		setMap(args[0]);
		
		//Spawn the buildings from the assigned layout file
		MapLayoutHandler.instance.SpawnBuildings_fromLayout();
	}

	private void setMap(string arg)
	{
		int mapNum = -1;
		if(int.TryParse(arg, out mapNum))
		{
			//int for map name
			MapLayoutHandler.instance.setMapFileName(mapNum);
		}
		else
		{
			//string for mapname
			MapLayoutHandler.instance.setMapFileName(arg);
		}
	}

	// =========== ENEMIES ===========




	// =========== RESOURCES ===========
	private void Resources_SetAll(string[] args)
	{
		int money = 0;
		int.TryParse(args[0], out money);
		int ore = 0;
		int.TryParse(args[1], out ore);

		Resources.instance.setAll(money, ore);
	}
}
