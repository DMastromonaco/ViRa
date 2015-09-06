using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
[XmlRoot("PlayerResources")]
public class PlayerResources
{
	[XmlAttribute]
	public float m_money{get;set;}

	[XmlAttribute]
	public float m_ore{get;set;}

	public PlayerResources()
	{
		m_money = 100f;
		m_ore = 25f;
	}
}
