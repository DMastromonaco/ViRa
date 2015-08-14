using UnityEngine;
using System.Collections;

public class DisableGameObject : MonoBehaviour
{
	// Use this for initialization
	void Start ()
	{
		this.gameObject.SetActive(false);
	}
}
