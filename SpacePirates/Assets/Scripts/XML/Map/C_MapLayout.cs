using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
[XmlRoot("MapLayout")]
public class C_MapLayout
{
	[XmlArray("TileLocations")]
	[XmlArrayItem("rowCol")]
	public List<Vector2> tileLocations = new List<Vector2>();
	
	public C_MapLayout()
	{
		tileLocations = new List<Vector2>();
	}
}
