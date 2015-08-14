using UnityEngine;
using System.Collections;

public interface iClickable
{
	void ClickStart(inputTracker input);
	void ClickEnd(inputTracker input);

	void HoverStart(inputTracker input);
	void HoverEnd(inputTracker input);
}
