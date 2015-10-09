using UnityEngine;
using System.Collections;
using InControl;

public class InputController : MonoBehaviour {
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------
	
	public static InputController Instance;
	
	public static float v, h, rawH, rawV, orbitH, orbitV;

	
	// Input Events -------------------------------------------------------------
	public delegate void InputAction(InputEvent inputEvent);
	public static InputAction onInput;
	
	//public delegate void ControllerDebugAction(ControllerDebugger.Axis axisEvent, ControllerDebugger.State state);
	//public static ControllerDebugAction onControllerDebug;
	
	// Enum to use for chcking input events
	public enum InputEvent										
	{
		Jump,
		RecenterCam,
		CamBehind,
		OrbitCamera,
		faceOppositeDirection,
		StopRunning,
		StartRunning,
		StopTurnRunning,
		StartTurnRunning
	}
	
	//[HideInInspector]
	//public float jumpKeyHoldDuration = 0.0f;
	
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Priate Variables
	//---------------------------------------------------------------------------------------------------------------------------
	private InputDevice inputDevice;
	public static bool jumpIsPressed = false;
	public static bool jumpReleased = false;

	private void Awake ()
	{
		if (Instance == null)
			Instance = this;
		//inputDevice = InputManager.ActiveDevice;
	}
	
	private void Update ()
	{
		inputDevice = InputManager.ActiveDevice;
		
		h = Input.GetAxis ("Horizontal");
		v = Input.GetAxis ("Vertical");
		
		rawH = Input.GetAxisRaw("Horizontal");
		rawV = Input.GetAxisRaw("Vertical");
		

		orbitH = inputDevice.RightStickX;
		orbitV = inputDevice.RightStickY;

		//----------------------------------------------------------------------------------------------------------------------
		// Player Movement
		//----------------------------------------------------------------------------------------------------------------------

		if (onInput == null)
			return;

		// Start running
		if (inputDevice.LeftStickY.WasPressed) 
		{
			onInput (InputEvent.StartRunning);
		}

		// Stop running
		if (inputDevice.LeftStickY.WasReleased) 
		{
			onInput (InputEvent.StopRunning);
			//print ("LeftStickY was released");
		}

		//----------------------------------------------------------------------------------------------------------------------
		// Jumping
		//----------------------------------------------------------------------------------------------------------------------

		// Time how long
//		if (inputDevice.Action4.IsPressed)
//		{
//			jumpKeyHoldDuration += Time.deltaTime;
//		}
		
		// if pressed Y or pressed Space
		if (inputDevice.Action4.WasPressed)
		{
			onInput(InputEvent.Jump);
			jumpIsPressed = true;	
		}

		if (inputDevice.Action4.WasReleased)
		{
			jumpIsPressed = false;
			jumpReleased = true;
		}

		//----------------------------------------------------------------------------------------------------------------------
		// Recenter Camera
		//----------------------------------------------------------------------------------------------------------------------
		
		if (inputDevice.RightBumper.WasReleased)
		{
			if (onInput != null)
				onInput (InputEvent.RecenterCam);	
		}
		
		//----------------------------------------------------------------------------------------------------------------------
		// Camera Orbiting
		//----------------------------------------------------------------------------------------------------------------------
		
		if (inputDevice.RightStickX.IsPressed)
		{
			if (onInput != null)
				onInput (InputEvent.OrbitCamera);
		}
		

		if (inputDevice.RightStickX.WasReleased) 
		{
			if (onInput != null)
				onInput (InputEvent.CamBehind);
		}


	}
		
	
}
