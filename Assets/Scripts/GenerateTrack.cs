using UnityEngine;
using System.Collections;

public class GenerateTrack : MonoBehaviour {

	public float trackHeight;
	public float curveVerticeDistance;
	public float maxRandomCurveAngle = 5f;
	public int verticesPerPiece;
	public int maxRepeatingSegments;
	public int maxCurveRepeatMultiplier;
	public float minGapWidth;
	public float maxGapWidth;
	public float minGapHeight;
	public float maxGapHeight;
	public float generateDistance;
	public float newTrackChance;
	public float newTrackMinDist;
	public float newTrackMaxDist;
	public float destroyDist;
	public int seed;

	public GameObject player;

	public GameObject generateTrackPrefab;

	public TrackManager trackManager;

	public float maxSteepness;
	public float minSteepness;

	public Material material;

	public int trackPieceNumber;

	private GenerateCurveMesh generateCurve;
	private Vector3 lastPiecePos = Vector3.zero;
	private Vector3 prevJoinPoint = Vector3.zero;

	private float startAngle = 0;
	private float targetAngle;
	private float segmentsRemaining = 0;
	private int pieceType = 0;

	private bool readyForNewPiece = true;

	private GameObject prevGeneratedPiece;

	void Start()
	{
		lastPiecePos = transform.position;
		startAngle = 0;
		segmentsRemaining = 0;
		pieceType = 0;

		//Random.seed = seed;
	}

	void Update()
	{
		GenerateNewTrack();
	}

	void GenerateNewTrack()
	{
		
		if (readyForNewPiece && Vector3.Distance(lastPiecePos + prevJoinPoint, player.transform.position) <= generateDistance)
		{
			print ("generating new piece");

			readyForNewPiece = false;

			StartCoroutine (GenerateNewPiece ());

			if (trackManager.currentConcurrentTracks < trackManager.maxConcurrentTracks && Random.Range(0f, 100f) <= newTrackChance)
			{
				print ("new track");

				GameObject newtrack = Instantiate (generateTrackPrefab, new Vector3(lastPiecePos.x + prevJoinPoint.x + Random.Range(newTrackMinDist, newTrackMaxDist), lastPiecePos.y + prevJoinPoint.y + Random.Range(newTrackMinDist, newTrackMaxDist), 0), Quaternion.identity) as GameObject;
			
				newtrack.GetComponent<GenerateTrack> ().player = player;

				trackManager.currentConcurrentTracks++;
			}
		}




//		for (int i = 0; i <= trackPieceNumber; i++) {
//
//			StartCoroutine (GenerateNewPiece ());
//		}
			
	}

	IEnumerator GenerateNewPiece()
	{
		print ("StartAngle " + startAngle);

		if (segmentsRemaining == 0)
		{
			targetAngle = Random.Range (minSteepness, maxSteepness);

			segmentsRemaining = Random.Range (1, maxRepeatingSegments + 1);

			if (pieceType != 1) 
			{
				pieceType = Random.Range (0, 2);
			}
			else
			{
				pieceType = 0;

				segmentsRemaining = 1;
			}

//			if (pieceType != 0)
//			{
//				segmentsRemaining *= Random.Range (1, maxRepeatingSegments + 1);
//			}
		}

		segmentsRemaining--;

		float pieceMinSteepness = 0;

		float pieceMaxSteepness = 0;

		float curveAngle = 0;

		if (pieceType == 1)
		{
			lastPiecePos = new Vector3 (lastPiecePos.x + Random.Range (minGapWidth, maxGapWidth), lastPiecePos.y + Random.Range (minGapHeight, maxGapHeight), 0);

			readyForNewPiece = true;

			yield break;
		}

		if (pieceType == 0)
		{
			if (startAngle != targetAngle) 
			{
				if (startAngle > targetAngle) 
				{

					curveAngle = Random.Range (-maxRandomCurveAngle, 0);




					pieceMinSteepness = targetAngle;
					pieceMaxSteepness = startAngle;
				} else {
					curveAngle = Random.Range (0, maxRandomCurveAngle);

					pieceMinSteepness = startAngle;
					pieceMaxSteepness = targetAngle;
				}
			}
			else
			{
				curveAngle = 0;

				pieceMinSteepness = targetAngle;
				pieceMaxSteepness = targetAngle;
			}
		}
//		else
//		{
//			curveAngle = Random.Range (-maxRandomCurveAngle, maxRandomCurveAngle);
//
//			pieceMinSteepness = minSteepness;
//			pieceMaxSteepness = maxSteepness;
//		}

		//			int maxVertCount;
		//
		//			if (curveAngle >= 0) 
		//			{
		//				maxVertCount = Mathf.RoundToInt (((80f - startAngle) / curveAngle));
		//			}
		//			else
		//			{
		//				maxVertCount = Mathf.RoundToInt (((80f + startAngle) / curveAngle));
		//			}

		//int verticeCount = Random.Range(5, Mathf.Clamp(maxVertCount, 0, 12));

		//verticeCount = (verticeCount * 2) + 2;

		generateCurve = new GenerateCurveMesh ();

		Mesh newCurvePieceMesh;

		Vector2 newJoinPoint;

		float prevPieceFinalAngle;

		generateCurve.GenerateNewCurve (trackHeight, curveVerticeDistance, verticesPerPiece, curveAngle, startAngle, pieceMinSteepness, pieceMaxSteepness, out newCurvePieceMesh, out newJoinPoint, out prevPieceFinalAngle);

		print (newJoinPoint);

		GameObject newCurvePiece = new GameObject ();



		DestroyTrack newDestroyTrack = newCurvePiece.AddComponent<DestroyTrack> ();

		newDestroyTrack.destroyDist = destroyDist;

		newDestroyTrack.player = player;

		newDestroyTrack.generator = gameObject;

		newDestroyTrack.trackManager = trackManager;

		if (prevGeneratedPiece != null)
		{
			prevGeneratedPiece.GetComponent<DestroyTrack> ().lastGeneratedPiece = false;
		}

		newDestroyTrack.lastGeneratedPiece = true;

		prevGeneratedPiece = newCurvePiece;

		newCurvePiece.AddComponent(typeof(MeshRenderer));
		MeshFilter filter = newCurvePiece.AddComponent(typeof(MeshFilter)) as MeshFilter;
		filter.mesh = newCurvePieceMesh;

		//newCurvePiece.AddComponent<MeshCollider> ();



		EdgeCollider2D edgeCollider = newCurvePiece.AddComponent<EdgeCollider2D> ();

		Vector2[] edgePoints = new Vector2[newCurvePieceMesh.vertices.Length/2];

		for (int i = 0; i < newCurvePieceMesh.vertices.Length/2; i++) {



			edgePoints [i] = new Vector2(newCurvePieceMesh.vertices [i].x, newCurvePieceMesh.vertices [i].y);
		}

		edgeCollider.points = edgePoints;


		Renderer rend = newCurvePiece.GetComponent<Renderer> ();

		Vector3 newPos = new Vector3 (lastPiecePos.x + prevJoinPoint.x, lastPiecePos.y + prevJoinPoint.y, 0);

		newCurvePiece.transform.position = newPos;

		lastPiecePos = newPos;

		prevJoinPoint = newJoinPoint;

		//lastPieceSize = rend.bounds.size;

		startAngle = prevPieceFinalAngle;

		//(((verticesPerPiece - 2) / 2) * curveAngle) + startAngle;

		rend.material = material;

		rend.material.mainTextureOffset = new Vector2 (newCurvePiece.transform.position.x * rend.material.mainTextureScale.x, newCurvePiece.transform.position.y * rend.material.mainTextureScale.y);

		//newCurvePiece.transform.parent = gameObject.transform;

		readyForNewPiece = true;

		yield break;
	}

	// Use this for initialization
//	GameObject GenerateNewCurvePiece (float height, float verticeDistance, int verticeCount, float curveAngle, float startAngle) {
//
//		Mesh newCurveMesh = generateCurve.GenerateNewCurve (height, verticeDistance, verticeCount, curveAngle, startAngle);
//
//		GameObject newCurve = new GameObject ();
//
//		newCurve.AddComponent(typeof(MeshRenderer));
//		MeshFilter filter = newCurve.AddComponent(typeof(MeshFilter)) as MeshFilter;
//		filter.mesh = newCurveMesh;
//
//		return newCurve;
//	}
	

}
