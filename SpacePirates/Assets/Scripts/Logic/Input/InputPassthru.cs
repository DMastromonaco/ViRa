using UnityEngine;
using System.Collections;

public class InputPassthru : MonoBehaviour
{
	public GameObject GO_target;
	private iClickable target = null;

	void Start()
	{
		target = GO_target.GetComponent<iClickable>();

		if(target == null)
		{
			Debug.LogWarning("No target for " + this.gameObject.name);
			this.enabled = false;
		}
	}

	public void ClickStart(inputTracker input)
	{
		target.ClickStart(input);
	}

	public void ClickEnd(inputTracker input)
	{
		target.ClickEnd(input);
	}

	public void RightClickStart(inputTracker input)
	{
		target.RightClickStart(input);
	}
	
	public void RightClickEnd(inputTracker input)
	{
		target.RightClickEnd(input);
	}

	public void HoverStart(inputTracker input)
	{
		target.HoverStart(input);
	}
	
	public void HoverEnd(inputTracker input)
	{
		target.HoverEnd(input);
	}
}
