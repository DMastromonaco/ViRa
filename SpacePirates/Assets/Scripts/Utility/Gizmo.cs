using UnityEngine;
using System.Collections;

public class Gizmo : MonoBehaviour
{
	public Texture2D myGizmoTexture;
	
	void OnDrawGizmos ()
	{
	    Gizmos.DrawIcon (transform.position, myGizmoTexture.name.ToString() + ".gif", false);
	}
}
