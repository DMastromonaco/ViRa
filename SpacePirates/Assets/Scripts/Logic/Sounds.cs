using UnityEngine;
using System.Collections;

public class Sounds : Singleton<Sounds>
{
	//Inspector vars
	public AudioSource sound_click;

	public AudioSource building_remove;

	public AudioSource building_place;
	
	public void Play(SoundType type)
	{
		//Check the global config to make sure sounds are active
		if(ConfigHandler.instance.ConfigData.m_soundEnabled)
		{
			//Update the audio source volumes based on the config data before playing a sound
			UpdateAudioSourceVolumes();

			if(SoundType.Click == type)
			{
				sound_click.Play();
			}

			if(SoundType.Building_remove == type)
			{
				building_remove.Play();
			}

			if(SoundType.Building_place == type)
			{
				building_place.Play();
			}
		}
	}

	public void PlayButtonClick()
	{
		Play (SoundType.Click);
	}

	//Update the volume of the audio sources based on the appropriate sound settings in ConfigData
	private void UpdateAudioSourceVolumes()
	{
		float f_MusicVol = ConfigHandler.instance.ConfigData.m_vol_Master *
			ConfigHandler.instance.ConfigData.m_vol_Music;

		float f_EffectsVol = ConfigHandler.instance.ConfigData.m_vol_Master *
			ConfigHandler.instance.ConfigData.m_vol_Effects;

		float f_InterfaceVol = ConfigHandler.instance.ConfigData.m_vol_Master *
			ConfigHandler.instance.ConfigData.m_vol_Interface;

		//INTERFACE Audio Sources
		sound_click.volume = f_InterfaceVol;

		//EFFECTS Audio Sources
		building_remove.volume = f_EffectsVol;

		building_place.volume = f_EffectsVol;

		//MUSIC Audio Sources

	}
}
