using UnityEngine;
using System.Collections;
using Prime31.MessageKit;

public class CameraScroll : MonoBehaviour
{
	public GameObject myCam;
	private Transform camTrans;

	public GameObject loc_bound_ZoomOut;
	private float bound_upper_Y = 0.0f;
	public GameObject loc_bound_ZoomIn;
	private float bound_lower_Y = 0.0f;

	private float maxVel = 100.0f;
	private float minVel = 0.02f;
	private float curVel = 0.0f;

	private float speed = 150.0f;

	private float direction = 1.0f;

	private float time_remaining = 0.0f;
	private float time_totalscrolltime = 0.05f;

	void Awake ()
	{	
		MessageKit<keyTracker>.addObserver((int)InputMsg.key_scroll, Scroll_keyPress);

		camTrans = myCam.GetComponent<Transform>();

		bound_upper_Y = loc_bound_ZoomOut.transform.position.y;
		bound_lower_Y = loc_bound_ZoomIn.transform.position.y;
	}

	public void Scroll_keyPress(keyTracker kt)
	{
		if(kt.is_KeyDown)
		{
			if(kt.value_keyAxis < 0.0f)
			{
				direction = -1.0f;
			}
			else
			{
				if(kt.value_keyAxis > 0.0f)
				{
					direction = 1.0f;
				}
				else
				{
					direction = 0.0f;
				}
			}

			curVel += kt.value_keyAxis + (direction * minVel);
			//Multiply by the scroll speed scroll bar speed multiplier
			curVel *= ConfigHandler.instance.ConfigData.m_zoomSpeed;

			//clamp to max velocity
			curVel = (Mathf.Min(Mathf.Abs(curVel), maxVel)) * direction;

			time_remaining = time_totalscrolltime;
		}
	}

	public void Update()
	{
		if(time_remaining > 0.0f)
		{
			time_remaining = Mathf.Max(time_remaining -= Time.deltaTime, 0.0f);

			//Slerp the cur vel toward 0
			curVel = Mathf.Lerp(curVel, 0f, ((time_totalscrolltime - time_remaining) / time_totalscrolltime)); //calculate elapsed time / total

			//Move Camera, use delta time to sync over devices, and speed to account for how small delta is
			MoveCamera(curVel * Time.deltaTime * speed);
		}
	}

	private Vector3 tempCameraPos = Vector3.zero;

	private void MoveCamera(float moveAmt)
	{
		//used to reset if we over shoot bounds
		tempCameraPos = camTrans.position;

		if(moveAmt > 0.0f)
		{
			if(camTrans.position.y > bound_lower_Y)
			{
				camTrans.Translate(Vector3.forward * moveAmt, Space.Self);

				//on overshoot : reset and then slowly move towards bounds until it is hit
				if(camTrans.position.y < bound_lower_Y)
				{
					camTrans.position = tempCameraPos;
					while(camTrans.position.y > bound_lower_Y)
					{
						camTrans.Translate(Vector3.forward * 0.5f, Space.Self);
					}
				}
			}
		}

		if(moveAmt < 0.0f)
		{
			if(camTrans.position.y < bound_upper_Y)
			{
				camTrans.Translate(Vector3.forward * moveAmt, Space.Self);

				//on overshoot : reset and then slowly move towards bounds until it is hit
				if(camTrans.position.y > bound_upper_Y)
				{
					camTrans.position = tempCameraPos;
					while(camTrans.position.y < bound_upper_Y)
					{
						camTrans.Translate(Vector3.forward * -0.5f, Space.Self);
					}
				}
			}
		}
	}

}
