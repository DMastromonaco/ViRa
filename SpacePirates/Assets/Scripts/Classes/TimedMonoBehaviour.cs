using UnityEngine;
using System.Collections;

//Any mono which should use a timed update should inherit from this class
public class TimedMonoBehaviour : MonoBehaviour
{
	protected float timeScale = 1f; //Time scale is used to speed or slow down the custom deltaTime
	protected bool paused = false;
	protected float deltaTime{ get; private set; } // Can be used in sub-classes
	
	private float lastUpdate = 0f;

	private void Start()
	{
		DoStart();
	}

	private void DoStart()
	{
		StartCoroutine(TimeLoop());
	}

	IEnumerator TimeLoop()
	{
		//Wait for TimeController singleton to set up
		yield return new WaitForFixedUpdate();

		//Register with the TimeController
		TimeController.instance.Register(this as TimedMonoBehaviour);

		//Loop here
		while( Application.isPlaying )
		{
			deltaTime = (Time.realtimeSinceStartup - lastUpdate) * timeScale;
			lastUpdate = Time.realtimeSinceStartup;

			if( !paused ) 
			{
				TimedUpdate();
			}
			yield return null;
		}
	}

	protected virtual void TimedUpdate(){} // Replace Update

	public void Pause()
	{
		paused = true;
		OnPause();
	}

	protected virtual void OnPause(){} // Triggers when pause is called

	public void Unpause()
	{
		paused = false;
		OnUnpause();
	}

	protected virtual void OnUnpause(){} // Triggers when pause is called

	//speed or slow down the custom deltaTime, called from TimeController
	public void SetTimeScale(float newTimeScale)
	{
		timeScale = newTimeScale;
	}
}
