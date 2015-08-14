using UnityEngine;
using System.Collections;

public class MapNameAssignment : MonoBehaviour
{
	public void ChangeMapSaveName(int newName)
	{
		MapLayoutNames newLayoutName = (MapLayoutNames)newName;

		MapLayoutSerializer.ChangeSaveName(newLayoutName.ToString());
	}
}
