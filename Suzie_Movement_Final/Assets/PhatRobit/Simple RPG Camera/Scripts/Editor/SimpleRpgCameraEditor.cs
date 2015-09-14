using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using PhatRobit;

[CustomEditor(typeof(SimpleRpgCamera))]
public class SimpleRpgCameraEditor : Editor
{
	private string[] _toolbarChoices = new string[] { "Collision", "Target", "Movement", "Rotation", "Zoom", "Fade", "Mobile" };
	private int _toolbarSelection = 0;

	private string[] _mobileChoices = new string[] { "Movement", "Rotation", "Zoom" };
	private int _mobileSelection = 0;

	private bool _foldInvert = false;
	private bool _foldObjectsToRotate = false;
	private bool _foldMouseAllow = false;
	private bool _foldMouseAllowObjectsToRotate = false;

	private int _objectsToRotateSize = 0;

	private bool _init = false;

	private GUIContent _content;

	private SimpleRpgCamera _self;

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		_self = (SimpleRpgCamera)target;

		if(!_init)
		{
			_init = true;
			_objectsToRotateSize = _self.objectsToRotate.Count;
		}

		bool allowSceneObjects = !EditorUtility.IsPersistent(_self);

		_toolbarSelection = GUILayout.Toolbar(_toolbarSelection, _toolbarChoices);

		if(_toolbarSelection == 0)
		{
			#region Collision Settings

			if(_self.collisionLayers.value != 0)
			{
				_content = new GUIContent("Collision Buffer", "A small value to prevent camera clipping");
				_self.collisionBuffer = EditorGUILayout.FloatField(_content, _self.collisionBuffer);
				_self.ignoreCurrentTarget = EditorGUILayout.Toggle("Ignore Current Target", _self.ignoreCurrentTarget);
				EditorGUILayout.Separator();
			}

			if(_self.avoidanceLayers.value != 0)
			{
				_self.avoidanceSpeed = EditorGUILayout.FloatField("Avoidance Speed", _self.avoidanceSpeed);
			}

			#endregion
		}
		else if(_toolbarSelection == 1)
		{
			#region Target Settings

			_content = new GUIContent("Target Tag", "Search for a target with the specified tag");
			_self.targetTag = EditorGUILayout.TextField(_content, _self.targetTag);
			_self.target = (Transform)EditorGUILayout.ObjectField("Target", _self.target, typeof(Transform), allowSceneObjects);
			_self.targetOffset = EditorGUILayout.Vector3Field("Target Offset", _self.targetOffset);
			_self.smoothOffset = EditorGUILayout.Toggle("Smooth Offset", _self.smoothOffset);

			if(_self.smoothOffset)
			{
				EditorGUI.indentLevel++;

				_self.smoothOffsetSpeed = EditorGUILayout.FloatField("Smooth Offset Speed", _self.smoothOffsetSpeed);

				EditorGUI.indentLevel--;
			}

			_self.relativeOffset = EditorGUILayout.Toggle("Relative Offset", _self.relativeOffset);
			_self.useTargetAxis = EditorGUILayout.Toggle("Use Target Axis", _self.useTargetAxis);

			_self.softTracking = EditorGUILayout.Toggle("Soft Tracking", _self.softTracking);

			if(_self.softTracking)
			{
				EditorGUI.indentLevel++;

				_self.softTrackingRadius = EditorGUILayout.FloatField("Tracking Radius", _self.softTrackingRadius);
				_self.softTrackingSpeed = EditorGUILayout.FloatField("Tracking Speed", _self.softTrackingSpeed);

				EditorGUI.indentLevel--;
			}

			#endregion
		}
		else if(_toolbarSelection == 2)
		{
			#region Movement Settings

			_self.allowMouseDrag = EditorGUILayout.Toggle("Allow Mouse Drag", _self.allowMouseDrag);

			if(_self.allowMouseDrag)
			{
				EditorGUI.indentLevel++;

				_self.mouseDragButton = (MouseButton)EditorGUILayout.EnumPopup("Drag Button", _self.mouseDragButton);
				_self.mouseDragSensitivity = EditorGUILayout.Vector2Field("Drag Sensitivity", _self.mouseDragSensitivity);

				EditorGUI.indentLevel--;
			}

			_self.allowEdgeMovement = EditorGUILayout.Toggle("Allow Edge Movement", _self.allowEdgeMovement);

			if(_self.allowEdgeMovement)
			{
				EditorGUI.indentLevel++;

				_self.edgePadding = EditorGUILayout.FloatField("Edge Padding", _self.edgePadding);

				_self.showEdges = EditorGUILayout.Toggle("Show Edges", _self.showEdges);

				if(_self.showEdges)
				{
					EditorGUI.indentLevel++;

					_self.edgeTexture = (Texture2D)EditorGUILayout.ObjectField("Edge Texture", _self.edgeTexture, typeof(Texture2D), allowSceneObjects);

					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}

			_self.allowEdgeKeys = EditorGUILayout.Toggle("Allow Keys", _self.allowEdgeKeys);

			if(_self.allowEdgeKeys)
			{
				EditorGUI.indentLevel++;

				_self.keyMoveUp = (KeyCode)EditorGUILayout.EnumPopup("Move Up Key", _self.keyMoveUp);
				_self.keyMoveDown = (KeyCode)EditorGUILayout.EnumPopup("Move Down Key", _self.keyMoveDown);
				_self.keyMoveLeft = (KeyCode)EditorGUILayout.EnumPopup("Move Left Key", _self.keyMoveLeft);
				_self.keyMoveRight = (KeyCode)EditorGUILayout.EnumPopup("Move Right Key", _self.keyMoveRight);

				EditorGUI.indentLevel--;
			}

			_self.lockToTarget = EditorGUILayout.Toggle("Lock To Target", _self.lockToTarget);

			_self.limitBounds = EditorGUILayout.Toggle("Limit Bounds", _self.limitBounds);

			if(_self.limitBounds)
			{
				EditorGUI.indentLevel++;

				_self.boundOrigin = EditorGUILayout.Vector3Field("Origin", _self.boundOrigin);
				_self.boundSize = EditorGUILayout.Vector3Field("Size", _self.boundSize);

				EditorGUI.indentLevel--;
			}

			_self.keyFollowTarget = (KeyCode)EditorGUILayout.EnumPopup("Follow Target Key", _self.keyFollowTarget);

			_self.scrollSpeed = EditorGUILayout.FloatField("Scroll Speed", _self.scrollSpeed);

			#endregion
		}
		else if(_toolbarSelection == 3)
		{
			#region Rotation Settings

			_self.originRotation = EditorGUILayout.Vector2Field("Origin Rotation", _self.originRotation);
			_self.stayBehindTarget = EditorGUILayout.Toggle("Stay Behind Target", _self.stayBehindTarget);

			if(_self.stayBehindTarget)
			{
				EditorGUI.indentLevel++;

				_self.stayBehindTargetOnKey = EditorGUILayout.Toggle("On Key Press", _self.stayBehindTargetOnKey);

				if(_self.stayBehindTargetOnKey)
				{
					EditorGUI.indentLevel++;

					_self.stayBehindTargetKey = (KeyCode)EditorGUILayout.EnumPopup("Key", _self.stayBehindTargetKey);

					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}

			_self.returnToOrigin = EditorGUILayout.Toggle("Return To Origin", _self.returnToOrigin);

			if(_self.returnToOrigin)
			{
				EditorGUI.indentLevel++;

				_self.returnToOriginOnKey = EditorGUILayout.Toggle("On Key Press", _self.returnToOriginOnKey);

				if(_self.returnToOriginOnKey)
				{
					EditorGUI.indentLevel++;

					_self.returnToOriginKey = (KeyCode)EditorGUILayout.EnumPopup("Return To Origin Key", _self.returnToOriginKey);

					EditorGUI.indentLevel--;
				}

				_self.setOriginKey = (KeyCode)EditorGUILayout.EnumPopup("Set Origin Key", _self.setOriginKey);
				_self.setOriginLeft = EditorGUILayout.Toggle("Set With Left Button", _self.setOriginLeft);
				_self.setOriginMiddle = EditorGUILayout.Toggle("Set With Middle Button", _self.setOriginMiddle);
				_self.setOriginRight = EditorGUILayout.Toggle("Set With Right Button", _self.setOriginRight);

				EditorGUI.indentLevel--;
			}

			if(_self.stayBehindTarget || _self.returnToOrigin)
			{
				_self.returnSmoothing = EditorGUILayout.FloatField("Return Smoothing", _self.returnSmoothing);
				_self.returnDelay = EditorGUILayout.FloatField("Return Delay", _self.returnDelay);
				EditorGUILayout.Space();
			}

			_self.allowRotation = EditorGUILayout.Toggle("Allow Rotation", _self.allowRotation);

			if(_self.allowRotation)
			{
				EditorGUI.indentLevel++;

				_self.mouseHorizontalAxis = EditorGUILayout.TextField("Mouse Horizontal Axis", _self.mouseHorizontalAxis);
				_self.mouseVerticalAxis = EditorGUILayout.TextField("Mouse Vertical Axis", _self.mouseVerticalAxis);

				_self.useJoystick = EditorGUILayout.Toggle("Use Joystick", _self.useJoystick);

				if(_self.useJoystick)
				{
					EditorGUI.indentLevel++;
					_self.joystickSensitivity = EditorGUILayout.Vector2Field("Joystick Sensitivity", _self.joystickSensitivity);
					_self.joystickHorizontalAxis = EditorGUILayout.TextField("Joystick Horizontal Axis", _self.joystickHorizontalAxis);
					_self.joystickVerticalAxis = EditorGUILayout.TextField("Joystick Vertical Axis", _self.joystickVerticalAxis);
					EditorGUI.indentLevel--;
				}

				_self.mouseLook = EditorGUILayout.Toggle("Mouse Look", _self.mouseLook);

				if(_self.mouseLook)
				{
					EditorGUI.indentLevel++;
					_self.lockCursor = EditorGUILayout.Toggle("Lock Mouse", _self.lockCursor);
					_self.disableWhileUnlocked = EditorGUILayout.Toggle("Disable While Unlocked", _self.disableWhileUnlocked);
					EditorGUI.indentLevel--;
				}
				else
				{
					_foldMouseAllow = EditorGUILayout.Foldout(_foldMouseAllow, "Allowed Mouse Buttons");

					if(_foldMouseAllow)
					{
						EditorGUI.indentLevel++;

						_self.allowRotationLeft = EditorGUILayout.Toggle("Left Button", _self.allowRotationLeft);
						_self.allowRotationMiddle = EditorGUILayout.Toggle("Middle Button", _self.allowRotationMiddle);
						_self.allowRotationRight = EditorGUILayout.Toggle("Right Button", _self.allowRotationRight);

						EditorGUI.indentLevel--;
					}

					if(_self.allowRotationLeft || _self.allowRotationMiddle || _self.allowRotationRight)
					{
						_self.lockCursor = EditorGUILayout.Toggle("Lock Mouse", _self.lockCursor);

						if(_self.lockCursor)
						{
							EditorGUI.indentLevel++;

							if(_self.allowRotationLeft)
							{
								_self.lockLeft = EditorGUILayout.Toggle("Left Button", _self.lockLeft);
							}

							if(_self.allowRotationMiddle)
							{
								_self.lockMiddle = EditorGUILayout.Toggle("Middle Button", _self.lockMiddle);
							}

							if(_self.allowRotationRight)
							{
								_self.lockRight = EditorGUILayout.Toggle("Right Button", _self.lockRight);
							}

							EditorGUI.indentLevel--;
						}
					}
				}

				EditorGUILayout.LabelField("Rotation Angle Limit [Min: " + _self.minAngle + " | Max: " + _self.maxAngle + "]");
				EditorGUILayout.MinMaxSlider(ref _self.minAngle, ref _self.maxAngle, -85, 85);

				_self.minAngle = Mathf.Round(_self.minAngle);
				_self.maxAngle = Mathf.Round(_self.maxAngle);

				_foldInvert = EditorGUILayout.Foldout(_foldInvert, "Invert Rotation");

				if(_foldInvert)
				{
					EditorGUI.indentLevel++;
					_self.invertRotationX = EditorGUILayout.Toggle("X", _self.invertRotationX);
					_self.invertRotationY = EditorGUILayout.Toggle("Y", _self.invertRotationY);
					EditorGUI.indentLevel--;
				}

				_self.rotationSensitivity = EditorGUILayout.Vector2Field("Sensitivity", _self.rotationSensitivity);

				if(_self.allowRotationLeft || _self.allowRotationMiddle || _self.allowRotationRight)
				{
					_self.rotateObjects = EditorGUILayout.Toggle("Rotate Objects", _self.rotateObjects);

					if(_self.rotateObjects)
					{
						EditorGUI.indentLevel++;

						_self.autoAddTargetToRotate = EditorGUILayout.Toggle("Auto Add Target", _self.autoAddTargetToRotate);

						if(!_self.mouseLook)
						{
							_foldMouseAllowObjectsToRotate = EditorGUILayout.Foldout(_foldMouseAllowObjectsToRotate, "Allowed Mouse Buttons");

							if(_foldMouseAllowObjectsToRotate)
							{
								EditorGUI.indentLevel++;

								if(_self.allowRotationLeft)
								{
									_self.rotateObjectsLeft = EditorGUILayout.Toggle("Left Button", _self.rotateObjectsLeft);
								}

								if(_self.allowRotationMiddle)
								{
									_self.rotateObjectsMiddle = EditorGUILayout.Toggle("Middle Button", _self.rotateObjectsMiddle);
								}

								if(_self.allowRotationRight)
								{
									_self.rotateObjectsRight = EditorGUILayout.Toggle("Right Button", _self.rotateObjectsRight);
								}

								EditorGUI.indentLevel--;
							}
						}

						_foldObjectsToRotate = EditorGUILayout.Foldout(_foldObjectsToRotate, "Objects To Rotate");

						if(_foldObjectsToRotate)
						{
							EditorGUI.indentLevel++;

							_objectsToRotateSize = EditorGUILayout.IntField("Size", _objectsToRotateSize);

							if(_objectsToRotateSize < 0)
							{
								_objectsToRotateSize = 0;
							}

							Transform[] objectsToRotate = new Transform[_objectsToRotateSize];

							for(int i = 0; i < _objectsToRotateSize; i++)
							{
								if(_self.objectsToRotate.Count == i)
								{
									break;
								}

								objectsToRotate[i] = _self.objectsToRotate[i];
							}

							for(int i = 0; i < _objectsToRotateSize; i++)
							{
								objectsToRotate[i] = (Transform)EditorGUILayout.ObjectField("Element " + i, objectsToRotate[i], typeof(Transform), allowSceneObjects);
							}

							_self.objectsToRotate = new List<Transform>();

							foreach(Transform t in objectsToRotate)
							{
								if(t)
								{
									_self.objectsToRotate.Add(t);
								}
							}

							EditorGUI.indentLevel--;
						}

						EditorGUI.indentLevel--;
					}
				}

				EditorGUI.indentLevel--;
			}

			_self.allowRotationKeys = EditorGUILayout.Toggle("Allow Rotation Keys", _self.allowRotationKeys);

			if(_self.allowRotationKeys)
			{
				EditorGUI.indentLevel++;

				_self.keyRotateUp = (KeyCode)EditorGUILayout.EnumPopup("Up", _self.keyRotateUp);
				_self.keyRotateDown = (KeyCode)EditorGUILayout.EnumPopup("Down", _self.keyRotateDown);
				_self.keyRotateLeft = (KeyCode)EditorGUILayout.EnumPopup("Left", _self.keyRotateLeft);
				_self.keyRotateRight = (KeyCode)EditorGUILayout.EnumPopup("Right", _self.keyRotateRight);
				_self.rotationKeySensitivity = EditorGUILayout.Vector2Field("Sensitivity", _self.rotationKeySensitivity);

				EditorGUI.indentLevel--;
			}

			if(_self.allowRotation || _self.allowRotationKeys)
			{
				_self.autoSmoothing = EditorGUILayout.Toggle("Auto Smoothing", _self.autoSmoothing);

				if(!_self.autoSmoothing)
				{
					_self.rotationSmoothing = EditorGUILayout.FloatField("Rotation Smoothing", _self.rotationSmoothing);

					EditorGUILayout.HelpBox("If you notice some strange 'rubberbanding' effect while rotating the camera quickly, increasing the Rotation Smoothing (or reducing the Sensitivity) may help to reduce it. The Auto Smoothing setting usually helps avoid this issue for you.", MessageType.Info);
				}
			}

			#endregion
		}
		else if(_toolbarSelection == 4)
		{
			#region Zoom Settings

			_self.allowZoom = EditorGUILayout.Toggle("Allow Zoom", _self.allowZoom);

			_self.allowZoomKeys = EditorGUILayout.Toggle("Allow Zoom Keys", _self.allowZoomKeys);

			if(_self.allowZoomKeys)
			{
				EditorGUI.indentLevel++;

				_content = new GUIContent("Zoom In Key", "Key for zooming in");
				_self.keyZoomIn = (KeyCode)EditorGUILayout.EnumPopup(_content, _self.keyZoomIn);
				_content = new GUIContent("Zoom Out Key", "Key for zooming out");
				_self.keyZoomOut = (KeyCode)EditorGUILayout.EnumPopup(_content, _self.keyZoomOut);
				_content = new GUIContent("Zoom Key Delay", "The amount of time needed to hold the key down before constant zoom takes effect");
				_self.keyZoomDelay = EditorGUILayout.FloatField(_content, _self.keyZoomDelay);

				EditorGUI.indentLevel--;
			}

			if(_self.allowZoom || _self.allowZoomKeys)
			{
				EditorGUI.indentLevel++;

				_self.zoomSpeed = EditorGUILayout.FloatField("Zoom Speed", _self.zoomSpeed);
				_self.zoomSmoothing = EditorGUILayout.FloatField("Zoom Smoothing", _self.zoomSmoothing);
				_self.invertZoom = EditorGUILayout.Toggle("Invert Direction", _self.invertZoom);

				EditorGUI.indentLevel--;
			}

			_content = new GUIContent("Distance", "The distance between the camera and target");

			_self.distance = EditorGUILayout.FloatField(_content, _self.distance);
			EditorGUI.indentLevel++;
			EditorGUILayout.LabelField("[Min: " + _self.minDistance + " | Max: " + _self.maxDistance + "]");
			EditorGUILayout.MinMaxSlider(ref _self.minDistance, ref _self.maxDistance, 1, 200);
			EditorGUI.indentLevel--;

			_self.minDistance = Mathf.Round(_self.minDistance);
			_self.maxDistance = Mathf.Round(_self.maxDistance);

			#endregion
		}
		else if(_toolbarSelection == 5)
		{
			#region Fade Settings

			_self.fadeCurrentTarget = EditorGUILayout.Toggle("Fade Current Target", _self.fadeCurrentTarget);

			if(_self.fadeCurrentTarget)
			{
				EditorGUI.indentLevel++;
				_self.fadeDistance = EditorGUILayout.FloatField("Fade Distance", _self.fadeDistance);
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Separator();

			_self.replaceShaders = EditorGUILayout.Toggle("Replace Shaders", _self.replaceShaders);

			if(_self.replaceShaders)
			{
				EditorGUI.indentLevel++;
				_content = new GUIContent("Transparent Shader", "This will replace the object's current shader with the selected shader when fading (Defaults to Standard Shader if left blank)");
				_self.transparentShader = (Shader)EditorGUILayout.ObjectField(_content, _self.transparentShader, typeof(Shader), false);
				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Separator();

			if(_self.collisionAlphaLayers.value != 0)
			{
				_content = new GUIContent("Fade Amount", "The alpha value for faded objects in front of the target");
				_self.fadeAmount = EditorGUILayout.Slider(_content, _self.fadeAmount, 0, 1);
				_content = new GUIContent("Fade Speed", "Modifier for the time it takes to fade objects in / out");
				_self.alphaFadeSpeed = EditorGUILayout.FloatField(_content, _self.alphaFadeSpeed);
			}
			else
			{
				EditorGUILayout.HelpBox("There are no Collision Alpha Layers specified", MessageType.Info);
			}

			/*
			public bool fadeCurrentTarget = true;

		[HideInInspector]
		public float fadeDistance = 1;
		[HideInInspector]
		public float fadeAmount = 0.25f;
		[HideInInspector]
		public float alphaFadeSpeed = 10;
		[HideInInspector]
		public Shader transparentShader;
			 */

			#endregion
		}
		else if(_toolbarSelection == 6)
		{
			#region Mobile Settings

			EditorGUILayout.HelpBox("Note: Most of the desktop settings affect both desktop and mobile functionality. Settings in this tab that are highlighted in yellow will override the desktop settings on mobile devices for convenience.", MessageType.Info);

			_self.allowTouch = EditorGUILayout.Toggle("Allow Touch", _self.allowTouch);

			if(_self.allowTouch)
			{
				_self.touchSensitivity = EditorGUILayout.FloatField("Touch Sensitivity", _self.touchSensitivity);

				_mobileSelection = GUILayout.Toolbar(_mobileSelection, _mobileChoices);

				if(_mobileSelection == 0)
				{
					#region Mobile Movement Settings

					if(_self.allowEdgeMovement)
					{
						_self.mobilePanType = (PanControlType)EditorGUILayout.EnumPopup("Movement Control Method", _self.mobilePanType);

						if(_self.mobilePanType == PanControlType.Drag)
						{
							EditorGUI.indentLevel++;
							_self.mobilePanningTouchCount = EditorGUILayout.IntField("Panning Touch Count", _self.mobilePanningTouchCount);
							EditorGUI.indentLevel--;
						}
						else if(_self.mobilePanType == PanControlType.Swipe)
						{
							EditorGUI.indentLevel++;
							_self.mobilePanSwipeActiveTime = EditorGUILayout.FloatField("Swipe Detection Time", _self.mobilePanSwipeActiveTime);
							_self.mobilePanSwipeMinDistance = EditorGUILayout.FloatField("Min Swipe Distance", _self.mobilePanSwipeMinDistance);
							_self.mobilePanSwipeDistance = EditorGUILayout.Vector2Field("Movement Distance", _self.mobilePanSwipeDistance);
							EditorGUI.indentLevel--;
						}
					}
					else
					{
						EditorGUILayout.HelpBox("Edge movement is disabled. Enable it in the Movement tab.", MessageType.Info);
					}

					#endregion
				}
				else if(_mobileSelection == 1)
				{
					#region Mobile Rotation Settings

					if(_self.allowRotation)
					{
						_self.mobileRotationType = (RotationControlType)EditorGUILayout.EnumPopup("Rotation Control Method", _self.mobileRotationType);

						if(_self.mobileRotationType == RotationControlType.Swipe)
						{
							EditorGUI.indentLevel++;
							_self.mobileSwipeActiveTime = EditorGUILayout.FloatField("Swipe Detection Time", _self.mobileSwipeActiveTime);
							_self.mobileSwipeMinDistance = EditorGUILayout.FloatField("Min Swipe Distance", _self.mobileSwipeMinDistance);
							_self.mobileSwipeRotationAmount = EditorGUILayout.Vector2Field("Swipe Rotation Amount", _self.mobileSwipeRotationAmount);
							EditorGUI.indentLevel--;
						}
						else if(_self.mobileRotationType == RotationControlType.Drag)
						{
							EditorGUI.indentLevel++;
							_self.mobileRotationDelay = EditorGUILayout.FloatField("Rotation Delay", _self.mobileRotationDelay);
							EditorGUI.indentLevel--;
						}
					}
					else
					{
						EditorGUILayout.HelpBox("Rotation is disabled. Enable it in the Rotation tab.", MessageType.Info);
					}

					#endregion
				}
				else if(_mobileSelection == 2)
				{
					#region Mobile Zoom Settings

					if(_self.allowZoom)
					{
						_self.mobileZoomDeadzone = EditorGUILayout.FloatField("Zoom Deadzone", _self.mobileZoomDeadzone);
						GUI.color = Color.yellow;
						_self.mobileZoomSpeed = EditorGUILayout.FloatField("Zoom Speed", _self.mobileZoomSpeed);
						GUI.color = Color.white;
					}
					else
					{
						EditorGUILayout.HelpBox("Zoom is disabled. Enable it in the Zoom tab.", MessageType.Info);
					}

					#endregion
				}
			}

			#endregion
		}

		if(GUI.changed)
		{
			EditorUtility.SetDirty(_self);
		}
	}
}