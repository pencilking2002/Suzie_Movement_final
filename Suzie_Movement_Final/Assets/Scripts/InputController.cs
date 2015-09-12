using UnityEngine;
using System.Collections;
using InControl;

public class InputController : MonoBehaviour {
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------
	
	public static InputController Instance;
	
	public static float h, v, orbitH, orbitV;

	
	// Input Events -------------------------------------------------------------
	public delegate void InputAction(InputEvent inputEvent);
	public static InputAction onInput;
	
	// Enum to use for chcking input events
	public enum InputEvent										
	{
		JumpUp,
		RecenterCam,
		CamBehind,
		OrbitCamera,
		faceOppositeDirection,
		StopRunning,
		StartRunning,
		StopTurnRunning
	}
	
	[HideInInspector]
	public float jumpKeyHoldDuration = 0.0f;
	
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Priate Variables
	//---------------------------------------------------------------------------------------------------------------------------
	private InputDevice inputDevice;

	private void Awake ()
	{
		if (Instance == null)
			Instance = this;
		//inputDevice = InputManager.ActiveDevice;
	}
	
	private void Update ()
	{
		inputDevice = InputManager.ActiveDevice;
		
		h = inputDevice.LeftStickX.Value;
		v = inputDevice.LeftStickY.Value;
		orbitH = inputDevice.RightStickX;
		orbitV = inputDevice.RightStickY;

		//----------------------------------------------------------------------------------------------------------------------
		// Player Movement
		//----------------------------------------------------------------------------------------------------------------------

		// Start running
		if (inputDevice.LeftStickY.WasPressed) 
		{
			if (onInput != null)
				onInput (InputEvent.StartRunning);
		}

		// Stop running
		if (inputDevice.LeftStickY.WasReleased) 
		{
			if (onInput != null)
			{
				onInput (InputEvent.StopRunning);
				print ("LeftStickY was released");
			}
		}
		
		if (inputDevice.LeftStickX.WasReleased)
		{
			if (onInput != null)
			{
				onInput (InputEvent.StopTurnRunning);
			}
		}

		// Run Right
//		if (InputDevice.LeftStickX.WasPressed)
//		{
//			if (onInput != InputEvent.)
//		}

		//----------------------------------------------------------------------------------------------------------------------
		// Jumping
		//----------------------------------------------------------------------------------------------------------------------
		
		if (inputDevice.Action4.IsPressed)
			jumpKeyHoldDuration += Time.deltaTime;
		
		
		// if pressed Y or pressed Space
		if (inputDevice.Action4.WasReleased)
		{
			if (onInput != null)
				onInput(InputEvent.JumpUp);
			
			// Reset the jump key timer
			jumpKeyHoldDuration = 0.0f;	
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
