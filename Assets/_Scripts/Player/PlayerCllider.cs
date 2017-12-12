using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerCllider: MonoBehaviour
{
	private GameController gameController;
	private GameObject zombies;

	public GameObject zombie;

	private float offset_x;
	private float currentTime;
	private float ctime;
	private Vector3 pzoffset;
	private Quaternion zombiesQuat;
	bool firstCollider = false;
	bool sceneChanged = false;

	// Use this for initialization
	void Start ()
	{
		offset_x = 0.0f;
		currentTime = 0.0f;
		ctime = 0.0f;
		firstCollider = false;
		sceneChanged = false;
		GameObject gameControllerObject = GameObject.FindWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent<GameController> ();
		}
		zombies = GameObject.FindGameObjectWithTag ("Zombie");
	}

	// Update is called once per frame
	void Update ()
	{
		if (sceneChanged == true) {
			this.transform.position = new Vector3 (offset_x, transform.position.y, -463f);
			zombies = Instantiate (zombie, new Vector3 (transform.position.x - pzoffset.x, transform.position.y - pzoffset.y, (-463) - pzoffset.z), zombiesQuat) as GameObject;

			sceneChanged = false;
		}
	}

	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		if (hit.collider.gameObject.CompareTag ("Boundary")) {
			offset_x = transform.position.x - hit.collider.gameObject.transform.position.x;
			pzoffset = transform.position - zombies.transform.position;
			if (pzoffset.z > 25.0) {
				pzoffset.z = 25.0f;
			}
			zombiesQuat = zombies.transform.rotation;
			Destroy (zombies.gameObject);
			string sceneName = "jigsaw" + Random.Range (1, 4);

			SceneManager.LoadScene (sceneName, LoadSceneMode.Single);
			sceneChanged = true;
		}

		if (hit.collider.gameObject.CompareTag ("Death") ||
		    hit.collider.gameObject.CompareTag ("Obstruction") ||
		    hit.collider.gameObject.CompareTag ("Obstruction2")) {
			gameController.GameOver ();
		}

		if (hit.collider.gameObject.CompareTag ("Fence")) {
			float fence_angles_y = Mathf.Abs (hit.collider.gameObject.transform.parent.transform.rotation.eulerAngles.y);
			float player_angles_y = Mathf.Abs (this.transform.rotation.eulerAngles.y);
			if ((Mathf.Abs (fence_angles_y - player_angles_y) / 90) % 2 == 1) {
				//todo
			}
		} 

		if (hit.collider.gameObject.CompareTag ("Zombie")) {
			gameController.GameOver ();
		}
	}
}