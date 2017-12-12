using UnityEngine;
using System.Collections;

public class ZombieController : MonoBehaviour
{

	Animator anim;
	/*
	CharacterController controller;
	public float speed = 20.0F;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;
	*/

	void Start ()
	{
		//controller = GetComponent<CharacterController> ();
		anim = GetComponent<Animator> ();
		anim.SetBool ("IsRun", true);
	}

	void Update ()
	{
		/*
		if (controller.isGrounded) {
			moveDirection = new Vector3 (0, 0, 1.0f);
			moveDirection = transform.TransformDirection (moveDirection);
			moveDirection *= speed;
		}
		moveDirection.y -= gravity * Time.deltaTime;
		controller.Move (moveDirection * Time.deltaTime);
	*/
	}


}
