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

	public bool CanAfford(int cost)
	{
		return (int)Resources.instance.playerRes.m_money >= cost;
	}

	public bool subMoney(int money)
	{
		if((int)Resources.instance.playerRes.m_money < money)
		{
			return false;
		}
		else
		{
			Resources.instance.playerRes.m_money -= (float)money;
			UpdateDisplay();
			return true;
		}
	}

	public void addMoney(int money)
	{
		Resources.instance.playerRes.m_money += (float)money;
		UpdateDisplay();
	}
}
