using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

[System.Serializable]
[XmlRoot("ConfigSettings")]
public class C_ConfigSettings
{
	// INTRO BOOL

	[XmlAttribute]
	public bool m_playIntro{get;set;}
	
	// PAN AND ZOOM

	[XmlAttribute]
	public float m_zoomSpeed{get;set;}

	[XmlAttribute]
	public float m_panSpeed_LR{get;set;}

	[XmlAttribute]
	public float m_panSpeed_FB{get;set;}

	// SOUND

	[XmlAttribute]
	public bool m_soundEnabled{get;set;}

	[XmlAttribute]
	public float m_vol_Master{get;set;}

	[XmlAttribute]
	public float m_vol_Music{get;set;}

	[XmlAttribute]
	public float m_vol_Effects{get;set;}

	[XmlAttribute]
	public float m_vol_Interface{get;set;}
	
	public C_ConfigSettings()
	{
		m_playIntro = true;
		m_zoomSpeed = 1.50f;
		m_panSpeed_LR = 1.10f;
		m_panSpeed_FB = 1.10f;

		m_soundEnabled = true;
		m_vol_Master = 1.00f;
		m_vol_Music = 1.00f;
		m_vol_Effects = 1.00f;
		m_vol_Interface = 1.00f;
	}

	public C_ConfigSettings(C_ConfigSettings clone)
	{
		this.m_playIntro = clone.m_playIntro;
		this.m_zoomSpeed = clone.m_zoomSpeed;
		this.m_panSpeed_LR = clone.m_panSpeed_LR;
		this.m_panSpeed_FB = clone.m_panSpeed_FB;

		this.m_soundEnabled = clone.m_soundEnabled;
		this.m_vol_Master = clone.m_vol_Master;
		this.m_vol_Music = clone.m_vol_Music;
		this.m_vol_Effects = clone.m_vol_Effects;
		this.m_vol_Interface = clone.m_vol_Interface;
	}
}
