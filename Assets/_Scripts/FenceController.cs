using UnityEngine;
using System.Collections;

public class FenceController : MonoBehaviour
{

	private GameController gameController;

	void Start ()
	{
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController> ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	void OnCollisionEnter (Collision collision)
	{
		Debug.Log (collision.gameObject.name);
		if (collision.gameObject.CompareTag ("Player")) {
			Debug.Log (collision.relativeVelocity.magnitude);
		}

	}
}
