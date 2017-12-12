using UnityEngine;
using System.Collections;

public class CamerasController : MonoBehaviour
{
	public Camera startCamera;
	public Camera mainCamera;
	public Camera zzCamera;

	public float timer = 3.0f;
	float start_time;
	public float ruler = 6.0f;

	bool isZZ = true;
	// Use this for initialization

	void Awake ()
	{
		DontDestroyOnLoad (zzCamera.gameObject);
	}

	void Start ()
	{
		zzCamera.enabled = true;
		startCamera.enabled = false;
		mainCamera.enabled = false;
		start_time = Time.time;
	}

	// Update is called once per frame
	void Update ()
	{
		if (isZZ) {
			foreach (Camera camera in Camera.allCameras) {
				camera.enabled = false;
			}
			isZZ = false;
			startCamera.enabled = true;
		}

		if (Time.time > (start_time + timer)) {
			startCamera.enabled = false;
			mainCamera.enabled = true;
			//Destroy (GameObject.FindGameObjectWithTag ("Zombie"));
		} else {
			startCamera.transform.Rotate (new Vector3 (ruler / 3, -ruler, ruler / 2) * Time.deltaTime);
		}
	}
}