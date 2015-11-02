using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnityStandardAssets.ImageEffects.Fisheye))]
public class SprintingEffects : MonoBehaviour {
	
	private bool wideAngle = false;
	public UnityStandardAssets.ImageEffects.Fisheye fisheyeEffect;
	
	private void Start () 
	{
	}
	
	private void OnEnable ()
	{
		EventManager.onInputEvent += DoFisheye;
	}
	
	private void OnDisable ()
	{
		EventManager.onInputEvent -= DoFisheye;
		
	}
	
	private void DoFisheye (GameEvent e)
	{
		if (e == GameEvent.StartSprinting)
		{
			//fisheye.strengthX = 0.4f;
			//fisheye.strengthY = 0.4f;
			//wideAngle = true;
			//fisheyeEffect.enabled = true;
		}
		else if (e == GameEvent.StopSprinting)
		{
			//fisheye.strengthX = 0;
			//fisheye.strengthY = 0;
			//wideAngle = false;
		}
	}

}
