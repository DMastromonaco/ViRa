using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
[XmlRoot("GameLevelData")]
public class C_GameLevelData
{
	[XmlAttribute]
	public string m_levelName{get;set;}

	[XmlArray("triggerTimes")]
	[XmlArrayItem("time")]
	public List<float> triggerTimes = new List<float>();

	[XmlArray("gameTriggerTypes")]
	[XmlArrayItem("triggerType")]
	public List<GameTriggerType> triggerTypes = new List<GameTriggerType>();

	[XmlArray("gameTriggerParams")]
	[XmlArrayItem("triggerParams")]
	public List<string> triggerParams = new List<string>();
	
	public C_GameLevelData()
	{
		m_levelName = "blank";

		triggerTimes = new List<float>();
		triggerTypes = new List<GameTriggerType>();
		triggerParams = new List<string>();
	}
}
