using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class MenuGroup
{
	public bool isOpen = false;
	public GameObject GO_Parent = null;
	public List<GameObject> gos_MenuButtons = new List<GameObject>();

	#region Public Methods

	public void Open()
	{
		if(GO_Parent)
		{
			GO_Parent.SetActive(true);

			//Try to set parent's canvas group too, if it has one
			ToggleCanvasGroup(GO_Parent, true);
		}
		
		isOpen = true;
		
		ToggleSelectable (ref gos_MenuButtons, true);
	}

	public void Close()
	{
		if(GO_Parent)
		{
			GO_Parent.SetActive(false);

			//Try to set parent's canvas group too, if it has one
			ToggleCanvasGroup(GO_Parent, false);
		}

		isOpen = false;
		
		ToggleSelectable (ref gos_MenuButtons, false);
	}
	
	public void ToggleMenu()
	{
		//Toggle actions
		if( isOpen )
		{
			Close();
		}
		else
		{
			Open();
		}
	}

	#endregion

	#region internal toggling and stuff
	private void ToggleSelectable(ref List<GameObject> gos, bool isOn)
	{
		CanvasGroup tempCG = null;

		foreach (GameObject go in gos)
		{
			if(go != null)
			{
				go.GetComponent<Selectable>().interactable = isOn;

				tempCG = go.GetComponent<CanvasGroup>();

				if(tempCG != null)
				{
					tempCG.interactable = isOn;
					tempCG.blocksRaycasts = isOn;
				}

			}
			tempCG = null;
		}
	}

	private void ToggleCanvasGroup(GameObject go, bool isOn)
	{
		CanvasGroup tempCG = null;

		tempCG = go.GetComponent<CanvasGroup>();
			
		if(tempCG != null)
		{
			tempCG.interactable = isOn;
			tempCG.blocksRaycasts = isOn;
		}
	}
	#endregion
}
