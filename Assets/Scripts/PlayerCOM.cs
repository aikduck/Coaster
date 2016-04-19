using UnityEngine;
using System.Collections;

public class PlayerCOM : MonoBehaviour {

	public Vector2 centerOfMass;

	// Use this for initialization
	void Start () {
	
		gameObject.GetComponent<Rigidbody2D> ().centerOfMass = centerOfMass;
	}
}
