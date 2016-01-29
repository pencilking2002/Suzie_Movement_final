using UnityEngine;
using System.Collections;

public class Sounds : MonoBehaviour {

	public AudioClip movingBoxAudio;
	public AudioClip nutCollectAudio;

	private AudioSource audio;

	// Use this for initialization
	void Start () {
		audio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlayCollectNut ()
	{
		audio.clip = nutCollectAudio;
		audio.Play();
	}

	public void PlayMovingBoxSound ()
	{
		audio.clip = movingBoxAudio;
		audio.Play();
	}

}
