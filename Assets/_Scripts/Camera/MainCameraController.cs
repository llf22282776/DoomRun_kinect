using UnityEngine;
using System.Collections;

public class MainCameraController : MonoBehaviour
{
	public GameObject player;
	private Vector3 offset;
	private Rigidbody playerRb;

	// Use this for initialization
	void Start ()
	{
		offset = transform.position - player.transform.position;
		playerRb = player.GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
//			Vector3 rotateVector = new Vector3 (0, -90, 0);
//			this.transform.Rotate (rotateVector);
//
//			Vector3 curVelocity = playerRb.velocity;
//			playerRb.velocity = new Vector3 (-curVelocity.z, 0.0f, 0.0f);
//
//			float tmp = offset.x;
//			offset.x = -offset.z;
//			offset.z = tmp;
//			Debug.Log (offset.x + "" + offset.y + "" + offset.z);
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
//			Vector3 rotateVector = new Vector3 (0, 90, 0);
//			this.transform.Rotate (rotateVector);
//
//			float tmp = offset.x;
//			offset.x = offset.z;
//			offset.z = tmp;
//
//			Vector3 curVelocity = playerRb.velocity;
//			playerRb.velocity = new Vector3 (curVelocity.z, 0.0f, 0.0f);
		} else {
		}
	}

	void LateUpdate ()
	{
		transform.position = player.transform.position + offset;
	}
}
