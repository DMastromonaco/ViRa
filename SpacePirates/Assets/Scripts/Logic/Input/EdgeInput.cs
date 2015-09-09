using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31.MessageKit;

public class EdgeInput : MonoBehaviour
{
	//assign in inspector
	public int ktIndex = 0;

	public keyTracker kt_hori_left = new keyTracker("Horizontal_Left", (int)InputMsg.key_horizontal, -1.0f);
	public keyTracker kt_hori_right = new keyTracker("Horizontal_Right", (int)InputMsg.key_horizontal, 1.0f);
	public keyTracker kt_vert_forward = new keyTracker("Vertical_Forward", (int)InputMsg.key_vertical, 1.0f);
	public keyTracker kt_vert_backward = new keyTracker("Vertical_Backward", (int)InputMsg.key_vertical, -1.0f);

	private List<keyTracker> allKTs = new List<keyTracker>();

	void Start()
	{
		//Add in order according to index assigned in editor
		allKTs.Add(kt_hori_left);
		allKTs.Add(kt_hori_right);
		allKTs.Add(kt_vert_forward);
		allKTs.Add(kt_vert_backward);
	}

	//Called from EventSystem PointerEnter()
	public void Move_start()
	{
		allKTs[ktIndex].is_KeyDown = true;

		StartCoroutine(Do_Move());
	}

	//Called from EventSystem PointerExit()
	public void Move_stop()
	{
		allKTs[ktIndex].is_KeyDown = false;
	}
	
	IEnumerator Do_Move()
	{
		while(allKTs[ktIndex].is_KeyDown)
		{
			//Simulate keypress while mouse in in this area
			MessageKit<keyTracker>.post( allKTs[ktIndex].keyMsg, allKTs[ktIndex] );
			yield return new WaitForFixedUpdate();
		}
	}
}
