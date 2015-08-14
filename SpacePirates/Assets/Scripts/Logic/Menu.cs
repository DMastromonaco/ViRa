using UnityEngine;
using System.Collections;
using Prime31.MessageKit;

public class Menu : Singleton<Menu>
{
	public MenuGroup menu_Dev;
	public MenuGroup menu_Config;

	public MenuGroup menu_EdgeInput;

	public MenuGroup menu_Painting;
	public MenuGroup menu_Building;

	public MenuGroup menu_Resources;

	//CONFIG MENU
	public ConfigHandler configHandler;

	void Awake ()
	{	
		//=====Default states of menus
		DevMenu_on();

		Config_off();

		Painting_off();

		Resources_off();

		//===== Add Key input handlers
		MessageKit<keyTracker>.addObserver(InputMsg.key_esc, DevMenu_keyPress);

		MessageKit<keyTracker>.addObserver(InputMsg.key_paint, PaintMenu_keyPress);

		MessageKit<keyTracker>.addObserver(InputMsg.key_build, Building_keyPress);
	}
	
	//////////////////////////////////////////
	/// Switching - Dev Menu
	
	public void DevMenu_on()
	{
		menu_Dev.Open();
	}
	
	public void DevMenu_off()
	{
		menu_Dev.Close();
	}

	public void DevMenu_keyPress(keyTracker kt_esc)
	{
		if(kt_esc.is_FirstFrame)
		{
			Sounds.instance.Play(SoundType.Click);

			menu_Dev.ToggleMenu();
		}
	}

	//////////////////////////////////////////
	/// Switching - Config Settings Menu
	
	public void Config_on()
	{
		menu_Config.Open();

		//Display the config data when the config menu opens
		configHandler.DisplayConfig();
	}
	
	public void Config_off()
	{
		menu_Config.Close();
	}

	public void Config_toggle()
	{
		menu_Config.ToggleMenu();

		if(menu_Config.isOpen)
		{
			//Display the config data when the config menu opens
			configHandler.DisplayConfig();
		}
	}

	//////////////////////////////////////////
	/// Switching - Edge Input
	
	public void EdgeInput_on()
	{
		menu_EdgeInput.Open();
	}
	
	public void EdgeInput_off()
	{
		menu_EdgeInput.Close();
	}

	public void EdgeInput_toggle()
	{
		menu_EdgeInput.ToggleMenu();
	}

	//////////////////////////////////////////
	/// Switching - Painting Menu
	
	public void Painting_on()
	{
		menu_Painting.Open();
	}
	
	public void Painting_off()
	{
		menu_Painting.Close();
	}

	public void Painting_toggle()
	{
		menu_Painting.ToggleMenu();
	}

	public void PaintMenu_keyPress(keyTracker kt)
	{
		if(kt.is_FirstFrame)
		{
			Sounds.instance.Play(SoundType.Click);

			menu_Painting.ToggleMenu();
		}
	}

	//////////////////////////////////////////
	/// Switching - Building Menu
	
	public void Building_on()
	{
		menu_Building.Open();
	}
	
	public void Building_off()
	{
		menu_Building.Close();
	}
	
	public void Building_toggle()
	{
		menu_Building.ToggleMenu();
	}
	
	public void Building_keyPress(keyTracker kt)
	{
		if(kt.is_FirstFrame)
		{
			Sounds.instance.Play(SoundType.Click);
			
			menu_Building.ToggleMenu();
		}
	}

	//////////////////////////////////////////
	/// Switching - Building Menu
	
	public void Resources_on()
	{
		menu_Resources.Open();

		Resources.instance.UpdateDisplay();
	}
	
	public void Resources_off()
	{
		menu_Resources.Close();
	}
	
	public void Resources_toggle()
	{
		menu_Resources.ToggleMenu();

		if(menu_Resources.isOpen)
		{
			Resources.instance.UpdateDisplay();
		}
	}
}
