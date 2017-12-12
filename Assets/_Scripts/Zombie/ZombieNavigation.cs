using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieNavigation : MonoBehaviour
{
	private GameObject player;
	UnityEngine.AI.NavMeshAgent nav;
	private GameController gameController;

	// Use this for initialization
	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController> ();
		}
		player = GameObject.FindGameObjectWithTag ("Player");
		DontDestroyOnLoad (transform.gameObject);
		nav = GetComponent<UnityEngine.AI.NavMeshAgent> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		nav.SetDestination (player.transform.position);
		nav.speed = gameController.v_speed - 2.5f;
	}
}
