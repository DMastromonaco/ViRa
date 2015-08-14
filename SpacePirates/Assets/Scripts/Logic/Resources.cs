using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Resources : Singleton<Resources>
{
	public PlayerResources playerRes;

	public Text lbl_Money;

	private const string _s_money_prefix = "";
	private const string _s_money_suffix = " $";

	void Start()
	{
		playerRes = new PlayerResources();
	}

	public void UpdateDisplay()
	{
		lbl_Money.text = _s_money_prefix + 
						 playerRes.m_money.ToString("F0") +
						 _s_money_suffix;
	}
}
