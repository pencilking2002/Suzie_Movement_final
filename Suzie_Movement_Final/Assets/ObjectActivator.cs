using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
	This class is responsible for fien grain control of component activation
	and deactivation. Each component that wants to hook in into the ObjectActivator 
	registers itself using the RegisterDeactivation/RegisterActivation methods and supplies
	which events it would like to be hooked to

*/

public class ObjectActivator : MonoBehaviour {

	public static ObjectActivator Instance;

	private List<ActivationQueue> componentList = new List<ActivationQueue>();

	//ActivationQueue activationQueue;

	// Use this for initialization
	void Awake () 
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(this);
	}

	/// <summary>
	/// This method is used by other scripts in ordet to register themselves with
	/// with the ObjectAcivator class so that it can activate/deacivate scripts at the specified events
	/// </summary>
	/// <param name="script">Script.</param>
	/// <param name="events">Events.</param>
	public void Register (MonoBehaviour script, GameEvent[] events, bool activate)
	{
		componentList.Add(new ActivationQueue(script, events, activate));
	} 

	private void OnEnable()
	{
		EventManager.onCharEvent += onChar;
		//EventManager.onInputEvent += onInput;
		//EventManager.onDetectEvent += onDetect;
	}

	private void OnDisable()
	{
		EventManager.onCharEvent -= onChar;
		//EventManager.onInputEvent -= onInput;
		//EventManager.onDetectEvent -= onDetect;
	}

	private void onChar (GameEvent gEvent)
	{
		ActivationHandler(gEvent);
	}

	private void onInput (GameEvent gEvent)
	{
		ActivationHandler(gEvent);
	}

//	private void onDetect (GameEvent gEvent)
//	{
//		ActivationHandler(gEvent);
//	}

	/// <summary>
	/// Activate or deactivate scripts 
	/// </summary>
	/// <param name="e">E.</param>
	private void ActivationHandler (GameEvent e)
	{
		for (int i = 0; i < componentList.Count; i++)
		{
			if (componentList[i].activate && ArrayContains(componentList[i].gEvents, e))
			{
				print(componentList[i].script.GetType() + " Activated");
				componentList[i].script.enabled = true;
			}
			else
			{
				print(componentList[i].script.GetType() + " Deactivated");
				componentList[i].script.enabled = false;
			}
		}
	}

	private bool ArrayContains(GameEvent[] gEvents, GameEvent e)
	{
		for(int i = 0; i < gEvents.Length; i++)
		{
			if(gEvents[i] == e)
				return true;
		}

		return false;
	}

}

/// <summary>
/// Custom class to use for each component's activation/deactivation
/// </summary>
public class ActivationQueue
{
	public MonoBehaviour script;
	public GameEvent[] gEvents;
	public bool activate;

	public ActivationQueue(MonoBehaviour _script, GameEvent[] _gEvents, bool _activate)
	{
		script = _script;
		gEvents = _gEvents;
		activate = _activate;
	}
}
