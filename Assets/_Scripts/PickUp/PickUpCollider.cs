using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpCollider : MonoBehaviour
{
	private GameController gameController;

	public int score = 10;

	// Use this for initialization
	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController> ();
		}

	}

	void OnTriggerEnter (Collider collider)
	{
		if (collider.gameObject.CompareTag ("Player")) {
			gameController.AddScore (score);
			gameController.PlayPickUpMusic ();
			Destroy (this.gameObject);
		}
	}
}
