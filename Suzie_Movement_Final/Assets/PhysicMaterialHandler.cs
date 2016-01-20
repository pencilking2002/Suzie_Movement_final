using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Class responsible for changing the PhysicMaterials of the colliders the player interacts with
//[RequireComponent(typeof(CapsuleCollider))]
public class PhysicMaterialHandler : MonoBehaviour {
	
	public PhysicMaterial groundMaterial;
	public PhysicMaterial wallMaterial;

	public float groundRayLenth = 0.1f;
	public float wallRayLength = 0.5f;

	private CapsuleCollider cCollider;
	private Vector3 origin;
	private Ray ray;
	private RaycastHit hit;
	
	private void Start ()
	{
		cCollider = GetComponent<CapsuleCollider>();

		ComponentActivator.Instance.Register(this, new Dictionary<GameEvent, bool> { 

			{ GameEvent.StopVineClimbing, true },
			{ GameEvent.StopEdgeClimbing, true },

			{ GameEvent.StartVineClimbing, false },
			{ GameEvent.StartEdgeClimbing, false }, 

		});
	}
	
	private void OnEnable () 
	{ 
		EventManager.onCharEvent += SetPhysicMaterial;
		
		//print ("yoo");
	}
	
	private void OnDisable () 
	{ 
		EventManager.onCharEvent -= SetPhysicMaterial;
	}
	
//	private void SetGround (GameEvent gEvent)
//	{
//		if (gEvent == GameEvent.Land)
//		{
//			print ("Ground material set");
//			SetPhysicMaterial(true, new Ray(origin, Vector3.down));
//		}
//	}
	/// <summary>
	/// Convinence method to set the physics material of a GameObject's mesh
	/// Used for setting ground and wall materials
	/// </summary>
	/// <param name="ground">If set to <c>true</c> ground.</param>
	private void SetPhysicMaterial(GameEvent gEvent)
	{
		if (gEvent == GameEvent.Land)
		{
			origin = cCollider.bounds.center - cCollider.bounds.extents;
			
			ray = new Ray(origin, Vector3.down);
			
			Debug.DrawLine (origin, origin + new Vector3(0, -groundRayLenth, 0), Color.green);
			
			//Debug.LogError("blah");
			if (Physics.Raycast (ray, out hit, groundRayLenth))
			{
				print ("is on ground");
				hit.collider.material = groundMaterial;
				
			}
		}
		else if (gEvent == GameEvent.WallCollision)
		{
			origin = cCollider.bounds.center;
			
			ray = new Ray(origin, transform.forward);
//			
//			
//			Debug.LogError("blah");
			if (Physics.Raycast (ray, out hit, wallRayLength))
			{
				Debug.DrawLine (origin, hit.point, Color.green);

				hit.collider.material = wallMaterial;
				print ("Wall Collision");
			}

		}
		
	}
	
//	private void SetPhysicMaterial(Collider col, bool ground)
//	{
//		col.material = ground ? groundMaterial : wallMaterial;
//	}
	
}
