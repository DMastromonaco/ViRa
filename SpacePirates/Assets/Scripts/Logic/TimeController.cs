using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//This collects a list of all TimedMonoBehaviours, so that their timescales can all be changed together
//Also allows pausing the game
public class TimeController : Singleton<TimeController>
{
	//reference the value calc of the scroll bar used to change the time scale
	public ScrollValueCalc timeScrollValue;

	//Ref to checkbox
	public Toggle pauseCheckbox;

	private List<TimedMonoBehaviour> TimedMonos = new List<TimedMonoBehaviour>();

	//Pause all TimedMonoBehaviours
	public void Pause()
	{
		foreach(TimedMonoBehaviour timedScript in TimedMonos)
		{
			timedScript.Pause();
		}
	}

	//Unpause all TimedMonoBehaviours
	public void Unpause()
	{
		foreach(TimedMonoBehaviour timedScript in TimedMonos)
		{
			timedScript.Unpause();
		}
	}

	//Set the time scale on all TimedMonoBehaviours
	public void SetTimeScale(float newTimeScale)
	{
		foreach(TimedMonoBehaviour timedScript in TimedMonos)
		{
			timedScript.SetTimeScale(newTimeScale);
		}
	}

	//All TimedMonoBehaviours will register with this controller
	public void Register(TimedMonoBehaviour script)
	{
		TimedMonos.Add(script);
	}


	/// UI CONTROLS

	public void ScrollBarTimeChanged()
	{
		float newTimeScale = timeScrollValue.calcVal;

		SetTimeScale(newTimeScale);
	}

	public void ToggleChanged()
	{
		bool isPaused = pauseCheckbox.isOn;

		if(isPaused)
		{
			Pause ();
		}
		else
		{
			Unpause ();
		}
	}
}
