using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour {

	public void Play()
	{
		SceneManager.LoadScene(1);
	}

}
