using UnityEngine;
using System.Collections;

public enum GameEvent
{
	// Camera/follow object
	AttachFollow,
	DetachFollow,
	
	//Input
	Jump,
	RecenterCam,
	CamBehind,
	OrbitCamera,
	faceOppositeDirection,
	StopRunning,
	StartRunning,
	StopTurnRunning,
	StartTurnRunning,
	StartSprinting,
	StopSprinting
}


public class EventManager : MonoBehaviour 
{
	//---------------------------------------------------------------------------------------------------------------------------
	// Character events 
	//---------------------------------------------------------------------------------------------------------------------------
	public delegate void CharEvent(GameEvent gameEvent);
	public static CharEvent onCharEvent;
	
	//---------------------------------------------------------------------------------------------------------------------------
	// Input events 
	//---------------------------------------------------------------------------------------------------------------------------
	public delegate void InputAction(GameEvent gameEvent);
	public static InputAction onInputEvent;
	

	public static void OnCharEvent (GameEvent gameEvent)
	{
		if (onCharEvent != null)
			onCharEvent(gameEvent);
	}
	
	public static void OnInputEvent (GameEvent gameEvent)
	{
		if (onInputEvent != null)
			onInputEvent(gameEvent);
	}
}
