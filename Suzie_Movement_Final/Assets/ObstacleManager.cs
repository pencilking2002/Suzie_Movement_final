using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ObstacleManager : MonoBehaviour {
	
	public static ObstacleManager Instance;

	public Text nutsCollectedText;
	//public Button respawnBTN;

	private int totalNutsCollected;
	private int totalNuts;
	private GameObject player;

	private GameObject[] spawnSpots;

	private GameObject[] boxesToRaise;
	private GameObject[] boxesToLower;

	private Sounds sounds;

	private void Awake ()
	{
		if (Instance == null)
			Instance = this;
		else
			Destroy(this);

		player = GameObject.FindGameObjectWithTag("Player");
		spawnSpots = GameObject.FindGameObjectsWithTag("SpawnSpot1");
		boxesToRaise = GameObject.FindGameObjectsWithTag("BoxToRaise1");
		boxesToLower = GameObject.FindGameObjectsWithTag("BoxToLower");

		GetTotalNuts();

		sounds = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Sounds>();
	}

	private void Start () {}

	public void CollectNut (GameObject nut)
	{
		sounds.PlayCollectNut();
		LeanTween.scale(nut, Vector3.zero, 0.2f)
			.setOnComplete(() => {
				nut.SetActive(false);
			});

		totalNutsCollected++;
		nutsCollectedText.text = "Nuts Collected: " + totalNutsCollected + " / " + totalNuts;
		if (nut.layer == 21)
			Raise();

		if (nut.layer == 22)
			Lower();
	}

	private void GetTotalNuts()
	{
		totalNuts = GameObject.FindGameObjectsWithTag("CrystalNut").Length;
		nutsCollectedText.text = "Nuts Collected: " + totalNutsCollected + " / " + totalNuts;

	}

	public void SpawnAtSpot()
	{
		player.transform.position = spawnSpots[Random.Range(0, spawnSpots.Length)].transform.position;
	}

	public void Raise()
	{
		RSUtil.Instance.DelayedAction(() => {
			sounds.PlayMovingBoxSound();
		}, 1f);

		foreach (var box in boxesToRaise)
			LeanTween.moveY(box, box.transform.position.y + 5, 2.0f);	
	}

	public void Restart()
	{
		SceneManager.LoadScene(1);
	}

	public void Lower()
	{
		RSUtil.Instance.DelayedAction(() => {
			sounds.PlayMovingBoxSound();

		}, 1f);

		foreach (var box in boxesToLower)
			LeanTween.moveY(box, box.transform.position.y - 4f, 2.0f);	
	}
}
