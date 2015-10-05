using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class TileAttribute : MonoBehaviour
{
	public abstract TileAttributeType getType();

	public abstract string getParameters();

	public abstract void setParameters(string[] arg);
}
