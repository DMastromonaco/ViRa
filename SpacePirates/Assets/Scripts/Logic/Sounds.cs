using UnityEngine;
using System.Collections;

public class Sounds : Singleton<Sounds>
{
	//Inspector vars
	public AudioSource sound_click;
	
	public void Play(SoundType type)
	{
		if(SoundType.Click == type)
		{
			sound_click.Play();
		}
	}
}
