using UnityEngine;
using System.Collections;

//Any mono which should use a timed update should inherit from this class
public class GameTimeMonoBehaviour : MonoBehaviour
{
	private void Start()
	{
		DoStart();
	}
	
	private void DoStart()
	{
		StartCoroutine(Register());
	}
	
	private IEnumerator Register()
	{
		//Wait for GameTime singleton to set up
		yield return new WaitForFixedUpdate();
		
		//Register with the GameTime singleton
		GameTime.instance.Register(this as GameTimeMonoBehaviour);
	}

	public virtual void OnPause(){} // Called by GameTime when pause is called

	public virtual void OnUnpause(){} // Called by GameTime when unpause is called
}
