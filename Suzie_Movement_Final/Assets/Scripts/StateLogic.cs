﻿using UnityEngine;
using System.Collections;

// Apply any extra logic when entering a state
public class StateLogic : StateMachineBehaviour {

	public enum AnimState
	{
		Enter,
		Exit
	}
	RomanCharController charController = null;
	
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
		if(charController == null)
			charController = animator.GetComponent<RomanCharController>();
		
		//charController.RunStateLogic(AnimState.Enter);
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if(charController == null)
			charController = animator.GetComponent<RomanCharController>();
		
		//charController.RunStateLogic(AnimState.Enter);
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
