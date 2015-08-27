using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputParser : MonoBehaviour
{
	#region Awake() & global singleton assignment
	/// <summary>
	/// Globally accessable singleton script for parsing input on mobile and standalone; SendMessage(s) : ClickStart ClickStop HoverStart HoverStop : 
	/// to renderers which are set to a layer assigned for input on this script, when raycasted by a camera(s) set on this script.
	/// </summary>
	public static InputParser instance;
	
	/// <summary>
	/// Assign the global singleton, or disables if one is already found
	/// Also intialize any necessary internal variables
	/// </summary>
	void Awake ()
	{
		if(InputParser.instance == null)
		{
			instance = this;
		}
		else
		{
			Debug.LogWarning("Multiple instance of InputParser singleton found, disabling this one : " + this.gameObject.name.ToString());
			this.enabled = false;
		}

		if(raycast_Cameras.Count == 0)
		{
			Debug.LogWarning("There are no cameras assigned to raycast from the InputParser, disabling this : " + this.gameObject.name.ToString());
			this.enabled = false;
		}

		if(LayersForInput == _layerSet_Nothing)
		{
			Debug.LogWarning("InputParser - no layers assigned for input : This is not an issue if you intend to assign new LayerMask objects to the InputParser's public mask object 'LayersForInput' during runtime.");
			Debug.LogWarning("InputParser - no layers assigned for input : Currently no renderers are going to be raycasted or sent input messages!");
		}
		
		//Clicks should never be delayed by this script for flow of control purposes, only enough to properly process inputs
		_time_ClickUnDelayed = Time.time;
		
		//Store screen dimensions so we can easily tell if the mouse is in the screen or not
		_screenW = Screen.width;
		_screenH = Screen.height;

		//Flag if we are on mobile or not so that we don't have to use preprocessor directives in the rest of the code
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
			_useMouse = true;
			
			//If this is built to use mouse, we will add a single internalInputTracker to our list so that we don't have to worry about it later
			//There will only ever be this one tracker in the List (for the mouse); the List remains because it makes the code uniform in FixedUpdate() when 
			//			sending messages, eliminating the need to handle them differently in FixedUpdate()
			_ongoing_Inputs.Add(new internalInputTracker(Vector3.zero));
		#else
			_useMouse = false;
			_ongoing_Inputs.Clear();	

			//Initialize the removal list for touches
			_queuedRemoval_Inputs = new List<internalInputTracker>();
			_queuedRemoval_Inputs.Clear();
		#endif
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
	/// Assigned on start to track if we are on Editor/OSX/Standalone/Webplayer or Mobile
	/// </summary>
	private bool _useMouse;

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
			if(_useMouse)
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

				// ==== HOVER HANDLING ====

				//See if we have raycast anything this frame
				if(_foundRendThisPass)
				{
					// == START OF we Found a Renderer this Update ==

					//If we processed Null hover last time, then this one is good for hover start
					if(_ongoing_Inputs[0].rend_Previous_HoverHit == null)
					{
						if(_enableDebugging){Debug.LogError("((( queue HoverStart " + _ongoing_Inputs[0].rend_RaycastHit.gameObject.name.ToString());}
						if(_enableDebugging_Parents){Debug.LogError("PPP ((( queue HoverStart " + _ongoing_Inputs[0].rend_RaycastHit.gameObject.transform.parent.gameObject.name.ToString());}
						_ongoing_Inputs[0].queueInput_HoverStart();
					}
					else
					{
						//Make sure this isn't the hover one we already processed last time
						if(_ongoing_Inputs[0].rend_Previous_HoverHit.GetInstanceID() != _ongoing_Inputs[0].rend_RaycastHit.GetInstanceID())
						{
							// Since this is different than the one we processed last frame, we need to send both a hover end and a hover start
							// !! The hover end must be queued first, because it uses the last frame's current rend; 
							//    which will be overwritten by the hoverstart queue call with the current rend_RaycastHit
							// !! Also, this allows the finalDura_Hover variable of the InputTracker to be updated in case the HoverEnd object wants to know the duration of it's hover
							if(_enableDebugging){Debug.LogError("((( queue HoverEnd " + _ongoing_Inputs[0].rend_Previous_HoverHit.gameObject.name.ToString());}
							if(_enableDebugging_Parents){Debug.LogError("PPP ((( queue HoverEnd " + _ongoing_Inputs[0].rend_Previous_HoverHit.gameObject.transform.parent.gameObject.name.ToString());}
							_ongoing_Inputs[0].queueInput_HoverEnd();

							//Then queue up the hover start, resetting both rend_Current_HoverStart and dura_Hover
							if(_enableDebugging){Debug.LogError("((( queue HoverStart2 "  + _ongoing_Inputs[0].rend_RaycastHit.gameObject.name.ToString());}
							if(_enableDebugging_Parents){Debug.LogError("PPP ((( queue HoverStart2 "  + _ongoing_Inputs[0].rend_RaycastHit.gameObject.transform.parent.gameObject.name.ToString());}
							_ongoing_Inputs[0].queueInput_HoverStart();
						}
						else
						{
							//We're still hovering on the same renderer we already sent a message to
							_ongoing_Inputs[0].updateDura_Hover();
						}
					}

					// == END OF we Found a Renderer this Update ==
				} 
				else // else of if(_foundRendThisPass){ }
				{
					// == START OF no rend this Update ==

					//We did not raycast a renderer this frame; if we have a previous renderer than it needs a HoverStop queue
					if(_ongoing_Inputs[0].rend_Previous_HoverHit != null)
					{
						if(_enableDebugging){Debug.LogError("((( queue HoverEnd2");}
						_ongoing_Inputs[0].queueInput_HoverEnd();
					}

					// == END OF no rend this Update ==
				} // End of else of if(_foundRendThisPass){ }else{ }

				// ==== CLICK HANDLING ====

				//See if a click is happening that hasn't been queued up yet
				if(Input.GetMouseButtonDown(0) && !_ongoing_Inputs[0].mouse_isButtonDown)
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
				if(Input.GetMouseButtonUp(0) && _ongoing_Inputs[0].mouse_isButtonDown)
				{
					_ongoing_Inputs[0].mouse_isButtonDown = false;

					if(_ongoing_Inputs[0].rend_RaycastHit || _ongoing_Inputs[0].rend_Current_ClickHit)
					{
						if(_enableDebugging){Debug.LogError("((( queue ClickEnd");}
						_ongoing_Inputs[0].queueInput_ClickEnd();
					}
				}
				
				if(Input.GetMouseButton(0) && _ongoing_Inputs[0].mouse_isButtonDown)
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
			}
			else // else of if(_useMouse){ }
			{			
				//===================================//
				//===================================//
				
				//====== TOUCH BASED INPUT  ====== START
				
				//===================================//
				//===================================//
				
				#region TOUCH BASED INPUT - Caching inputs

				//Clean all touches, so we know which ones update and don't this frame
				if(_ongoing_Inputs.Count > 0)
				{
					foreach(internalInputTracker _loopedInputTracker in _ongoing_Inputs)
					{
						_loopedInputTracker.Clean();	
					}
				}

				//Loop all touches that are going on, and rectify them against our touchTracker List
				foreach (Touch touch in Input.touches)
				{
					//See if this one is already ongoing
					int alreadyInList_ListIndex = -1;
					//See if this one is already ongoing
					if(_ongoing_Inputs.Count > 0)
					{
						for(int x = 0; x < _ongoing_Inputs.Count; x++)
						{
							if(_ongoing_Inputs[x].touchID == touch.fingerId)
							{
								alreadyInList_ListIndex = x; //This one is in our List already, so get it's index
							}
						}
					}
					
					if(alreadyInList_ListIndex == -1)
					{
						//This is a new touch we haven't been tracking

						//Clamp position of touch tracker
						_v3_tempInputPos = touch.position;
						//Make sure the position is on the screen
						_v3_tempInputPos = new Vector3(Mathf.Clamp(_v3_tempInputPos.x, 0f, _screenW),
						                               Mathf.Clamp(_v3_tempInputPos.y, 0f, _screenH),
						                               _v3_tempInputPos.z);
						
						//Add it to our array list
						_ongoing_Inputs.Add(new internalInputTracker(touch.fingerId, _v3_tempInputPos));
						//New touches added get flagged as Dirty and TouchModeBegan automatically
					}
					else
					{
						//Update an ongoing touch
						
						//Mark as Dirty so it doesn't get parsed as a touch end and removed
						_ongoing_Inputs[alreadyInList_ListIndex].MakeDirty();

						//Update position of touch tracker
						_v3_tempInputPos = touch.position;
						
						//Make sure the position is on the screen
						_v3_tempInputPos = new Vector3(Mathf.Clamp(_v3_tempInputPos.x, 0f, _screenW),
						                               Mathf.Clamp(_v3_tempInputPos.y, 0f, _screenH),
						                               _v3_tempInputPos.z);

						//Update it's new position
						_ongoing_Inputs[alreadyInList_ListIndex].input_Tracking.pos_Current = _v3_tempInputPos;
					}
				} // == END OF foreach (Touch touch in Input.touches)

				//Update the renderers that each of these touches is on this frame
				foreach (internalInputTracker _loopedInputTracker in _ongoing_Inputs)
				{
					//First clear off any that may have been hit from last frame
					_loopedInputTracker.rend_RaycastHit = null;
					//Flag off until we manage to raycast at least 1 renderer on the correct layer
					_foundRendThisPass = false;
					//Loop all cameras and raycast from them, dropping out of loop if a renderer is hit
					for(int x = 0; x < raycast_Cameras.Count; x++)
					{
						//temp assignment
						_raycam = raycast_Cameras[x];
						
						if ( Physics.Raycast( _raycam.ScreenPointToRay( _loopedInputTracker.input_Tracking.pos_Current ), out _rayhit, raycast_Distance, LayersForInput ) )
						{
							//Save the reference of the first renderer that we successfully hit
							_loopedInputTracker.rend_RaycastHit = _rayhit.collider.gameObject.GetComponent<Renderer>();

							//Store the Hit Point

							_loopedInputTracker.input_Tracking.v3_cur_RaycastHitPoint = _rayhit.point;
							
							//Once we have hit one renderer, we don't need to bother casting from the other cameras
							x = raycast_Cameras.Count + 99;
							_foundRendThisPass = true;
						}
					}
					
					//Now that we have either raycast something or null, we will pass that to our current HOVER rend
					//		(click does not get overwritten with null, to save what rend the click started on for later)
					_loopedInputTracker.refresh_afterRaycastDuringUpdate();

					// ==== HOVER HANDLING ====
					
					//See if we have raycast anything this frame
					if(_foundRendThisPass)
					{
						// == START OF we Found a Renderer this Update ==

						//Processing HOVER first :
						//Click messages will be handled differently afterwards, using the _isTouchModeStart flag
						
						//If we processed Null hover last time, then this one is good for hover start
						if(_loopedInputTracker.rend_Previous_HoverHit == null)
						{
							if(_enableDebugging){Debug.LogError("((( queue HoverStart - Touch");}
							_loopedInputTracker.queueInput_HoverStart();
						}
						else
						{
							//Make sure this isn't the hover one we already processed last time
							if(_loopedInputTracker.rend_Previous_HoverHit.GetInstanceID() != _loopedInputTracker.rend_RaycastHit.GetInstanceID())
							{
								// Since this is different than the one we processed last frame, we need to send both a hover end and a hover start
								// !! The hover end must be queued first, because it uses the last frame's current rend; 
								//    which will be overwritten by the hoverstart queue call with the current rend_RaycastHit
								// !! Also, this allows the finalDura_Hover variable of the InputTracker to be updated in case the HoverEnd object wants to know the duration of it's hover
								if(_enableDebugging){Debug.LogError("((( queue HoverEnd - Touch");}
								_loopedInputTracker.queueInput_HoverEnd();
								
								//Then queue up the hover start, resetting both rend_Current_HoverStart and dura_Hover
								if(_enableDebugging){Debug.LogError("((( queue HoverStart2 - Touch");}
								_loopedInputTracker.queueInput_HoverStart();
							}
							else
							{
								//We're still hovering on the same renderer we already sent a message to
								_loopedInputTracker.updateDura_Hover();
							}
						}
						
						// == END OF we Found a Renderer this Update ==

					} 
					else // else of if(_foundRendThisPass){ }
					{
						// == START OF no rend this Update ==
						
						//We did not raycast a renderer this frame; if we have a previous renderer than it needs a HoverStop queue
						if(_loopedInputTracker.rend_Previous_HoverHit != null)
						{
							if(_enableDebugging){Debug.LogError("((( queue HoverEnd2 - Touch");}
							_loopedInputTracker.queueInput_HoverEnd();
						}
						
						// == END OF no rend this Update ==
					} // End of else of if(_foundRendThisPass){ }else{ }

					// ==== CLICK HANDLING ====

					//See if a touch (click) start is happening
					if(_loopedInputTracker.touch_isTouchModeStart)
					{
						if(_foundRendThisPass)
						{
							if(_enableDebugging){Debug.LogError("((( queue ClickStart - Touch");}
							_loopedInputTracker.queueInput_ClickStart();
						}
					}
					
					//See if a touch was clean this frame (this means it needs a touch (click) end)
					if(!_loopedInputTracker.touch_isDirty)
					{
						if(_foundRendThisPass || _loopedInputTracker.rend_Current_ClickHit)
						{
							if(_enableDebugging){Debug.LogError("((( queue ClickEnd - Touch");}
							_loopedInputTracker.queueInput_ClickEnd();
						}

						//Touches that are clean (not dirty / updated) need to be queued for removal at the end of FixedUpdate()
						_queuedRemoval_Inputs.Add(_loopedInputTracker);
					}
					
					if(_loopedInputTracker.rend_Current_ClickHit && _loopedInputTracker.touch_isDirty)
					{
						//If a click is still happening that we already knew about, update it's duration
						_loopedInputTracker.updateDura_Click();
					}

				} // == END OF foreach (internalInputTracker _loopedInputTracker in _ongoing_Inputs)

				//===================================//
				//===================================//
				
				//====== TOUCH BASED INPUT  ====== END
				
				//===================================//
				//===================================//
				#endregion

			} // == END OF else of if(_useMouse){}else{}

		} // == END OF if(LayersForInput != _layerSet_Nothing)

	} // == END OF Update()


	/// <summary>
	/// All tracker class objects are processed during FixedUpdate() if there have been any changes flagged for them since the last FixedUpdate()
	/// (Fixed update is called on game cycles, so this way we do not miss any processing)
	/// </summary>
	void FixedUpdate()
	{	
		//if(_useMouse) //TBD : Remove this?
		//{
			//===================================//
			//===================================//
			
			//====== BOTH BASED INPUT  ====== START
			
			//===================================//
			//===================================//
			
			#region BOTH BASED INPUT - Processing inputs

			foreach(internalInputTracker _loopedInputTracker in _ongoing_Inputs)
			{
			//		Ends need to be processed before Starts, generally (for GO_beingClickHover, and other reasons?)

			// === CLICK END ===
			if(_loopedInputTracker.ClickEnd_NeedsProcessing && _loopedInputTracker.rend_Previous_ClickHit)
			{
				//Process Click Input End
				_loopedInputTracker.ClickEnd_NeedsProcessing = false;
				
				//Assign the GameObject which has been messaged to the external generic inputTracker
				_loopedInputTracker.input_Tracking.GO_beingClicked = (GameObject)(_loopedInputTracker.rend_Previous_ClickHit.gameObject);
				
				//Send the actual message, passing a ref to the inputTracker object
				_loopedInputTracker.rend_Previous_ClickHit.SendMessage("ClickEnd", _loopedInputTracker.input_Tracking, SendMessageOptions.DontRequireReceiver);
				
				//Clear the GameObject which has been messaged to the external generic inputTracker
				//  Now that the clickend has been sent
				_loopedInputTracker.input_Tracking.GO_beingClicked = null;
				
				//Debugging
				if(_enableDebugging){Debug.LogError("ClickEnd sent ==== - === >>> " + _loopedInputTracker.rend_Previous_ClickHit.gameObject.name.ToString());}
				if(_enableDebugging_Parents){Debug.LogError("PPP ClickEnd sent ==== - === >>> " + _loopedInputTracker.rend_Previous_ClickHit.gameObject.transform.parent.gameObject.name.ToString());}
				
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
					_loopedInputTracker.rend_Current_ClickHit.SendMessage("ClickStart", _loopedInputTracker.input_Tracking, SendMessageOptions.DontRequireReceiver);

					//Debugging
				if(_enableDebugging){Debug.LogError("ClickStart sent ==== + === >>> " + _loopedInputTracker.rend_Current_ClickHit.gameObject.name.ToString());}
				if(_enableDebugging_Parents){Debug.LogError("PPP ClickStart sent ==== + === >>> " + _loopedInputTracker.rend_Current_ClickHit.gameObject.transform.parent.gameObject.name.ToString());}
				}

			// === HOVER END ===
				if(_loopedInputTracker.HoverEnd_NeedsProcessing && _loopedInputTracker.rend_Previous_HoverHit)
				{
					//Process Hover Input End
					_loopedInputTracker.HoverEnd_NeedsProcessing = false;
					
					//Assign the GameObject which has been messaged to the external generic inputTracker
					_loopedInputTracker.input_Tracking.GO_beingHovered = (GameObject)(_loopedInputTracker.rend_Previous_HoverHit.gameObject);
					
					//Send the actual message, passing a ref to the inputTracker object
					_loopedInputTracker.rend_Previous_HoverHit.SendMessage("HoverEnd", _loopedInputTracker.input_Tracking, SendMessageOptions.DontRequireReceiver);

					//Clear the GameObject which has been messaged to the external generic inputTracker
					_loopedInputTracker.input_Tracking.GO_beingHovered = null;
					
					//Debugging
				if(_enableDebugging){Debug.LogError("HoverEnd sent ==== - === >>> " + _loopedInputTracker.rend_Previous_HoverHit.gameObject.name.ToString());}
				if(_enableDebugging_Parents){Debug.LogError("PPP HoverEnd sent ==== - === >>> " + _loopedInputTracker.rend_Previous_HoverHit.gameObject.transform.parent.gameObject.name.ToString());}
					
					//Reset after processing a hover end
					_loopedInputTracker.clearHoverData();
				}
				
			// === HOVER START ===
				if(_loopedInputTracker.HoverStart_NeedsProcessing && _loopedInputTracker.rend_Current_HoverHit)
				{
					//Process Hover Input Start
					_loopedInputTracker.HoverStart_NeedsProcessing = false;
					
					//Assign the GameObject which has been messaged to the external generic inputTracker
					_loopedInputTracker.input_Tracking.GO_beingHovered = (GameObject)(_loopedInputTracker.rend_Current_HoverHit.gameObject);

					//Send the actual message, passing a ref to the inputTracker object
					_loopedInputTracker.rend_Current_HoverHit.SendMessage("HoverStart", _loopedInputTracker.input_Tracking, SendMessageOptions.DontRequireReceiver);

					//Debugging
				if(_enableDebugging){Debug.LogError("HoverStart sent ==== + === >>> " + _loopedInputTracker.rend_Current_HoverHit.gameObject.name.ToString());}
				if(_enableDebugging_Parents){Debug.LogError("PPP HoverStart sent ==== + === >>> " + _loopedInputTracker.rend_Current_HoverHit.gameObject.transform.parent.gameObject.name.ToString());}
				}
				

				
				//Shuffle the current hover and click renderers to the previous, no matter what they were or if we processed them this frame, or w/e (shouldn't matter)
				//This is the end of FixedUpdate shift of data to prepare for the next frame
				_loopedInputTracker.refresh_endOfFixedUpdate();
			}

			//===================================//
			//===================================//
			
			//====== BOTH BASED INPUT  ====== END
			
			//===================================//
			//===================================//
			#endregion


		//===================================//
		//===================================//
		
		//====== TOUCH BASED INPUT  ====== START
		
		//===================================//
		//===================================//

		// Removal of any old touch inputs (touches that remained clean and queued for removal thru Update() at this point,
		//		now that all messages have been sent and processing is done
		foreach(internalInputTracker _loopedInputTracker in _queuedRemoval_Inputs)
		{
			_ongoing_Inputs.Remove(_loopedInputTracker);
		}

		//Then clear out the List of queued removals at the end of each update
		_queuedRemoval_Inputs.Clear();
			
		//===================================//
		//===================================//
		
		//====== TOUCH BASED INPUT  ====== END
		
		//===================================//
		//===================================//
		
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

} //End of InputParser : Mono


//===================================//
//===================================//

//======== TRACKING CLASSES  ======== 

//===================================//
//===================================//
#region TRACKING CLASSES

[System.Serializable]
public class internalInputTracker
{
	/// <summary>
	/// This is the 'public' class member of the internal input tracking: this will be used to perform internal calculations and tracking,
	/// and will be passed as a reference to the GameObjects which receive input messages; that they might handle more complex inputs themselves
	/// </summary>
	public inputTracker input_Tracking = new inputTracker(Vector3.zero);

	//===

	#region General touch vars
	/// <summary>
	/// Flag for if this input is tracking a touch on a mobile device or not
	/// </summary>
	public bool isTouchInput = false;
	/// <summary>
	/// The ID for the touch as given by the device : this is used to pair inputTrackers to their touches between frames
	/// </summary>
	public int touchID;
	#endregion

	//===

	#region Renderers which got raycast
	/// <summary>
	/// A renderer which got raycast this frame; this is the preliminary renderer that gets assigned during the camera loop pass
	/// It is then passed to one of: rend_Current_HoverHit or rend_Current_ClickHit depending on if it should be
	/// It is likely that it will always be passed to rend_Current_HoverHit, but I am separating it for clarity here.
	/// </summary>
	public Renderer rend_RaycastHit;
	/// <summary>
	/// A renderer which got raycast this frame, passed from rend_CurrentRaycastHit in most (all?) cases during Update()
	/// </summary>
	public Renderer rend_Current_HoverHit;
	/// <summary>
	/// A renderer which got raycast last frame, passed from rend_Current_HoverHit at the end of FixedUpdate()
	/// </summary>
	public Renderer rend_Previous_HoverHit;
	/// <summary>
	/// A renderer which got raycast this frame, passed from rend_CurrentRaycastHit during Update() when we need to send a ClickStart message
	/// </summary>
	public Renderer rend_Current_ClickHit;
	/// <summary>
	/// A renderer which got raycast last frame, passed from rend_Current_ClickHit at the end of FixedUpdate()
	/// </summary>
	public Renderer rend_Previous_ClickHit;
	#endregion

	//===

	#region Touch based Flags
	/// <summary>
	/// Touches that are dirty at the end of each update do not get removed / parsed as touch end; because they actively changed that frame
	/// </summary>
	public bool touch_isDirty = false;
	/// <summary>
	/// Used to flag touches that just began, so they can be parsed for Click starts.
	/// </summary>
	public bool touch_isTouchModeStart = false;
	#endregion

	//===

	#region Mouse based Flags
	/// <summary>
	/// Used to flag when the mouse button has been held down from the last frame or not, to help determine click end messages more accurately
	/// </summary>
	public bool mouse_isButtonDown = false;
	#endregion
	
	//===

	#region Flags for sending messages
	/// <summary>
	/// Used to flag a HoverStart SendMessage(ref inputTracker) ; will be processed and unflagged during FixedUpdate()
	/// </summary>
	public bool HoverStart_NeedsProcessing = false;
	/// <summary>
	/// Used to flag a HoverEnd SendMessage(ref inputTracker) ; will be processed and unflagged during FixedUpdate()
	/// </summary>
	public bool HoverEnd_NeedsProcessing = false;
	/// <summary>
	/// Used to flag a ClickStart SendMessage(ref inputTracker) ; will be processed and unflagged during FixedUpdate()
	/// </summary>
	public bool ClickStart_NeedsProcessing = false;
	/// <summary>
	/// Used to flag a ClickEnd SendMessage(ref inputTracker) ; will be processed and unflagged during FixedUpdate()
	/// </summary>
	public bool ClickEnd_NeedsProcessing = false;
	#endregion

	//===

	#region Constructors
	/// <summary>
	/// Declare a new mouse-based internal inputTracker
	/// </summary>
	public internalInputTracker(Vector3 _startPos)
	{
		touchID = -1;
		isTouchInput = false;
		
		input_Tracking = new inputTracker(_startPos);
		
		rend_Previous_HoverHit = null;
		rend_Current_HoverHit = null;
		rend_Previous_ClickHit = null;
		rend_Current_ClickHit = null;
		
		touch_isDirty = false; //This is not a touch anyway
		touch_isTouchModeStart = false;
		
		HoverStart_NeedsProcessing = false;
		HoverEnd_NeedsProcessing = false;
		ClickStart_NeedsProcessing = false;
		ClickEnd_NeedsProcessing = false;
	}

	/// <summary>
	/// Declare a new touch-based internal inputTracker
	/// </summary>
	public internalInputTracker(int _ID, Vector3 _startPos)
	{
		touchID = _ID;
		isTouchInput = true;
		
		input_Tracking = new inputTracker(_startPos);
		
		rend_Previous_HoverHit = null;
		rend_Current_HoverHit = null;
		rend_Previous_ClickHit = null;
		rend_Current_ClickHit = null;
		
		touch_isDirty = true; //New touches are always dirty
		touch_isTouchModeStart = true; //New touches are always touch mode start

		//UNLESS THE LEVEL JUST LOADED
		if(Time.timeSinceLevelLoad < 0.1f)
		{
			touch_isTouchModeStart = false;
		}
		
		HoverStart_NeedsProcessing = false;
		HoverEnd_NeedsProcessing = false;
		ClickStart_NeedsProcessing = false;
		ClickEnd_NeedsProcessing = false;
	}

	#endregion

	// ===

	#region Touch functions for Clean/Dirty
	public void Clean()
	{
		touch_isDirty = false; //Flag for removal unless it gets updated
		//Clean touches cannot be touch_mode_began, by definition; they have been in the List for at least 1 cycle
		touch_isTouchModeStart = false;
	}
	
	public void MakeDirty()
	{
		touch_isDirty = true;	//Flag as dirty, meaning it has been updated this frame and should not be parsed as touch end / removed
	}
	#endregion

	// ===

	#region Updating tracking times (subtraction)
	/// <summary>
	/// Updates the duration of the hover based on the current run time and the previously assigned start time
	/// </summary>
	public void updateDura_Hover()
	{
		input_Tracking.dura_Hover = Time.time - input_Tracking.time_HoverStart;
	}
	/// <summary>
	/// Updates the duration of the click based on the current run time and the previously assigned start time
	/// </summary>
	public void updateDura_Click()
	{
		input_Tracking.dura_Click = Time.time - input_Tracking.time_ClickStart;
	}

	#endregion

	// ===

	#region Methods to fix up internal/external tracking data
	/// <summary>
	/// Clears out some of the click processing vars when we send a click end (they are no longer needed)
	/// </summary>
	public void clearClickData()
	{
		rend_Previous_ClickHit = null;
		rend_Current_ClickHit = null;
		input_Tracking.dura_Click = 0f;
	}

	/// <summary>
	/// Clears out some of the hover processing vars when we send a hover end (they are no longer needed)
	/// </summary>
	public void clearHoverData()
	{
		input_Tracking.dura_Hover = 0f;
	}

	/// <summary>
	/// Passes the current rends to previous rends, updates previous position for use during next frame
	/// </summary>
	public void refresh_endOfFixedUpdate()
	{
		rend_Previous_ClickHit = rend_Current_ClickHit;
		rend_Previous_HoverHit = rend_Current_HoverHit;

		input_Tracking.pos_Previous = input_Tracking.pos_Current;
		input_Tracking.v3_prev_RaycastHitPoint = input_Tracking.v3_cur_RaycastHitPoint;
	}

	/// <summary>
	/// Passes the rend we just raycast to our current HOVER renderer (not Click; as that would fuck it up), at the begining of Update just after raycasting; before determining message flags
	/// </summary>
	public void refresh_afterRaycastDuringUpdate()
	{
		rend_Current_HoverHit = rend_RaycastHit;
	}

	#endregion
	
	// ===

	#region Queue inputs methods (used to pass rend_RaycastHit to one/some of the message rend vars (rend_Current/Prev_*)

	/// <summary>
	/// Assigns the currently hit renderer to the messaging rend, flags the message to be sent during FixedUpdate(),
	/// and fixes the vars (position, dura) of our generic (external) inputTracker object
	/// </summary>
	public void queueInput_HoverStart()
	{
		rend_Current_HoverHit = rend_RaycastHit;
		HoverStart_NeedsProcessing = true;

		input_Tracking.pos_HoverStart = input_Tracking.pos_Current;
		input_Tracking.time_HoverStart = Time.time;
		input_Tracking.dura_Hover = 0f;
	}

	/// <summary>
	/// Assigns the last current renderer (from last frame) to the messaging rend, flags the message to be sent during FixedUpdate(),
	/// and fixes the vars (position, final duration) of our generic (external) inputTracker object
	/// </summary>
	public void queueInput_HoverEnd()
	{
		HoverEnd_NeedsProcessing = true;

		updateDura_Hover();
		input_Tracking.finalDura_Hover = input_Tracking.dura_Hover;
	}

	/// <summary>
	/// Assigns the currently hit renderer to the messaging rend, flags the message to be sent during FixedUpdate(),
	/// and fixes the vars (position, dura) of our generic (external) inputTracker object
	/// </summary>
	public void queueInput_ClickStart()
	{
		rend_Current_ClickHit = rend_RaycastHit;
		ClickStart_NeedsProcessing = true;
		
		input_Tracking.pos_ClickStart = input_Tracking.pos_Current;
		input_Tracking.time_ClickStart = Time.time;
		input_Tracking.dura_Click = 0f;
	}

	/// <summary>
	/// Assigns the last current renderer (from last frame) to the messaging rend, flags the message to be sent during FixedUpdate(),
	/// and fixes the vars (position, final duration) of our generic (external) inputTracker object
	/// </summary>
	public void queueInput_ClickEnd()
	{
		//If the click started on a renderer, the End message will be sent to that renderer
		if(rend_Current_ClickHit)
		{
			rend_Previous_ClickHit = rend_Current_ClickHit;
		}
		else
		{
			//Otherwise, the End message will be sent to the rend we raycast this frame instead (the click started on no rends early)
			rend_Previous_ClickHit = rend_RaycastHit;
		}

		ClickEnd_NeedsProcessing = true;
		
		updateDura_Click();
		input_Tracking.finalDura_Click = input_Tracking.dura_Click;

		//Afer we've queued up the click end, we can clear out the rend which was used for the ClickStart earlier (assuming it's not queued currently)
		if(!ClickStart_NeedsProcessing)
		{
			rend_Current_ClickHit = null;
		}
	}

	#endregion
}


[System.Serializable]
public class inputTracker
{
	private float _time_ClickStart;
	/// <summary>
	/// Gets or sets the game time that a click began
	/// </summary>
	/// <value>The time of the click start.</value>
	public float time_ClickStart { get{ return _time_ClickStart; } set{ _time_ClickStart = value; } }
	private	float _time_HoverStart;
	/// <summary>
	/// Gets or sets the game time that a hover began
	/// </summary>
	/// <value>The time of the hover start.</value>
	public float time_HoverStart { get{ return _time_HoverStart; } set{ _time_HoverStart = value; } }

	//===

	private float _dura_Click;
	/// <summary>
	/// Gets or sets the duration of the current click
	/// </summary>
	/// <value>The duration in seconds.</value>
	public float dura_Click { get{ return _dura_Click; } set{ _dura_Click = value; } }
	private	float _dura_Hover;
	/// <summary>
	/// Gets or sets the duration of the current hover
	/// </summary>
	/// <value>The duration in seconds.</value>
	public float dura_Hover { get{ return _dura_Hover; } set{ _dura_Hover = value; } }

	//===

	private	float _finalDura_Click;
	/// <summary>
	/// Gets or sets the final duration of the click, which can be used by anything which receives a ClickEnd message
	/// </summary>
	/// <value>The duration in seconds.</value>
	public float finalDura_Click { get{ return _finalDura_Click; } set{ _finalDura_Click = value; } }
	private	float _finalDura_Hover;
	/// <summary>
	/// Gets or sets the final duration of the hover, which can be used by anything which receives a ClickEnd message
	/// </summary>
	/// <value>The duration in seconds.</value>
	public float finalDura_Hover { get{ return _finalDura_Hover; } set{ _finalDura_Hover = value; } }
	
	//===
	
	private	Vector3 _pos_ClickStart;
	/// <summary>
	/// The screen position of the input when we sent the most recent ClickStart message
	/// </summary>
	/// <value>The screen position in Vector3 (z == 0f)</value>
	public Vector3 pos_ClickStart { get{ return _pos_ClickStart; } set{ _pos_ClickStart = value; } }
	private	Vector3 _pos_HoverStart;
	/// <summary>
	/// The screen position of the input when we sent the most recent HoverStart message
	/// </summary>
	/// <value>The screen position in Vector3 (z == 0f)</value>
	public Vector3 pos_HoverStart { get{ return _pos_HoverStart; } set{ _pos_HoverStart = value; } }
	private Vector3 _pos_Current;
	/// <summary>
	/// The current screen position of the input
	/// This will be refreshed during Update(), well before messages are sent in FixedUpdate()
	/// </summary>
	/// <value>The screen position in Vector3 (z == 0f)</value>
	public Vector3 pos_Current { get{ return _pos_Current; } set{ _pos_Current = value; } }
	private	Vector3 _pos_Previous;
	/// <summary>
	/// The screen position of the input during the last frame, can be used to calculate delta positions
	/// This will be refreshed at the end of each FixedUpdate() after messages are sent, so that it can be used during next frame
	/// </summary>
	/// <value>The screen position in Vector3 (z == 0f)</value>
	public Vector3 pos_Previous { get{ return _pos_Previous; } set{ _pos_Previous = value; } }

	//===

	private	GameObject _GO_beingClicked;
	/// <summary>
	/// Gets or sets a game object that is currently being clicked, in case it needs to be externally referenced
	/// This will be cleared when nothing is being clicked / touched
	/// </summary>
	/// <value>A GameObject which has been send a ClickStart message as of last.</value>
	public GameObject GO_beingClicked { get{ return _GO_beingClicked; } set{ _GO_beingClicked = value; } }
	private	GameObject _GO_beingHovered;
	/// <summary>
	/// Gets or sets a game object that is currently being hovered, in case it needs to be externally referenced
	/// This will be cleared when nothing is being hovered
	/// </summary>
	/// <value>A GameObject which has been send a HoverStart message as of last.</value>
	public GameObject GO_beingHovered { get{ return _GO_beingHovered; } set{ _GO_beingHovered = value; } }
	
	//===
	
	private	Vector3 _v3_cur_RaycastHitPoint;
	/// <summary>
	/// Gets or sets a world position of the current RaycastHit.point
	/// </summary>
	/// <value>A GameObject which has been send a ClickStart message as of last.</value>
	public Vector3 v3_cur_RaycastHitPoint { get{ return _v3_cur_RaycastHitPoint; } set{ _v3_cur_RaycastHitPoint = value; } }
	private	Vector3 _v3_prev_RaycastHitPoint;
	/// <summary>
	/// Gets or sets a game object that is currently being hovered, in case it needs to be externally referenced
	/// This will be cleared when nothing is being hovered
	/// </summary>
	/// <value>A GameObject which has been send a HoverStart message as of last.</value>
	public Vector3 v3_prev_RaycastHitPoint { get{ return _v3_prev_RaycastHitPoint; } set{ _v3_prev_RaycastHitPoint = value; } }
	
	//===

	/// <summary>
	/// Declare a new generic input tracker object, with a given start position
	/// This object is used to pass input tracking information (positions, durations, etc) externally from InputParser, by ref
	/// </summary>
	public inputTracker(Vector3 pos_Cur)
	{
		//Initialize everything to 0 !

		_time_ClickStart = 0f;
		_time_HoverStart = 0f;
		
		_dura_Click = 0f;
		_dura_Hover = 0f;
		
		_pos_ClickStart = Vector3.zero;
		_pos_HoverStart = Vector3.zero;
		_pos_Current = pos_Cur;
		_pos_Previous = pos_Cur;

		_v3_cur_RaycastHitPoint = Vector3.zero;
		_v3_prev_RaycastHitPoint = Vector3.zero;
	}
}
#endregion