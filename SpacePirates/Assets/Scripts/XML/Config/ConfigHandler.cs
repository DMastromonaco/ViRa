using UnityEngine;
using UnityEngine.UI;

public class ConfigHandler : Singleton<ConfigHandler>
{
	//File Name
	private static string _configFileName = "config.xml";

	public C_ConfigSettings ConfigData;

	public Toggle toggle_playIntro;

	// PAN AND ZOOM config

	public Scrollbar scrollBar_zoomSpeed;
	//Script for scroll value
	public ScrollValueCalc scroll_zoomValue;

	public Scrollbar scrollBar_panSpeed_LR;
	//Script for pan value
	public ScrollValueCalc scroll_panValue_LR;

	public Scrollbar scrollBar_panSpeed_FB;
	//Script for pan value
	public ScrollValueCalc scroll_panValue_FB;

	// SOUND config

	public Toggle toggle_soundEnabled;

	public Scrollbar scrollBar_vol_Master;
	//Script for scroll value
	public ScrollValueCalc scroll_vol_Master_value;

	public Scrollbar scrollBar_vol_Music;
	//Script for scroll value
	public ScrollValueCalc scroll_vol_Music_value;

	public Scrollbar scrollBar_vol_Effects;
	//Script for scroll value
	public ScrollValueCalc scroll_vol_Effects_value;

	public Scrollbar scrollBar_vol_Interface;
	//Script for scroll value
	public ScrollValueCalc scroll_vol_Interface_value;

	void Start () 
	{
		//Load from file
		ConfigData = XMLSerialization.Deserialize<C_ConfigSettings>(_configFileName);
	}

	//////////////////////////////////////////

	//Load settings from config data into the UI
	// This function sets values to the 'ConfigMenu'
	public void DisplayConfig()
	{
		//Store in temp so that it doesn't get overwritten while assigning
		C_ConfigSettings tempConfigData = new C_ConfigSettings(ConfigData);

		//Use TempData to do all setting, as it won't be changed during this process
		toggle_playIntro.isOn = tempConfigData.m_playIntro;

		//Reverse the current multiplier to a 0-1 range for the scroll bar
		float normalizedValue = scroll_zoomValue.normalizedCalcVal(tempConfigData.m_zoomSpeed);
		scrollBar_zoomSpeed.value = normalizedValue;

		//Reverse the current multiplier to a 0-1 range for the scroll bar
		normalizedValue = scroll_panValue_LR.normalizedCalcVal(tempConfigData.m_panSpeed_LR);
		scrollBar_panSpeed_LR.value = normalizedValue;

		//Reverse the current multiplier to a 0-1 range for the scroll bar
		normalizedValue = scroll_panValue_FB.normalizedCalcVal(tempConfigData.m_panSpeed_FB);
		scrollBar_panSpeed_FB.value = normalizedValue;

		//Copy back to correct any overwrites that happened
		ConfigData = tempConfigData;
	}

	// This function sets values to the 'SoundMenu'
	public void DisplaySoundConfig()
	{
		//Store in temp so that it doesn't get overwritten while assigning
		C_ConfigSettings tempConfigData = new C_ConfigSettings(ConfigData);
		
		//Use TempData to do all setting, as it won't be changed during this process
		toggle_soundEnabled.isOn = tempConfigData.m_soundEnabled;
		
		//Reverse the current multiplier to a 0-1 range for the scroll bar
		float normalizedValue = scroll_vol_Master_value.normalizedCalcVal(tempConfigData.m_vol_Master);
		scrollBar_vol_Master.value = normalizedValue;
		
		//Reverse the current multiplier to a 0-1 range for the scroll bar
		normalizedValue = scroll_vol_Music_value.normalizedCalcVal(tempConfigData.m_vol_Music);
		scrollBar_vol_Music.value = normalizedValue;
		
		//Reverse the current multiplier to a 0-1 range for the scroll bar
		normalizedValue = scroll_vol_Effects_value.normalizedCalcVal(tempConfigData.m_vol_Effects);
		scrollBar_vol_Effects.value = normalizedValue;

		//Reverse the current multiplier to a 0-1 range for the scroll bar
		normalizedValue = scroll_vol_Interface_value.normalizedCalcVal(tempConfigData.m_vol_Interface);
		scrollBar_vol_Interface.value = normalizedValue;
		
		//Copy back to correct any overwrites that happened
		ConfigData = tempConfigData;
	}

	//////////////////////////////////////////
	
	//Get settings from UI, set to ConfigData, and save
	public void SaveConfig()
	{
		//Get settings from UI
		UpdateConfig();

		//Save to XML file
		XMLSerialization.Serialize<C_ConfigSettings>(ConfigData, _configFileName);
	}

	//Get settings from UI, set to ConfigData, and save
	public void SaveSoundConfig()
	{
		//Get settings from UI
		UpdateSoundConfig();

		//Save to XML file
		XMLSerialization.Serialize<C_ConfigSettings>(ConfigData, _configFileName);
	}

	//Updates from ConfigMenu
	public void UpdateConfig()
	{
		ConfigData.m_playIntro = toggle_playIntro.isOn;

		ConfigData.m_zoomSpeed = scroll_zoomValue.calcVal;
		ConfigData.m_panSpeed_LR = scroll_panValue_LR.calcVal;
		ConfigData.m_panSpeed_FB = scroll_panValue_FB.calcVal;
	}

	//Updates from SoundMenu
	public void UpdateSoundConfig()
	{
		ConfigData.m_soundEnabled = toggle_soundEnabled.isOn;

		ConfigData.m_vol_Master = scroll_vol_Master_value.calcVal;
		ConfigData.m_vol_Music = scroll_vol_Music_value.calcVal;
		ConfigData.m_vol_Effects = scroll_vol_Effects_value.calcVal;
		ConfigData.m_vol_Interface = scroll_vol_Interface_value.calcVal;
	}

	//DEBUGGING, display of sound levels check
	public void DebugOutputSoundLevels()
	{
		Debug.Log("Sound on : " + ConfigData.m_soundEnabled);
		Debug.Log("Vol Master : " + ConfigData.m_vol_Master);
		Debug.Log("Vol Music : " + ConfigData.m_vol_Music);
		Debug.Log("Vol Effects : " + ConfigData.m_vol_Effects);
		Debug.Log("Vol Interface : " + ConfigData.m_vol_Interface);
	}
}
