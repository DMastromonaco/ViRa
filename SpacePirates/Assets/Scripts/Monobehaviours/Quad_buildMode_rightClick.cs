using UnityEngine;
using System.Collections;

public class Quad_buildMode_rightClick : MonoBehaviour
{
	//If they right click the cancel quad, cancel build mode
	public void RightClickStart(inputTracker input)
	{
		if(Buildings.instance.buildInGame.isOn)
		{
			Buildings.instance.StopBuildingPurchase();
		}
	}
}
