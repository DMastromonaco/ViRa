using UnityEngine;
using System.Collections;

[System.Serializable]
public class PaintBrush
{
	public bool isOn = false;
	
	public BrushType currentBrush = BrushType.Off;

	public PaintBrush()
	{
		isOn = false;
		currentBrush = BrushType.Off;
	}
}
