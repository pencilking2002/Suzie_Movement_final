using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ControllerDebugger : MonoBehaviour {
	public Image Up, Right, Down, Left;

	private Image image;
	
	public enum Axis
	{
		UP,
		RIGHT,
		DOWN,
		LEFT
	}
	//public Axis axis;
	
	public enum State 
	{
		Pressed,
		Released
	}
	
	private Vector4 tempColor = Color.white;
	
	private void Awake ()
	{
		image = GetComponent<Image>();
	}
	
	private void Update ()
	{


		//print (InputController.rawH);
		Up.color = new Vector4(InputController.v, InputController.v, InputController.v, 1);
		Down.color = new Vector4(-InputController.v, -InputController.v, -InputController.v, 1);



		Right.color = new Vector4(InputController.h, InputController.h, InputController.h, 1);
		Left.color = new Vector4(-InputController.h, -InputController.h, -InputController.h, 1);


	}
	
//	private void OnEnable()
//	{
//		InputController.onControllerDebug += Pressed;
//		InputController.onControllerDebug += Released;
//	}
//	
//	private void OnDisable()
//	{
//		InputController.onControllerDebug -= Pressed;
//		InputController.onControllerDebug -= Released;
//	}
//	
//	private void Pressed(Axis _axis, State _state)
//	{
//		if (_axis == axis && _state == State.Pressed)
//		{
//			print ("Axis: " + axis);
//			image.color = Color.black;
//		}
//	}
//	
//	private void Released(Axis _axis, State _state)
//	{
//		if (_axis == axis && _state == State.Released)
//		{
//			image.color = Color.white;
//		}
//	}
	
//	private void OnGUI ()
//	{
//		if (GUI.Button(new Rect(Screen.width - 150, 150, 170, 50), "change color "))
//		{
//			image.color = Color.black;
//		}
//		
//	}
	
}
