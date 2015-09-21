using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoPopup : Singleton<InfoPopup>
{
	public MenuGroup menuPopup;

	public Text name;

	public bool suppressPopup = false;

	void Start ()
	{	
		//===== INIT menu to fix flags
		menuPopup.Init();

		//===== Default states
		Popup_off();
	}
	
	//////////////////////////////////////////
	/// Switching - Menu
	
	public void Popup_on()
	{
		if(!suppressPopup)
		{
			menuPopup.Open();
		}
	}

	public void Popup_off()
	{
		menuPopup.Close();
	}

	//////////////////////////////////////////
	/// Display - Settings

	public void setName(string newname)
	{
		name.text = newname;
	}
}
