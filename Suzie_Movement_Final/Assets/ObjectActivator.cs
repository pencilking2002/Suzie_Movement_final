using UnityEngine;
using System.Collections;

public class ObjectActivator : MonoBehaviour {

	public ObjectActivator Instance;

	// Use this for initialization
	void Awake () 
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(this);
	}
	
	public static void Register ()
	{

	} 

//	private void OnEnable()
//	{
//		EventManager.onInputEvent += Activate;
//		EventManager.onCharEvent += Activate;
//		EventManager.onDetectEvent += Activate;
//
//		EventManager.onInputEvent += Deactivate;
//		EventManager.onCharEvent += Deactivate;
//		EventManager.onDetectEvent += Deactivate;
//	}
//
//	private void OnDisable()
//	{
//		EventManager.onInputEvent -= Activate;
//		EventManager.onCharEvent -= Activate;
//		EventManager.onDetectEvent -= Activate;
//
//		EventManager.onInputEvent -= Deactivate;
//		EventManager.onCharEvent -= Deactivate;
//		EventManager.onDetectEvent -= Deactivate;
//	}
//
//	private void Activate (GameEvent gEvent)
//	{
//		switch(gEvent)
//		{
//			case GameEvent.Land:
//
//			break;
//		}	
//	}


	private void Deactivate (GameEvent gEvent)
	{
		
	}

}
