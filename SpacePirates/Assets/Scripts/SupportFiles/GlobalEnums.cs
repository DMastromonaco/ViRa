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


	remove = 99,
}


public enum SoundType
{
	None = 0,
	Click = 1,

	Building_remove = 10,
	Building_place = 11,
}









// define your messageTypes (which are ints) preferably as const so they can be easily referenced
// and will benefit from code completion
public class InputMsg
{
	public const int key_esc = 0;
	public const int key_paint = 1;
	public const int key_build = 2;
	public const int key_scroll = 3;

	public const int key_horizontal = 4;
	public const int key_vertical = 5;

	public const int key_shift = 6;
}
