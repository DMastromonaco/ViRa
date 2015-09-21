using UnityEngine;
using System.Collections;

public class InputPassthru : MonoBehaviour
{
	public GameObject GO_target;
	private iClickable clickTarget = null;
	private iHoverable hoverTarget = null;

	private bool hasClick = false;
	private bool hasHover = false;

	void Start()
	{
		clickTarget = GO_target.GetComponent<iClickable>();
		hoverTarget = GO_target.GetComponent<iHoverable>();

		if(clickTarget != null)
		{
			hasClick = true;
		}

		if(hoverTarget != null)
		{
			hasHover = true;
		}
	}

	public void ClickStart(inputTracker input)
	{
		if(hasClick)
		{
			clickTarget.ClickStart(input);
		}
	}

	public void ClickEnd(inputTracker input)
	{
		if(hasClick)
		{
			clickTarget.ClickEnd(input);
		}
	}

	public void RightClickStart(inputTracker input)
	{
		if(hasClick)
		{
			clickTarget.RightClickStart(input);
		}
	}
	
	public void RightClickEnd(inputTracker input)
	{
		if(hasClick)
		{
			clickTarget.RightClickEnd(input);
		}
	}

	public void HoverStart(inputTracker input)
	{
		if(hasHover)
		{
			hoverTarget.HoverStart(input);
		}
	}
	
	public void HoverEnd(inputTracker input)
	{
		if(hasHover)
		{
			hoverTarget.HoverEnd(input);
		}
	}
}
