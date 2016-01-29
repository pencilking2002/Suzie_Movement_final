using UnityEngine;
using System.Collections;

public class ObstacleCharCOntroller : MonoBehaviour {

	private float timer = 5.0f;
	private float lastCollect;

	private bool collected = false;

	private void OnTriggerStay(Collider other)
	{
		if (other.CompareTag("CrystalNut") && Time.time > lastCollect + timer && !collected)
		{
			ObstacleManager.Instance.CollectNut(other.gameObject);
			lastCollect = Time.time;
			collected = true;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("CrystalNut"))
		{
			collected = false;
		}
	}

}
