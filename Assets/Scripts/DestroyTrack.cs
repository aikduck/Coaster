using UnityEngine;
using System.Collections;

public class DestroyTrack : MonoBehaviour {

	public float destroyDist = Mathf.Infinity;

	public GameObject player;

	public GameObject generator;

	public TrackManager trackManager;

	public bool lastGeneratedPiece = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
		if (player.transform.position.x - transform.position.x >= destroyDist)
		{
			if (lastGeneratedPiece)
			{
				print ("destroying generate track object");

				trackManager.currentConcurrentTracks--;

				Destroy(generator);
				Destroy (gameObject);
			}
			else
			{
				Destroy (gameObject);
			}
		}

	}
}
