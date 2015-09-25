using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Resources : Singleton<Resources>
{
	public PlayerResources playerRes;

	public Text lbl_Money;
	public Text lbl_Ore;

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

		lbl_Ore.text = playerRes.m_ore.ToString("F0");
	}

	public bool CanAfford(int cost)
	{
		return (int)Resources.instance.playerRes.m_money >= cost;
	}

	// == All

	public void setAll(int money, int ore)
	{
		Resources.instance.playerRes.m_money = money;
		Resources.instance.playerRes.m_ore = ore;
		UpdateDisplay();
	}

	// == Money

	public bool subMoney(int amt)
	{
		if((int)Resources.instance.playerRes.m_money < amt)
		{
			return false;
		}
		else
		{
			Resources.instance.playerRes.m_money -= (float)amt;
			UpdateDisplay();
			return true;
		}
	}

	public void addMoney(int amt)
	{
		Resources.instance.playerRes.m_money += (float)amt;
		UpdateDisplay();
	}

	// == Ore

	public bool subOre(int amt)
	{
		if((int)Resources.instance.playerRes.m_ore < amt)
		{
			return false;
		}
		else
		{
			Resources.instance.playerRes.m_ore -= (float)amt;
			UpdateDisplay();
			return true;
		}
	}
	
	public void addOre(int amt)
	{
		Resources.instance.playerRes.m_ore += (float)amt;
		UpdateDisplay();
	}
}
