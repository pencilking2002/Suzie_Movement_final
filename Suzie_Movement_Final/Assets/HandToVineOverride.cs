using UnityEngine;
using System.Collections;

public class HandToVineOverride : StateMachineBehaviour {

//	private Transform leftHand, rightHand;					
//	private Vector3 leftHandPos, rightHandPos; 				// For updating the hand positions
//	private VineClimbController2 vClimbController;
//
//	private int anim_attachToVine = Animator.StringToHash("LeftHandAttachToVine");

	//  OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{
//		animator.updateMode = AnimatorUpdateMode.Normal;
//
//		if (vClimbController == null)
//			vClimbController = animator.GetComponent<VineClimbController2>();
//
//		if (leftHand == null)
//			leftHand = GameObject.FindGameObjectWithTag("LeftHand").transform;
//
//		if (rightHand == null)
//			rightHand = GameObject.FindGameObjectWithTag("RightHand").transform;
//		//animator.updateMode = AnimatorUpdateMode.Normal;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	//override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
	{

//		if (InputController.v != 0 && vClimbController != null)
//		{
//			float weight = animator.GetFloat(anim_attachToVine);
//
//			Debug.Log(animator.GetFloat(anim_attachToVine));
//				
//			//Debug.Log("Override");
//			leftHandPos = new Vector3(vClimbController.vinePos.x, leftHand.position.y, vClimbController.vinePos.z);
//			//rightHandPos = new Vector3(vClimbController.vineAttachPoint.position.x, rightHand.position.y, vClimbController.vineAttachPoint.position.z);
//
//			animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandPos);
//			animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, weight);
//
////			animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandPos);
////			animator.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
//
//			Debug.DrawLine(leftHand.position, leftHandPos, Color.red);
//		}
	}
}
