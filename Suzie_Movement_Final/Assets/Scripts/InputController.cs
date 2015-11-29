using UnityEngine;
using System.Collections;
using InControl;

public class InputController : MonoBehaviour {
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------
	
	public static InputController Instance;
	public static float v, h, orbitV, orbitH, rawV, rawH;

	// Fix values coming in from InControl. 
	// For whatever reason the values aren't raw when using the controller
//	public static float rawV
//	{
//		get { return _rawV; }
//		set 
//		{
//			if (value < 0)
//				_rawV = -1;
//
//			else if (value > 0)
//			    _rawV = 1;
//
//			else
//				_rawV = 0;
//
//		}
//	}
//
//	public static float rawH
//	{
//		get { return _rawH; }
//		set 
//		{
//			if (value < 0)
//				_rawH = -1;
//			
//			else if (value > 0)
//				_rawH = 1;
//
//			else
//				_rawH = 0;
//		}
//	}

	public static bool jumpReleased = false;
	private RomanCharState charState;
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Priate Variables
	//---------------------------------------------------------------------------------------------------------------------------
	
	private InputDevice inputDevice;
	private bool canSprint = false;
	private static float _rawV, _rawH;

	private void Awake ()
	{
		if (Instance == null)
			Instance = this;
		//inputDevice = InputManager.ActiveDevice;
		charState = GameObject.FindObjectOfType<RomanCharState>();
	}
	
	private void Update ()
	{
		inputDevice = InputManager.ActiveDevice;
		
		h = Input.GetAxis ("Horizontal");
		v = Input.GetAxis ("Vertical");
//		h = inputDevice.LeftStickX.Value;
//		v = inputDevice.LeftStickY.Value;

		rawH = inputDevice.LeftStickX.RawValue;
		rawV = inputDevice.LeftStickY.RawValue;
		
		orbitH = inputDevice.RightStickX;
		orbitV = inputDevice.RightStickY;

		//----------------------------------------------------------------------------------------------------------------------
		// Player Movement
		//----------------------------------------------------------------------------------------------------------------------

		// Sprinting ------------------------------------------------
		if (inputDevice.LeftTrigger.WasPressed)
			EventManager.OnInputEvent(GameEvent.SprintModifierDown);
		
		else if (inputDevice.LeftTrigger.WasReleased)
			EventManager.OnInputEvent(GameEvent.SprintModifierUp);
	

		if (charState.IsEdgeClimbing() && inputDevice.LeftStickY.WasPressed)
		{
			print ("edge climbing raw v: " + v);
			
			if (rawV == 1 || v > 0)
			{
				EventManager.OnInputEvent(GameEvent.ClimbOverEdge);
			}
			// TODO - doesnt work with controller
			else if (rawV == -1 || v < 0)
			{
	//			print ("InputController: stop climbing " + rawV);
				EventManager.OnInputEvent(GameEvent.StopClimbing);

			}
//			print ("Event sent: climboveredge");
		}

		//----------------------------------------------------------------------------------------------------------------------
		// Jumping
		//----------------------------------------------------------------------------------------------------------------------
		
		// if pressed Y or pressed Space
		if (inputDevice.Action1.WasPressed)
			EventManager.OnInputEvent(GameEvent.Jump);
		
		else if (inputDevice.Action1.WasReleased)
			jumpReleased = true;


		//----------------------------------------------------------------------------------------------------------------------
		// Recenter Camera
		//----------------------------------------------------------------------------------------------------------------------
		
		if (inputDevice.RightBumper.WasReleased)
			EventManager.OnInputEvent(GameEvent.RecenterCam);	
		
		
		//----------------------------------------------------------------------------------------------------------------------
		// Camera Orbiting
		//----------------------------------------------------------------------------------------------------------------------
		
//		if (inputDevice.RightStickX.IsPressed)
//			EventManager.OnInputEvent(GameEvent.OrbitCamera);
//		
//		else if (inputDevice.RightStickX.WasReleased) 
//			EventManager.OnInputEvent(GameEvent.CamBehind);
		


	}
		
	
}
