using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
	private bool gameOver;
	private bool restart;
	private int score;

	public GameObject player;
	public Camera zzCamera;

	bool isPlayed = false;

	bool gamePaused = false;

	public Text gameOverText;
	public Text scoreText;
	public Text pauseText;
	public Text finalScoreText;
	public Button continueButton;
	public Button restartButton;
	public Button exitButton;

	//player
	public float v_speed;

	AudioSource audio1;
	AudioSource audio2;

	//restart
	private Vector3 startPos;

	private float nextTime;

	void Awake ()
	{
		DontDestroyOnLoad (transform.gameObject);
		if (FindObjectsOfType (GetType ()).Length > 1) {
			Destroy (gameObject);
		}
	}

	// Use this for initialization
	void Start ()
	{
		AudioSource[] audioSource = GetComponents<AudioSource> ();
		gameOver = false;
		restart = false;

		score = 0;
		nextTime = Time.time;

		audio1 = audioSource [1];
		audio2 = audioSource [2];

		gameOverText.text = "";
		scoreText.text = "0";
		pauseText.text = "Game Paused!";
		finalScoreText.text = "";

		startPos = player.transform.transform.position;
		continueButton.onClick.AddListener (OnContinueClick);
		exitButton.onClick.AddListener (OnExitClick);
		restartButton.onClick.AddListener (OnRestartClick);
		HideButtons ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			if (gamePaused == false) {
				gamePaused = true;
				Time.timeScale = 0.0f;
				ShowButtons ();
			} else {
				gamePaused = false;
				Time.timeScale = 1.0f;
				HideButtons ();
			}
		}

		if (Time.time >= nextTime) {
			nextTime = Time.time + 0.3f;
			AddScore (1);
		}
	}

	public void GameOver ()
	{
		gameOver = true;
		gameOverText.text = "GAME OVER!";
		scoreText.text = score + "";
		if (isPlayed == false && audio2.isPlaying == false) {
			audio2.Play ();
			isPlayed = true;
		}
		Time.timeScale = 0.0f;
		restartButton.gameObject.SetActive (true);
		exitButton.gameObject.SetActive (true);
		scoreText.text = "";
		finalScoreText.text = "You scored " + score + " !";
	}

	public void AddScore (int count)
	{
		score += count;
		scoreText.text = score + "";
	}

	public void PlayPickUpMusic ()
	{
		audio1.Play ();
	}

	private void OnContinueClick ()
	{
		gamePaused = false;
		Time.timeScale = 1.0f;
		HideButtons ();
	}

	private void OnRestartClick ()
	{
		//todo
		GameObject[] allGameobjects = GameObject.FindObjectsOfType (typeof(GameObject)) as GameObject[];
		foreach (GameObject gb in allGameobjects) {
			if (!gb.CompareTag ("ZZ")) {
				Destroy (gb);
			}
		}
		zzCamera.enabled = true;
		SceneManager.LoadScene ("start", LoadSceneMode.Single);

		gamePaused = false;
		Time.timeScale = 1.0f;
	}

	private void OnExitClick ()
	{
		Application.Quit ();
	}

	public void ShowButtons ()
	{
		pauseText.gameObject.SetActive (true);
		continueButton.gameObject.SetActive (true);
		restartButton.gameObject.SetActive (true);
		exitButton.gameObject.SetActive (true);
	}

	public void HideButtons ()
	{
		pauseText.gameObject.SetActive (false);
		continueButton.gameObject.SetActive (false);
		restartButton.gameObject.SetActive (false);
		exitButton.gameObject.SetActive (false);
	}
}
