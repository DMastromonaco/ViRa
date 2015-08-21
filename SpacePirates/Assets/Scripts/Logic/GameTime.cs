using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameTime : TimedMonoBehaviour 
{
	public static GameTime instance;

	private List<GameTimeMonoBehaviour> TimedMonos = new List<GameTimeMonoBehaviour>();

	public float elapsedGameTime = 0.0f;

	void Awake()
	{
		if(!instance)
		{
			instance = this as GameTime;
		}
		else
		{
			this.enabled = false;
		}
	}

	//////////////////////////////////////////////////

	//Returns the delta time since the last update, corrected by the current time scale
	public float getDeltaTime()
	{
		return deltaTime;
	}

	//Returns the elapsed GameTime (of this level)
	public float getGameTime()
	{
		return elapsedGameTime;
	}

	//Starts a new level by setting elasped time back to 0
	public void ResetGameTime()
	{
		elapsedGameTime = 0.0f;
	}

	//////////////////////////////////////////////////

	//All MonoBehaviours which utilize the elasped GameTime will register with this controller, to receive Pause and UnPause messages
	public void Register(GameTimeMonoBehaviour script)
	{
		TimedMonos.Add(script);
	}

	//////////////////////////////////////////////////

	//Called from TimedMonoBehaviour, used to increment the corrected elapsed time
	//		only called if the game is not paused
	protected override void TimedUpdate()
	{
		//Increment the elasped game time based on the TimedMonoBehaviour delta time
		elapsedGameTime += deltaTime;
	}

	//Pause all GameTimeMonoBehaviour
	protected override void OnPause()
	{
		foreach(GameTimeMonoBehaviour timedScript in TimedMonos)
		{
			timedScript.OnPause();
		}
	}
	
	//Unpause all GameTimeMonoBehaviour
	protected override void OnUnpause()
	{
		foreach(GameTimeMonoBehaviour timedScript in TimedMonos)
		{
			timedScript.OnUnpause();
		}
	}
}
