using UnityEngine;
using System.Collections;

public interface iClickable
{
	void ClickStart(inputTracker input);
	void ClickEnd(inputTracker input);

	void RightClickStart(inputTracker input);
	void RightClickEnd(inputTracker input);
}
