using UnityEngine;
using System.Collections;

public interface iHoverable
{
	void HoverStart(inputTracker input);
	void HoverEnd(inputTracker input);
}
