using UnityEngine;
using System.Collections;

public class ClickableTest : MonoBehaviour, iClickable, iHoverable
{
	public void ClickStart(inputTracker input)
	{
		Debug.LogError("ClickStart");
	}
	public void ClickEnd(inputTracker input)
	{
		Debug.LogError("ClickEnd");
	}

	public void RightClickStart(inputTracker input)
	{
		Debug.LogError("RightClickStart");
	}
	public void RightClickEnd(inputTracker input)
	{
		Debug.LogError("RightClickEnd");
	}
	
	public void HoverStart(inputTracker input)
	{
		Debug.LogError("HoverStart");
	}
	public void HoverEnd(inputTracker input)
	{
		Debug.LogError("HoverEnd");
	}
}
