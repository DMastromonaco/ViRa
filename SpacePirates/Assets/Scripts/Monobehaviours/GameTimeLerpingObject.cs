using UnityEngine;
using System.Collections;

public class GameTimeLerpingObject : GameTimeMonoBehaviour
{
	public float speed = 1.0f;

	//Use Update, NOT FixedUpdate, to stay in sync with the GameTime : TimedMonoBehaviour.cs TimedUpdate() cycle
	private void Update()
	{
		//EXAMPLE OF GameTime DELTA TIME USAGE

		if(!GameTime.instance.paused)
		{
			//use the deltaTime from the GameTime.instance for all calculations
			transform.Translate( new Vector3( 0f, 0f, -1 * GameTime.instance.getDeltaTime() * speed ) );
		}
	}

	public override void OnPause()
	{
		Debug.LogError("+++ pausing AT " + GameTime.instance.getGameTime());
	}

	public override void OnUnpause()
	{
		Debug.LogError("--- UNpausing AT " + GameTime.instance.getGameTime());
	}
}
