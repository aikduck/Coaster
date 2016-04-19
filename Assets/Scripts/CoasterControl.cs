using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class CoasterControl : MonoBehaviour {

	public GameObject raycastOrigin;

	public float raycastDistance;
	public float detectDistance;
	public float moveSpeed;

	public LayerMask trackLayerMask;

	public GenerateTrack generateTrack;

	private bool onTrack = false;

	private GameObject currentTrack;
	private EdgeCollider2D currentTrackEdgeCollider;

	private Vector3 initialPosition;
	private float initPosLerpOffset;
	private Vector3 currentDirection;
	private Vector3 prevDirection;
	private Vector3 currentPoint;
	private Vector3 prevPoint;
	private int currentPointIndex;

	private float prevHitDist = 0;

	private float curveVerticeDistance;

	private bool usingInitPos = false;

	private Stopwatch stopWatch;

	private Rigidbody2D playerRb;

	// Use this for initialization
	void Start () {
	
		stopWatch = new Stopwatch ();

		playerRb = gameObject.GetComponent<Rigidbody2D> ();

		curveVerticeDistance = generateTrack.curveVerticeDistance;
	}
	
	// Update is called once per frame
	void Update () {
	
		DetectTrackBelow ();
	}

	void DetectTrackBelow()
	{
		if (onTrack == false) 
		{
			RaycastHit2D hit = Physics2D.Raycast (new Vector2(raycastOrigin.transform.position.x, raycastOrigin.transform.position.y), Vector2.down, raycastDistance, trackLayerMask);

			UnityEngine.Debug.DrawRay (raycastOrigin.transform.position, Vector3.down * raycastDistance, Color.blue);

			if (hit.collider != null ) 
			{
				if (hit.distance <= detectDistance && (hit.distance < prevHitDist && prevHitDist >= detectDistance)) 
				{
					currentTrack = hit.collider.gameObject;

					initialPosition = new Vector3 (hit.point.x, hit.point.y, 0);

					currentTrackEdgeCollider = currentTrack.GetComponent<EdgeCollider2D> ();

					gameObject.transform.position = hit.point;

					for (int i = 0; i < currentTrackEdgeCollider.pointCount; i++) {

						if (currentTrackEdgeCollider.points [i].x + currentTrack.transform.position.x >= hit.point.x) {
							currentPoint = currentTrackEdgeCollider.points [i];

							currentPointIndex = i;

							i = currentTrackEdgeCollider.pointCount;
						}
					}

					currentDirection = (currentPoint - (initialPosition - currentTrack.transform.position)).normalized;

					prevDirection = currentDirection;

					transform.right = currentDirection;

					SetOnTrack ();

					usingInitPos = true;

					initPosLerpOffset = Vector3.Distance (initialPosition, currentPoint + currentTrack.transform.position) / curveVerticeDistance;

					stopWatch.Start ();
				}

				prevHitDist = hit.distance;
			}
			else if (prevHitDist != 0)
			{
				prevHitDist = 0;
			}
		}
		else
		{
			if ((stopWatch.ElapsedMilliseconds / (moveSpeed*initPosLerpOffset)) >= 1)
			{
				if (usingInitPos)
				{
					usingInitPos = false;

					initPosLerpOffset = 1;
				}

				prevPoint = currentPoint;

				currentPointIndex++;

				if (currentPointIndex == currentTrackEdgeCollider.pointCount)
				{
					currentPointIndex = 1;

					if (currentTrack.GetComponent<CurveInfo>().nextTrackPiece != null)
					{
						currentTrack = currentTrack.GetComponent<CurveInfo> ().nextTrackPiece;

						currentTrackEdgeCollider = currentTrack.GetComponent<EdgeCollider2D> ();

						currentPoint = currentTrackEdgeCollider.points [currentPointIndex];

						prevPoint = currentTrackEdgeCollider.points [0];
					}
					else
					{
						SetOffTrack ();
					}
				}

				currentPoint = currentTrackEdgeCollider.points [currentPointIndex];

				prevDirection = currentDirection;

				currentDirection = (currentPoint - prevPoint).normalized;

				stopWatch.Reset ();
				stopWatch.Start ();
			}

			transform.right = Vector3.Lerp (prevDirection, currentDirection, stopWatch.ElapsedMilliseconds / moveSpeed);

			if (usingInitPos)
			{
				gameObject.transform.position = Vector3.Lerp (initialPosition, currentPoint + currentTrack.transform.position, (stopWatch.ElapsedMilliseconds / (moveSpeed*initPosLerpOffset)));
			}
			else
			{
				gameObject.transform.position = Vector3.Lerp (prevPoint + currentTrack.transform.position, currentPoint + currentTrack.transform.position, stopWatch.ElapsedMilliseconds / moveSpeed);
			}
		}
	}

	void SetOnTrack()
	{
		playerRb.constraints = RigidbodyConstraints2D.FreezeAll;

		onTrack = true;
	}

	void SetOffTrack()
	{
		playerRb.constraints = RigidbodyConstraints2D.None;

		onTrack = false;
	}
}
