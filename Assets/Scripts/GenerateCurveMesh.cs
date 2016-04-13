using UnityEngine;
using System.Collections;

public class GenerateCurveMesh
{

	public void GenerateNewCurve (float height, float verticeDistance, int verticeCount, float curveAngle, float startAngle, float minSteepness, float maxSteepness, out Mesh newCurveMesh, out Vector2 joinPoint, out float lastAngle) {
		// Create Vector2 vertices
		Vector2[] vertices2D = new Vector2[verticeCount];

		float currentCurve = startAngle;

		float realCurve = currentCurve;

		vertices2D [0] = new Vector2 (0, 0);
		//vertices2D [1] = new Vector2 (distance, height);

		for (int i = 1; i < (verticeCount/2); i++) {

			vertices2D [i] = new Vector2 (vertices2D[(int)Mathf.Clamp(i-1, 0, Mathf.Infinity)].x + GetOffsetLength (currentCurve, verticeDistance), vertices2D[(int)Mathf.Clamp(i-1, 0, Mathf.Infinity)].y - GetOffsetHeight (currentCurve, verticeDistance));

			currentCurve = Mathf.Clamp((currentCurve + curveAngle), minSteepness, maxSteepness);

			realCurve += curveAngle;
		}



//		vertices2D [0] = new Vector2(0,0);
//		vertices2D [1] = new Vector2 (0, height);
//		vertices2D [2]= new Vector2 (distance, height);
//		vertices2D [3] = new Vector2 (distance + GetOffsetLength (curveAngle, distance), height - GetOffsetHeight (curveAngle, distance));
//
//		currentCurve += curveAngle;
//
//		vertices2D [4] = new Vector2 (vertices2D[3].x + GetOffsetLength (curveAngle + currentCurve, distance), vertices2D[3].y - GetOffsetHeight (curveAngle + currentCurve, distance));
//
//		currentCurve += curveAngle;
//
//		vertices2D [5] = new Vector2 (vertices2D[4].x + GetOffsetLength (curveAngle + currentCurve, distance), vertices2D[4].y - GetOffsetHeight (curveAngle + currentCurve, distance));
//
//		currentCurve += curveAngle;

//		vertices2D [6] = new Vector2 (vertices2D[5].x + GetOffsetLength (curveAngle + currentCurve, distance), vertices2D[5].y - GetOffsetHeight (curveAngle + currentCurve, distance));

//		currentCurve = Mathf.Clamp((currentCurve + curveAngle), minSteepness, maxSteepness);
//
//		realCurve += curveAngle;

		lastAngle = currentCurve;

		int a = 1;

		for (int i = verticeCount/2; i < verticeCount; i++) {

			currentCurve = Mathf.Clamp(realCurve, minSteepness, maxSteepness);

			vertices2D [i] = new Vector2 (vertices2D [(verticeCount/2)-a].x - GetOffsetHeight(currentCurve, height), vertices2D [(verticeCount/2)-a].y - GetOffsetLength(currentCurve, height));

			realCurve -= curveAngle;

			a++;
		}

		joinPoint = vertices2D [(verticeCount / 2) - 1];

//		vertices2D [6] = new Vector2 (vertices2D [5].x, vertices2D [5].y - height);
//		vertices2D [7] = new Vector2 (vertices2D [4].x, vertices2D [4].y - height);
//		vertices2D [8] = new Vector2 (vertices2D [3].x, vertices2D [3].y - height);
//		vertices2D [9] = new Vector2 (vertices2D [2].x, vertices2D [2].y - height);
//		vertices2D [10] = new Vector2 (vertices2D [1].x, vertices2D [1].y - height);
//		vertices2D [11] = new Vector2 (vertices2D [0].x, vertices2D [0].y - height);
		

		// Use the triangulator to get indices for creating triangles
		Triangulator tr = new Triangulator(vertices2D);
		int[] indices = tr.Triangulate();

		// Create the Vector3 vertices
		Vector3[] vertices = new Vector3[vertices2D.Length];
		for (int i=0; i<vertices.Length; i++) {
			vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
		}

		// Create the mesh
		Mesh msh = new Mesh();
		msh.vertices = vertices;
		msh.triangles = indices;
		msh.RecalculateNormals();
		msh.RecalculateBounds();

		// Set up game object with mesh;

		Vector2[] uvs = new Vector2[vertices.Length];

		for (int i=0; i < uvs.Length; i++) {

			uvs[i] = new Vector2(vertices[i].x, vertices[i].y);
		}
		msh.uv = uvs;

		newCurveMesh = msh;
	}

	float GetOffsetHeight(float angle, float hypotenuse)
	{
		return hypotenuse * Mathf.Sin (angle * Mathf.Deg2Rad);
	}

	float GetOffsetLength(float angle, float hypotenuse)
	{
		return hypotenuse * Mathf.Cos (angle * Mathf.Deg2Rad);
	}

	float GetOppositeSide(float angle, float adjacent)
	{
		return adjacent * Mathf.Tan (angle * Mathf.Deg2Rad);
	}
}
