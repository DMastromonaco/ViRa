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
		allKTs.Add(kt_hori_left);
		allKTs.Add(kt_hori_right);
		allKTs.Add(kt_vert_forward);
		allKTs.Add(kt_vert_backward);
	}

	private bool _isMoving = false;

	public void Move_start()
	{
		_isMoving = true;
		StartCoroutine(Do_Move());
	}
	
	public void Move_stop()
	{
		_isMoving = false;
	}
	
	IEnumerator Do_Move()
	{
		while(_isMoving)
		{
			MessageKit<keyTracker>.post( allKTs[ktIndex].keyMsg, allKTs[ktIndex] );
			yield return new WaitForFixedUpdate();
		}
	}
}
