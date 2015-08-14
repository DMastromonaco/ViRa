using UnityEngine;
using System.Collections;
using Prime31.MessageKit;

public class CameraPan_hori : MonoBehaviour
{
	public GameObject myCam;
	private Transform camTrans;

	public GameObject loc_bound_Lower;
	public GameObject loc_bound_Upper;
	private Vector2 v2_bound_LowerUpper;

	private float maxVel = 100.0f;
	private float minVel = 0.02f;
	private float curVel = 0.0f;

	private float speed = 5.0f;

	public Vector3 v3_direction;
	private float f_bearing = 1.0f;

	private float time_remaining = 0.0f;
	private float time_totalscrolltime = 0.15f;

	private int myMsgType = InputMsg.key_horizontal;

	void Awake ()
	{	
		MessageKit<keyTracker>.addObserver(myMsgType, keyPress);

		camTrans = myCam.GetComponent<Transform>();

		v2_bound_LowerUpper = new Vector2(loc_bound_Lower.transform.localPosition.x,
			                      loc_bound_Upper.transform.localPosition.x);
	}

	public void keyPress(keyTracker kt)
	{
		if(kt.value_keyAxis < 0.0f)
		{
			f_bearing = -1.0f;
		}
		else
		{
			if(kt.value_keyAxis > 0.0f)
			{
				f_bearing = 1.0f;
			}
			else
			{
				f_bearing = 0.0f;
			}
		}

		curVel += kt.value_keyAxis + (f_bearing * minVel);
		//Multiply by the scroll speed scroll bar speed multiplier
		curVel *= ConfigHandler.instance.ConfigData.m_panSpeed_LR;

		//clamp to max velocity
		curVel = (Mathf.Min(Mathf.Abs(curVel), maxVel)) * f_bearing;

		time_remaining = time_totalscrolltime;
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
		tempCameraPos = camTrans.localPosition;

		if(moveAmt < 0.0f)
		{
			if(camTrans.localPosition.x > v2_bound_LowerUpper.x)
			{
				camTrans.Translate(v3_direction * moveAmt, Space.Self);

				//on overshoot : reset and then slowly move towards bounds until it is hit
				if(camTrans.localPosition.x < v2_bound_LowerUpper.x)
				{
					camTrans.localPosition = tempCameraPos;
					while(camTrans.localPosition.x > v2_bound_LowerUpper.x)
					{
						camTrans.Translate(v3_direction * -0.5f, Space.Self);
					}
				}
			}
		}

		if(moveAmt > 0.0f)
		{
			if(camTrans.localPosition.x < v2_bound_LowerUpper.y)
			{
				camTrans.Translate(v3_direction * moveAmt, Space.Self);

				//on overshoot : reset and then slowly move towards bounds until it is hit
				if(camTrans.localPosition.x > v2_bound_LowerUpper.y)
				{
					camTrans.localPosition = tempCameraPos;
					while(camTrans.localPosition.x < v2_bound_LowerUpper.y)
					{
						camTrans.Translate(v3_direction * 0.5f, Space.Self);
					}
				}
			}
		}
	}

}
