using UnityEngine;
using System.Collections;

public class Quad_buildMode_OutOfBounds : MonoBehaviour
{
	//If they mouse over an out of bounds area, the transparent building should be placed off screen
	public void HoverStart(inputTracker input)
	{
		//IN-GAME TRANSPARENT BUILDINGS - Hover
		if(Buildings.instance.buildInGame.isOn)
		{
			Buildings.instance.MoveTransparentBuildingOffScreen();
		}
	}
}
