using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
[XmlRoot("ConfigSettings")]
public class C_ConfigSettings
{
	[XmlAttribute]
	public bool m_playIntro{get;set;}

	[XmlAttribute]
	public float m_zoomSpeed{get;set;}

	[XmlAttribute]
	public float m_panSpeed_LR{get;set;}

	[XmlAttribute]
	public float m_panSpeed_FB{get;set;}
	
	public C_ConfigSettings()
	{
		m_playIntro = true;
		m_zoomSpeed = 1.50f;
		m_panSpeed_LR = 1.10f;
		m_panSpeed_FB = 1.10f;
	}

	public C_ConfigSettings(C_ConfigSettings clone)
	{
		this.m_playIntro = clone.m_playIntro;
		this.m_zoomSpeed = clone.m_zoomSpeed;
		this.m_panSpeed_LR = clone.m_panSpeed_LR;
		this.m_panSpeed_FB = clone.m_panSpeed_FB;
	}
}
