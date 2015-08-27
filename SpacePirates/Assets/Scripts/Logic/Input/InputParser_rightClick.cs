using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputParser_rightClick : MonoBehaviour
{
	#region Awake() & global singleton assignment
	public static InputParser_rightClick instance;

	void Awake ()
	{
		if(InputParser_rightClick.instance == null)
		{
			instance = this;
		}
		else
		{
			Debug.LogWarning("Multiple instance of InputParser_rightClick singleton found, disabling this one : " + this.gameObject.name.ToString());
			this.enabled = false;
		}

		if(raycast_Cameras.Count == 0)
		{
			Debug.LogWarning("There are no cameras assigned to raycast from the InputParser_rightClick, disabling this : " + this.gameObject.name.ToString());
			this.enabled = false;
		}

		if(LayersForInput == _layerSet_Nothing)
		{
			Debug.LogWarning("InputParser_rightClick - no layers assigned for input : This is not an issue if you intend to assign new LayerMask objects to the InputParser's public mask object 'LayersForInput' during runtime.");
			Debug.LogWarning("InputParser_rightClick - no layers assigned for input : Currently no renderers are going to be raycasted or sent input messages!");
		}

		//Clicks should never be delayed by this script for flow of control purposes, only enough to properly process inputs
		_time_ClickUnDelayed = Time.time;

		//Store screen dimensions so we can easily tell if the mouse is in the screen or not
		_screenW = Screen.width;
		_screenH = Screen.height;

		//If this is built to use mouse, we will add a single internalInputTracker to our list so that we don't have to worry about it later
		//There will only ever be this one tracker in the List (for the mouse); the List remains because it makes the code uniform in FixedUpdate() when 
		//			sending messages, eliminating the need to handle them differently in FixedUpdate()
		_ongoing_Inputs.Add(new internalInputTracker(Vector3.zero));

	}
	#endregion

	#region Public Variables
	/// <summary>
	/// All of the cameras to use for raycasting input from
	/// The closer the camera is to the top of the List (index 0), the higher priority it has for raycasting
	/// (only the first renderer hit by an input during the raycasting loop will register, masking all others)
	/// </summary>
	public List<Camera> raycast_Cameras = new List<Camera>();
	
	/// <summary>
	/// The allowable layers to raycast against for hit detection / input messages
	/// </summary>
	public LayerMask LayersForInput;
	
	/// <summary>
	/// Hard-coded period of time to delay between the processing of clicks.
	/// At least some delay (0.05) is needed in order to parse input correctly
	/// </summary>
	public float delay_Clicks = 0.05f;
	
	/// <summary>
	/// The distance which will be used to raycast from all cameras
	/// (Reduce this to improve performance, but do not make smaller than the largest expected camera render distance)
	/// </summary>
	public float raycast_Distance = 100.0f;
	#endregion

	#region Private Variables
	/// <summary>
	/// Resolution storage
	/// </summary>
	private float _screenW;
	/// <summary>
	/// Resolution storage
	/// </summary>
	private float _screenH;

	/// <summary>
	/// Internal temp variables when looping cameras
	/// </summary>
	private RaycastHit _rayhit;
	/// <summary>
	/// Internal temp variables when looping cameras
	/// </summary>
	private Camera _raycam;
	/// <summary>
	/// Internal temp variables when looping cameras
	/// </summary>
	private bool _foundRendThisPass = false;
	/// <summary>
	/// Internal temp variable used to get the mouse position and keep it on the screen
	/// or used to clamp each touch to the screen while looping
	/// </summary>       
	private Vector3 _v3_tempInputPos;

	/// <summary>
	/// Internal comparison variable to see if we even have any layers to raycast / or to set to raycast against nothing; using TurnOffInput()
	/// </summary>
	private LayerMask _layerSet_Nothing = 0;
	/// <summary>
	/// Internal comparison variable to set all layers for raycasting; using overloaded TurnOnInput()
	/// </summary>
	private LayerMask _layerSet_Everything = -1;

	/// <summary>
	/// The time when clicks will be allowed again (click time + delay_Clicks)
	/// </summary>
	private float _time_ClickUnDelayed;

	/// <summary>
	/// General Info :
	/// This is the input tracker List which is used to store info on both mouse and touch position, time, renderers, et al. between frames
	/// These are each refreshed in every Update() and reprocessed based on the boolean flags every FixedUpdate()
	/// Touch Specific :
	/// These are refreshed and marked Clean in every Update(), and flagged dirty if they are updated
	/// Any that are marked still marked Clean in FixedUpdate() will process as touch ends; Dirty touches are still ongoing
	/// </summary>
	private List<internalInputTracker> _ongoing_Inputs = new List<internalInputTracker>();

	/// <summary>
	/// General Info :
	/// These will be removed from the _ongoing_Inputs list at the end of FixedUpdate() after processing;
	/// such that that List does not become messed up during queueing, handling, or processing steps
	/// Touch Specific :
	/// The mouse input will stay in the _ongoing_Inputs List as the only entry;
	/// This removal process is for touch inputs only (see Touch Handling in Update())
	/// </summary>
	private List<internalInputTracker> _queuedRemoval_Inputs = new List<internalInputTracker>();

	/// <summary>
	/// Looping var used during FixedUpdate to go thru the above List on inputs and send messages as needed
	/// </summary>
	private internalInputTracker _loopedInputTracker;
	#endregion

	#region Debugging Variable
	/// <summary>
	/// Testing: Provides Debug.LogError messages when inputs are queued or sent
	/// </summary>
	public bool _enableDebugging = false; //TBD ! Change to false !
	/// <summary>
	/// Testing: Provides Debug.LogError messages of the Parent Name when inputs are queued or sent
	/// </summary>
	public bool _enableDebugging_Parents = false; //TBD ! Change to false !
	/// <summary>
	/// This is for testing purposes only
	/// Provides Debug.LogError messages when inputs are queued or sent
	/// It probably should not ever be enabled, unless the InputParser itself is being debugged
	/// </summary>
	/// <value>If Debug.LogError messages are being sent for InputParser activity.</value>
	public bool EnableDebugging { get{ return _enableDebugging; } set{ _enableDebugging = value; } }
	#endregion

	#region Enable and Disable input parsing and messages
	/// <summary>
	/// Enables parsing of input for all layers.
	/// Any object with a renderer will begin receiving input messages Click/Hover Start/End
	/// </summary>
	public void TurnOnInput()
	{
		LayersForInput = _layerSet_Everything;
	}

	/// <summary>
	/// Enables parsing of input for the given layers.
	/// Any object with a renderer on a layer in the passed LayerMask will begin receiving input messages Click/Hover Start/End
	/// </summary>
	public void TurnOnInput(LayerMask newInputLayers)
	{
		LayersForInput = newInputLayers;
	}

	/// <summary>
	/// Disables parsing of input.
	/// No Objects will receive input messages Click/Hover Start/End
	/// </summary>
	public void TurnOffInput()
	{
		LayersForInput = _layerSet_Nothing;
	}
	#endregion

	/// <summary>
	/// All tracker class objects (_ongoing_Inputs) are populated with the current ongoing inputs during Update()
	/// (Update is called on CPU clocks, so this way we do not miss any input)
	/// </summary>
	void Update ()
	{		
		//If there are no layers to be raycast, we can skip everything else for Update(), as there is no input to gather
		if(LayersForInput != _layerSet_Nothing)
		{
			//===================================//
			//===================================//
			
			//====== MOUSE BASED INPUT  ====== START
			
			//===================================//
			//===================================//
			
			#region MOUSE BASED INPUT - Caching inputs

			//Update position of mouse tracker
			_v3_tempInputPos = Input.mousePosition;
			
			//Make sure the position is on the screen
			_v3_tempInputPos = new Vector3(Mathf.Clamp(_v3_tempInputPos.x, 0f, _screenW),
			                          Mathf.Clamp(_v3_tempInputPos.y, 0f, _screenH),
			                          _v3_tempInputPos.z);
			
			// == Update position of mouse tracker
			//There is only ever going to be 1 element in the _ongoing_Input Lists, so ref mouse by index 0
			_ongoing_Inputs[0].input_Tracking.pos_Current = _v3_tempInputPos;

			// == Update the renderer that the mouse is on this frame
			//First clear off any that may have been hit from last frame
			_ongoing_Inputs[0].rend_RaycastHit = null;
			//Flag off until we manage to raycast at least 1 renderer on the correct layer
			_foundRendThisPass = false;
			//Loop all cameras and raycast from them, dropping out of loop if a renderer is hit
			for(int x = 0; x < raycast_Cameras.Count; x++)
			{
				//temp assignment
				_raycam = raycast_Cameras[x];
				
				if ( Physics.Raycast( _raycam.ScreenPointToRay( _ongoing_Inputs[0].input_Tracking.pos_Current ), out _rayhit, raycast_Distance, LayersForInput ) )
				{
					//Save the reference of the first renderer that we successfully hit
					//if(_enableDebugging){Debug.LogError("}}} GO Ray GO {{{ " + _rayhit.collider.gameObject.name.ToString());}
					//if(_enableDebugging){Debug.LogError("}}} Rend Ray Hit {{{ " + _rayhit.collider.gameObject.renderer.gameObject.name.ToString());}
					_ongoing_Inputs[0].rend_RaycastHit = _rayhit.collider.gameObject.GetComponent<Renderer>();

					//Store the Hit Point
					
					_ongoing_Inputs[0].input_Tracking.v3_cur_RaycastHitPoint = _rayhit.point;
					
					//Once we have hit one renderer, we don't need to bother casting from the other cameras
					x = raycast_Cameras.Count + 99;
					_foundRendThisPass = true;
				}
			}

			//Now that we have either raycast something or null, we will pass that to our CurrentRends
			_ongoing_Inputs[0].refresh_afterRaycastDuringUpdate();

			// ==== CLICK HANDLING ====

			//See if a click is happening that hasn't been queued up yet
			if(Input.GetMouseButtonDown(1) && !_ongoing_Inputs[0].mouse_isButtonDown)
			{
				//See if a new click is allowed to be processed yet
				if(_time_ClickUnDelayed < Time.time)
				{
					_ongoing_Inputs[0].mouse_isButtonDown = true;

					if(_ongoing_Inputs[0].rend_RaycastHit)
					{
						//Set delay to prevent multiple clicks, since we intend to process this one
						_time_ClickUnDelayed = Time.time + delay_Clicks;

						if(_enableDebugging){Debug.LogError("((( queue ClickStart");}
						_ongoing_Inputs[0].queueInput_ClickStart();
					}
				}
			}

			//See if a click stopped that hasn't been queued up yet
			if(Input.GetMouseButtonUp(1) && _ongoing_Inputs[0].mouse_isButtonDown)
			{
				_ongoing_Inputs[0].mouse_isButtonDown = false;

				if(_ongoing_Inputs[0].rend_RaycastHit || _ongoing_Inputs[0].rend_Current_ClickHit)
				{
					if(_enableDebugging){Debug.LogError("((( queue ClickEnd");}
					_ongoing_Inputs[0].queueInput_ClickEnd();
				}
			}
			
			if(Input.GetMouseButton(1) && _ongoing_Inputs[0].mouse_isButtonDown)
			{
				//If a click is still happening that we already knew about, update it's duration
				_ongoing_Inputs[0].updateDura_Click();
			}

			//===================================//
			//===================================//
			
			//====== MOUSE BASED INPUT  ====== END
			
			//===================================//
			//===================================//
			#endregion
		} // == END OF if(LayersForInput != _layerSet_Nothing)

	} // == END OF Update()


	/// <summary>
	/// All tracker class objects are processed during FixedUpdate() if there have been any changes flagged for them since the last FixedUpdate()
	/// (Fixed update is called on game cycles, so this way we do not miss any processing)
	/// </summary>
	void FixedUpdate()
	{	
			//===================================//
			//===================================//
			
			//====== INPUT - Processing inputs  ====== START
			
			//===================================//
			//===================================//
			
			#region INPUT - Processing inputs

			foreach(internalInputTracker _loopedInputTracker in _ongoing_Inputs)
			{
				//		Ends need to be processed before Starts

				// === CLICK END ===
				if(_loopedInputTracker.ClickEnd_NeedsProcessing && _loopedInputTracker.rend_Previous_ClickHit)
				{
					//Process Click Input End
					_loopedInputTracker.ClickEnd_NeedsProcessing = false;
					
					//Assign the GameObject which has been messaged to the external generic inputTracker
					_loopedInputTracker.input_Tracking.GO_beingClicked = (GameObject)(_loopedInputTracker.rend_Previous_ClickHit.gameObject);
					
					//Send the actual message, passing a ref to the inputTracker object
					_loopedInputTracker.rend_Previous_ClickHit.SendMessage("RightClickEnd", _loopedInputTracker.input_Tracking, SendMessageOptions.DontRequireReceiver);
					
					//Clear the GameObject which has been messaged to the external generic inputTracker
					//  Now that the clickend has been sent
					_loopedInputTracker.input_Tracking.GO_beingClicked = null;
					
					//Debugging
					if(_enableDebugging){Debug.LogError("RightClickEnd sent ==== - === >>> " + _loopedInputTracker.rend_Previous_ClickHit.gameObject.name.ToString());}
					if(_enableDebugging_Parents){Debug.LogError("PPP RightClickEnd sent ==== - === >>> " + _loopedInputTracker.rend_Previous_ClickHit.gameObject.transform.parent.gameObject.name.ToString());}
					
					//Reset after processing a click end
					_loopedInputTracker.clearClickData();
				}

				// === CLICK START ===
				if(_loopedInputTracker.ClickStart_NeedsProcessing && _loopedInputTracker.rend_Current_ClickHit)
				{
					//Process Click Input Start
					_loopedInputTracker.ClickStart_NeedsProcessing = false;

					//Assign the GameObject which has been messaged to the external generic inputTracker
					_loopedInputTracker.input_Tracking.GO_beingClicked = (GameObject)(_loopedInputTracker.rend_Current_ClickHit.gameObject);

					//Send the actual message, passing a ref to the inputTracker object
					_loopedInputTracker.rend_Current_ClickHit.SendMessage("RightClickStart", _loopedInputTracker.input_Tracking, SendMessageOptions.DontRequireReceiver);

					//Debugging
					if(_enableDebugging){Debug.LogError("RightClickStart sent ==== + === >>> " + _loopedInputTracker.rend_Current_ClickHit.gameObject.name.ToString());}
					if(_enableDebugging_Parents){Debug.LogError("PPP RightClickStart sent ==== + === >>> " + _loopedInputTracker.rend_Current_ClickHit.gameObject.transform.parent.gameObject.name.ToString());}
				}
				
				//Shuffle the current hover and click renderers to the previous, no matter what they were or if we processed them this frame, or w/e (shouldn't matter)
				//This is the end of FixedUpdate shift of data to prepare for the next frame
				_loopedInputTracker.refresh_endOfFixedUpdate();
			}

			//===================================//
			//===================================//
			
			//====== INPUT - Processing inputs  ====== END
			
			//===================================//
			//===================================//
			#endregion
		
	} //End of FixedUpdate()


	#region Public screen refresh
	/// <summary>
	/// This function MUST be called if the screen resolution changes (ex: orientation rotate, etc)
	/// 	Simply call :
	/// InputParser.instance.RefreshScreenDims();
	/// 	after you change the resolution / dimentions of any of the Input cameras.
	/// </summary>
	public void RefreshScreenDims()
	{
		_screenW = Screen.width;
		_screenH = Screen.height;
	}
	#endregion

} //End of InputParser_rightClick : Mono