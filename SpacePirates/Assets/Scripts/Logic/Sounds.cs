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
