using UnityEngine;
using System.Collections;
using UnityStandardAssets.ImageEffects;

//[RequireComponent(typeof(UnityStandardAssets.ImageEffects.Fisheye))]
public class SprintingEffects : MonoBehaviour {

	public float strength = 0.4f;

	private bool wideAngle = false;
	private Fisheye fisheyeEffect;


	private void Start () 
	{
		//fisheyeEffect = GetComponent<Fisheye>();
	}
	
	private void OnEnable ()
	{
		EventManager.onCharEvent += DoFisheye;
	}
	
	private void OnDisable ()
	{
		EventManager.onCharEvent -= DoFisheye;
	}
	
	private void DoFisheye (GameEvent e)
	{
		if (e == GameEvent.StartSprinting)
		{	
//			fisheyeEffect.enabled = true;	
//			LeanTween.value(gameObject, 0, strength, 0.5f)
//
//				.setOnUpdate((float val) => {
//					fisheyeEffect.strengthX = val;
//					fisheyeEffect.strengthY = val;
//				});
				
		}
		else if (e == GameEvent.StopSprinting)
		{
//			LeanTween.value(gameObject, fisheyeEffect.strengthX, 0, 1f)
//				.setOnUpdate((float val) => {
//					fisheyeEffect.strengthX = val;
//					fisheyeEffect.strengthY = val;
//				})
//
//				.setOnComplete(() => {
//					fisheyeEffect.enabled = false;	
//				});
		}
	}

}
