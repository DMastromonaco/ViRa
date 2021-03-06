﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Prime31.MessageKit;

[RequireComponent(typeof(Image))]
public class BuildingButton : MonoBehaviour
{
	//PUBLIC VARS
	public BuildingType myBuildingType = BuildingType.off; //Assign to correct building enum in inspector

	public InputMsg myHotkey; //Assign to correct input message number key (hotkey) for this button

	//The image component of this button
	private Image _image;

	//The starting color of the UI Image
	private Color _defaultColor;

	//Use awake so this will register once when it is turned on
	public void Awake()
	{
		//get ref to image on this button
		_image = this.gameObject.GetComponent<Image>();

		//store starting color
		_defaultColor = _image.color;

		//===== Add Key input handlers for Hotkey
		MessageKit<keyTracker>.addObserver((int)myHotkey, hotkeyPress);

		DoAwake();
	}

	private void DoAwake()
	{
		StartCoroutine(RegisterAsBuildingButton());
	}

	private IEnumerator RegisterAsBuildingButton()
	{
		//Wait to ensure building singleton is set up
		yield return new WaitForFixedUpdate();

		//Pass a reference of this button to the Building script
		Buildings.instance.RegisterBuildingButton(this as BuildingButton);
	}

	//HOTKEY - from messageKit
	public void hotkeyPress(keyTracker kt)
	{
		//If they press the hotkey for this button, treat same as click of the building button
		if(kt.is_FirstFrame)
		{
			if(this.isActiveAndEnabled)
			{
				BeginBuildingPurchase();
			}
		}
	}


	//Methods for Button.cs and interactive with Buildings.cs singleton
	public void BeginBuildingPurchase()
	{
		Buildings.instance.TryBeginBuildingPurchase(myBuildingType, this as BuildingButton);
	}

	public void ResetBuildingButtons()
	{
		Buildings.instance.ResetAllBuildingButtonColors();
	}

	//METHODS FOR BUILDINGS.cs to call

	//Reset the button back to it's original color
	public void ResetButtonColor()
	{
		_image.color = _defaultColor;
	}

	public void SetButtonColor(Color newColor)
	{
		_image.color = newColor;
	}
}