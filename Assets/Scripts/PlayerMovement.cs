using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public float movementForce;

	private Rigidbody2D playerRb;

	// Use this for initialization
	void Start () {
	
		playerRb = gameObject.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		playerRb.AddForce (new Vector3 (0, Input.GetAxis ("Vertical"), 0).normalized * movementForce);
	}
}
