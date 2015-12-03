using UnityEngine;
using System.Collections;

// Class responsible for changing the PhysicMaterials of the colliders the player interacts with
//[RequireComponent(typeof(CapsuleCollider))]
public class PhysicMaterialHandler : MonoBehaviour {
	
	public PhysicMaterial groundMaterial;
	public PhysicMaterial wallMaterial;
	
	private CapsuleCollider cCollider;
	private Vector3 origin;
	private Ray ray;
	private RaycastHit hit;
	
	private void Start ()
	{
		cCollider = GetComponent<CapsuleCollider>();
	}
	
	private void OnEnable () 
	{ 
		EventManager.onCharEvent += SetGround;
		
		//print ("yoo");
	}
	
	private void OnDisable () 
	{ 
		EventManager.onCharEvent -= SetGround;
	}
	
	private void SetGround (GameEvent gEvent)
	{
		if (gEvent == GameEvent.Land)
		{
			print ("Ground material set");
			SetPhysicMaterial(true);
			
		}
	}
	/// <summary>
	/// Convinence method to set the physics material of a GameObject's mesh
	/// Used for setting ground and wall materials
	/// </summary>
	/// <param name="ground">If set to <c>true</c> ground.</param>
	private void SetPhysicMaterial(bool ground)
	{
		origin = cCollider.bounds.center - cCollider.bounds.extents;
		
		ray = new Ray(origin, Vector3.down);
		
		Debug.DrawLine (origin, origin + new Vector3(0, -0.1f, 0), Color.green);
		
		//Debug.LogError("blah");
		if (ground && Physics.Raycast (ray, out hit, 0.1f))
		{
			print ("is on ground");
			hit.collider.material = groundMaterial;
			
		}
		
	}
	
//	private void SetPhysicMaterial(Collider col, bool ground)
//	{
//		col.material = ground ? groundMaterial : wallMaterial;
//	}
	
}
