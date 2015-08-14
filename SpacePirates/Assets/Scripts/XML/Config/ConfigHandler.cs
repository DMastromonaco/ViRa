using UnityEngine;
using UnityEngine.UI;

public class ConfigHandler : Singleton<ConfigHandler>
{
	public C_ConfigSettings ConfigData;

	public Toggle toggle_playIntro;

	public Scrollbar scrollBar_zoomSpeed;
	//Script for scroll value
	public ScrollValueCalc scroll_zoomValue;

	public Scrollbar scrollBar_panSpeed_LR;
	//Script for pan value
	public ScrollValueCalc scroll_panValue_LR;

	public Scrollbar scrollBar_panSpeed_FB;
	//Script for pan value
	public ScrollValueCalc scroll_panValue_FB;

	void Start () 
	{
		//Load from file
		ConfigData = ConfigSerializer.DeserializeConfig();
	}

	//////////////////////////////////////////

	//Load settings from config data into the UI
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

	//////////////////////////////////////////
	
	//Get settings from UI, set to ConfigData, and save
	public void SaveConfig()
	{
		UpdateConfig();

		//Save to file
		ConfigSerializer.SerializeConfig(ConfigData);
	}

	public void UpdateConfig()
	{
		ConfigData.m_playIntro = toggle_playIntro.isOn;
		ConfigData.m_zoomSpeed = scroll_zoomValue.calcVal;
		ConfigData.m_panSpeed_LR = scroll_panValue_LR.calcVal;
		ConfigData.m_panSpeed_FB = scroll_panValue_FB.calcVal;
	}
}
