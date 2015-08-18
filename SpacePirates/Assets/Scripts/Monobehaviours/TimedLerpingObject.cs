using UnityEngine;
using System.Collections;

public class TimedLerpingObject : TimedMonoBehaviour
{
	public float speed = 0.75f;

	protected override void TimedUpdate()
	{
		//EXAMPLE OF TimedMonoBehaviour DELTA TIME USAGE

		//use the deltaTime from the TimedMonoBehaviour for all calculations
		transform.Translate( new Vector3( 0f, 0f, -1 * deltaTime * speed ) );
	}

	protected override void OnPause()
	{
		Debug.LogError("+++ pausing");
	}

	protected override void OnUnpause()
	{
		Debug.LogError("--- UNpausing");
	}
}
