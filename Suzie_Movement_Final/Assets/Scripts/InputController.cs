using UnityEngine;
using System.Collections;
using InControl;

public class InputController : MonoBehaviour {
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Public Variables
	//---------------------------------------------------------------------------------------------------------------------------
	
	public static InputController Instance;
	
	public static float v, h, rawH, rawV, orbitH, orbitV;
	
	//public delegate void ControllerDebugAction(ControllerDebugger.Axis axisEvent, ControllerDebugger.State state);
	//public static ControllerDebugAction onControllerDebug;
	
	
	
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

		if (EventManager.onInputEvent == null)
			return;

		// Start running
		if (inputDevice.LeftStickY.WasPressed) 
		{
			EventManager.OnInputEvent(GameEvent.StartRunning);
		}

		// Stop running
		if (inputDevice.LeftStickY.WasReleased) 
		{
			EventManager.OnInputEvent (GameEvent.StopRunning);
			//print ("LeftStickY was released");
		}

		//----------------------------------------------------------------------------------------------------------------------
		// Jumping
		//----------------------------------------------------------------------------------------------------------------------
		
		// if pressed Y or pressed Space
		if (inputDevice.Action1.WasPressed)
		{
			EventManager.OnInputEvent(GameEvent.Jump);
			jumpIsPressed = true;	
		}

		if (inputDevice.Action1.WasReleased)
		{
			jumpIsPressed = false;
			jumpReleased = true;
		}

		//----------------------------------------------------------------------------------------------------------------------
		// Recenter Camera
		//----------------------------------------------------------------------------------------------------------------------
		
		if (inputDevice.RightBumper.WasReleased)
		{
			EventManager.OnInputEvent(GameEvent.RecenterCam);	
		}
		
		//----------------------------------------------------------------------------------------------------------------------
		// Camera Orbiting
		//----------------------------------------------------------------------------------------------------------------------
		
		if (inputDevice.RightStickX.IsPressed)
		{
			EventManager.OnInputEvent(GameEvent.OrbitCamera);
		}
		
		if (inputDevice.RightStickX.WasReleased) 
		{
			EventManager.OnInputEvent(GameEvent.CamBehind);
		}


	}
		
	
}
