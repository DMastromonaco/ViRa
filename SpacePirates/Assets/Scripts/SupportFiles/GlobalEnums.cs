﻿using UnityEngine;
using System.Collections;

public enum BrushType
{
	Off = 0,

	DrawSingle = 1,
	DrawMed = 2,
	DrawWide = 3,
	
	EraseSingle = 4,
	EraseMed = 5,
	EraseWide = 6,
}

public enum NeighborDirection
{
	North = 0,
	East = 1,
	South = 2,
	West = 3,
}

public enum MapLayoutNames
{
	maplayout_00 = 0,
	maplayout_01 = 1,
	maplayout_02 = 2,
	maplayout_03 = 3,
	maplayout_04 = 4,
	maplayout_05 = 5,
	maplayout_06 = 6,
	maplayout_07 = 7,
	maplayout_08 = 8,
	maplayout_09 = 9,
	maplayout_10 = 10,
	maplayout_11 = 11,
}


public enum BuildingType
{
	off = 0,

	bld_govMansion = 10,
	bld_workerHouse = 11,
	bld_bar = 12,
	bld_security = 13,
	bld_hotel = 14,
	bld_mine = 15,


	remove = 99,
}


public enum SoundType
{
	None = 0,
	Click = 1,

	Building_remove = 10,
	Building_place = 11,
}


public enum GameTriggerType
{
	None = 0,

	showMessage = 10,

	spawnMap = 15,

	spawnEnemy = 20,
}


//messageTypes for Prime31.MessageKit, these must be cast as (int) to be usable
public enum InputMsg
{
	key_esc = 0,
	key_paint = 1,
	key_build = 2,
	key_scroll = 3,
	
	key_horizontal = 4,
	key_vertical = 5,
	
	key_shift = 6,
	key_tab = 7,
	
	key_num_01 = 11,
	key_num_02 = 12,
	key_num_03 = 13,
	key_num_04 = 14,
	key_num_05 = 15,
	key_num_06 = 16,
	key_num_07 = 17,
	key_num_08 = 18,
	key_num_09 = 19,
	key_num_00 = 20,
}
