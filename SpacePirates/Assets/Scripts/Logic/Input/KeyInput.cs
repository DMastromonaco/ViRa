using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Prime31.MessageKit;

public class KeyInput : MonoBehaviour
{
	//==================================//
	//========= Keyboard Input =========//
	//==================================//

	// The KeyTracker names MUST match the names assigned in the Unity InputManager
	public keyTracker kt_esc = new keyTracker("Esc", InputMsg.key_esc);
	public keyTracker kt_paint = new keyTracker("Paint", InputMsg.key_paint);
	public keyTracker kt_build = new keyTracker("Build", InputMsg.key_build);
	public keyTracker kt_scroll = new keyTracker("Mouse ScrollWheel", InputMsg.key_scroll);

	public keyTracker kt_hori = new keyTracker("Horizontal", InputMsg.key_horizontal);
	public keyTracker kt_vert = new keyTracker("Vertical", InputMsg.key_vertical);

	public keyTracker kt_shift = new keyTracker("Shift", InputMsg.key_shift);

	private List<keyTracker> allKeys = new List<keyTracker>();

	//Loop var
	int x = 0;

	void Start()
	{
		//Add all keys to the list so we can loop them
		allKeys.Add(kt_esc);
		allKeys.Add(kt_paint);
		allKeys.Add(kt_build);
		allKeys.Add(kt_scroll);
		allKeys.Add(kt_hori);
		allKeys.Add(kt_vert);
		allKeys.Add(kt_shift);
	}

	//Input gathered in update so nothing is missed
	void Update()
	{
		//Loop all keys and check if they have a value, store value and flag for processing
		for(x = 0; x < allKeys.Count; x++)
		{
			if(Input.GetAxis(allKeys[x].s_KeyName) != 0.0f)
			{
				allKeys[x].value_keyAxis = Input.GetAxis(allKeys[x].s_KeyName);
				allKeys[x].do_process = true;
			}
		}
	}

	void CheckKeyReleases()
	{
		//Loop all keys and check if they are zero'd, release press if they were pressed
		for(x = 0; x < allKeys.Count; x++)
		{
			if(Input.GetAxis(allKeys[x].s_KeyName) == 0.0f)
			{
				if(allKeys[x].is_KeyDown)
				{
					allKeys[x].ReleasePress();
				}
			}
		}
	}

	void ProcessKey(int keyMsg, ref keyTracker kt)
	{
		if(kt.is_KeyDown)
		{
			//Key has been held between multiple fixed updates, increment time
			kt.HeldDown();
		}
		else
		{
			//New key press, reinit
			kt.NewPress();
		}
		
		//POST the message for any other scripts subscribed to this message type
		MessageKit<keyTracker>.post( keyMsg, kt );
	}

	//Input processing done in fixed update so everything fires
	void FixedUpdate()
	{
		//Loop all keys and check which were flagged for processing
		for(x = 0; x < allKeys.Count; x++)
		{
			if(allKeys[x].do_process)
			{
				ProcessKey(allKeys[x].keyMsg, ref allKeys[x].self);
				allKeys[x].do_process = false;
			}
		}

		//This will recheck at the end of each fixed update to see if the key has been released or not.
		//Every keypress is guarenteed at least 1 frame of processing in FixedUpdated
		CheckKeyReleases();
	}
}

//===================================//
//===================================//

//======== TRACKING CLASSES  ======== 

//===================================//
//===================================//
#region TRACKING CLASSES

[System.Serializable]
public class keyTracker
{
	public keyTracker self = null;

	private bool _do_process;
	public bool do_process { get{ return _do_process; } set{ _do_process = value; } }

	private bool _is_KeyDown;
	/// <summary>
	/// Gets or sets the state of the key
	/// </summary>
	/// <value>The key is down or not in bool</value>
	public bool is_KeyDown { get{ return _is_KeyDown; } set{ _is_KeyDown = value; } }

	private bool _is_FirstFrame;
	/// <summary>
	/// Gets or sets if the key has just been processed this frame
	/// </summary>
	/// <value>The key is down or not in bool</value>
	public bool is_FirstFrame { get{ return _is_FirstFrame; } set{ _is_FirstFrame = value; } }

	private float _dura_KeyPress;
	/// <summary>
	/// Gets or sets the duration of the current key press
	/// </summary>
	/// <value>The duration in seconds.</value>
	public float dura_KeyPress { get{ return _dura_KeyPress; } set{ _dura_KeyPress = value; } }

	private string _s_KeyName;
	/// <summary>
	/// Gets or sets the name of the current key press (Input Manager name)
	/// </summary>
	/// <value>The key name in string.</value>
	public string s_KeyName { get{ return _s_KeyName; } set{ _s_KeyName = value; } }

	private float _value_keyAxis;
	public float value_keyAxis { get{ return _value_keyAxis; } set{ _value_keyAxis = value; } }

	private int _keyMsg;
	public int keyMsg { get{ return _keyMsg; } set{ _keyMsg = value; } }

	public void NewPress()
	{
		_is_KeyDown = true;
		_is_FirstFrame = true;
		_dura_KeyPress = 0f;
	}

	public void HeldDown()
	{
		_is_FirstFrame = false;
		_dura_KeyPress += Time.fixedDeltaTime;
	}

	public void ReleasePress()
	{
		_is_KeyDown = false;
		_is_FirstFrame = false;
		_value_keyAxis = 0f;

		//Send 1 final message when key is released
		MessageKit<keyTracker>.post( _keyMsg, this );

		_dura_KeyPress = 0f;
	}

	public keyTracker(string newKeyName)
	{
		self = this;
		
		//Initialize everything
		_do_process = false;
		_is_KeyDown = false;
		_is_FirstFrame = false;
		_dura_KeyPress = 0f;
		_value_keyAxis = 0f;
		_s_KeyName = newKeyName;
	}

	public keyTracker(string newKeyName, int newMsg)
	{
		self = this;

		//Initialize everything
		_do_process = false;
		_is_KeyDown = false;
		_is_FirstFrame = false;
		_dura_KeyPress = 0f;
		_value_keyAxis = 0f;
		_s_KeyName = newKeyName;
		_keyMsg = newMsg;
	}

	public keyTracker(string newKeyName, int newMsg, float axisInput)
	{
		self = this;
		
		//Initialize everything
		_do_process = false;
		_is_KeyDown = false;
		_is_FirstFrame = false;
		_dura_KeyPress = 0f;
		_value_keyAxis = axisInput;
		_s_KeyName = newKeyName;
		_keyMsg = newMsg;
	}
}
#endregion
