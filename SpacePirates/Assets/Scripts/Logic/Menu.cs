using UnityEngine;
using System.Collections;
using Prime31.MessageKit;

public class Menu : Singleton<Menu>
{
	public MenuGroup menu_Dev;
	public MenuGroup menu_Config;
	public MenuGroup menu_Pause;

	public MenuGroup menu_EdgeInput;

	public MenuGroup menu_Painting;
	public MenuGroup menu_Building;

	public MenuGroup menu_Resources;

	public MenuGroup menu_Sound;

	//IN GAME MENUS
	public MenuGroup menu_Build_InGame;

	//CONFIG MENU
	public ConfigHandler configHandler;

	void Start ()
	{	
		//===== INIT all menus to fix flags
		InitMenus();


		//===== Default states of menus
		DevMenu_on();

		Config_off();

		Painting_off();

		Building_off();

		Resources_off();

		Pause_off();

		//Calling direct close to prevent config XML save
		menu_Sound.Close();

		//===== IN GAME MENUS - Default states

		//Do not turn off the in-game building menu, so the buttons can register correctly when it is on or turned on
		//Building_InGame_off();


		//===== Add Key input handlers
		MessageKit<keyTracker>.addObserver((int)InputMsg.key_esc, DevMenu_keyPress);

		MessageKit<keyTracker>.addObserver((int)InputMsg.key_paint, PaintMenu_keyPress);

		MessageKit<keyTracker>.addObserver((int)InputMsg.key_build, Building_keyPress);
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
	/// Switching - Pause Menu
	
	public void Pause_on()
	{
		menu_Pause.Open();
	}
	
	public void Pause_off()
	{
		menu_Pause.Close();
	}
	
	public void Pause_toggle()
	{
		menu_Pause.ToggleMenu();
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
	/// Switching - DEV - Building Menu
	
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
	/// Switching - Resources Menu
	
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

	//////////////////////////////////////////
	/// Switching - Pause Menu
	
	public void Sound_on()
	{
		menu_Sound.Open();

		//Display the sound config data when the config menu opens
		configHandler.DisplaySoundConfig();
	}
	
	public void Sound_off()
	{
		//Save the sound config data when this menu closes
		configHandler.SaveSoundConfig();

		menu_Sound.Close();
	}
	
	public void Sound_toggle()
	{
		//Calling the other functions so that the save and load of sound config XML fires
		if(menu_Sound.isOpen)
		{
			Sound_off();
		}
		else
		{
			Sound_on();
		}
	}

	//////////////////////////////////////////
	/// Switching - Building Menu - IN GAME
	
	public void Building_InGame_on()
	{
		menu_Build_InGame.Open();
	}
	
	public void Building_InGame_off()
	{
		menu_Build_InGame.Close();

		//Turn off in-game build mode on close
		Buildings.instance.StopBuildingPurchase();
	}
	
	public void Building_InGame_toggle()
	{
		menu_Build_InGame.ToggleMenu();

		if(!menu_Build_InGame.isOpen)
		{
			//Turn off in-game build mode on close
			Buildings.instance.StopBuildingPurchase();
		}
	}



	//////////////////////////////////////////

	/// MENU INITIALIZATION

	//////////////////////////////////////////
	private void InitMenus()
	{
		// This is probably only needed for the IN GAME MENUs, but check all: in case their default states are not handled in Start
		menu_Dev.Init();
		menu_Config.Init();

		menu_Pause.Init();
		menu_EdgeInput.Init();
		menu_Painting.Init();
		menu_Building.Init();
		menu_Resources.Init();
		menu_Sound.Init();

		// IN GAME MENUS
		menu_Build_InGame.Init();
	}
}
