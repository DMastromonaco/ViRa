using UnityEngine;
using System;
using System.Collections;

public class Painting : Singleton<Painting>
{
	//STATE
	public PaintBrush paintBrush = new PaintBrush();

	// Paint Brush Setting

	public void SetPaintBrush(int whatBrush)
	{
		//Only set as valid enum values
		if(Enum.IsDefined(typeof(BrushType), whatBrush))
		{
			paintBrush.currentBrush = (BrushType)whatBrush;
		}
		else
		{
			//otherwise default to off
			paintBrush.currentBrush = BrushType.Off;
		}
	}

	public void StartPainting()
	{
		paintBrush.isOn = true;
	}

	public void StopPainting()
	{
		paintBrush.isOn = false;
	}
}
