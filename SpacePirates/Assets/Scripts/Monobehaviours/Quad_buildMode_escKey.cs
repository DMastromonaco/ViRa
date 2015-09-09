using UnityEngine;
using System.Collections;
using Prime31.MessageKit;

public class Quad_buildMode_escKey : MonoBehaviour
{
	//If they press esc, cancel build mode
	public void keyPress(keyTracker kt)
	{
		if(kt.is_FirstFrame)
		{
			if(Buildings.instance.buildInGame.isOn)
			{
				Buildings.instance.StopBuildingPurchase();
			}
		}
	}

	public void OnEnable()
	{
		MessageKit<keyTracker>.addObserver((int)InputMsg.key_esc, keyPress);
	}

	public void OnDisable()
	{
		MessageKit<keyTracker>.removeObserver((int)InputMsg.key_esc, keyPress);
	}
}
