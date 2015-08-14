using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Scrollbar))]
public class ScrollValueCalc : MonoBehaviour
{
	//Inspector Vars
	public float f_min_adden = 0f;
	public float f_max_mult = 0f;

	private Scrollbar _scroll;

	[HideInInspector]
	private float m_calcVal = -1;
	[HideInInspector]
	public float calcVal 
	{ 
		set
		{
			m_calcVal = value; 
		}
		get
		{
			m_calcVal = (((float)_scroll.value * f_max_mult) + f_min_adden);
			return m_calcVal;
		}
	}

	// Use this for initialization
	void Awake ()
	{
		_scroll = this.gameObject.GetComponent<Scrollbar>();
	}

	public float normalizedCalcVal()
	{
		m_calcVal = (((float)_scroll.value * f_max_mult) + f_min_adden);

		//this reverse engineers the current calculated value into the 0-1 range for a scrollbar
		float retVal = m_calcVal - f_min_adden;

		retVal = retVal / f_max_mult;

		return retVal;
	}

	public float normalizedCalcVal(float value)
	{
		//this reverse engineers the current calculated value into the 0-1 range for a scrollbar
		float retVal = value - f_min_adden;
		
		retVal = retVal / f_max_mult;
		
		return retVal;
	}
}
