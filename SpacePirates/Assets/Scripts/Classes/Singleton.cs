using UnityEngine;
using System.Collections;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	public static T instance;
	
	public virtual void Awake()
	{
			if(!instance)
			{
				instance = this as T;
			}
			else
			{
				Debug.LogWarning("Multiple instance of Singleton found, disabling this one : " + this.gameObject.name.ToString());
				this.enabled = false;
			}
	}
}