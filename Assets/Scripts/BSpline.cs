using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSpline : MonoBehaviour
{
    public Vector3 P0;
    public Vector3 P1;
    public Vector3 P2;

    public int tri;
    public int prevTri;

    public List<Vector3> ControlPoints;
    public int numberOfPoints = 10;

    private float time = 0f;

    private LineRenderer lineRenderer;
    public TerrainGen genScript;
    public GameObject PointGen;

    void Start()
    {
        //PointGen = GameObject.Find("PointGen");
        //genScript = PointGen.GetComponent("TerrainGen") as TerrainGen;

    }

    public void DrawQuadraticBSpline()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numberOfPoints;

        //P0.y = -200f; P1.y = -200f; P2.y = -200f;

        for (int i = 0; i < numberOfPoints; i++)
        {
            float t = i / (float)(numberOfPoints - 1);
            Vector3 position = CalculateQuadraticBSplinePoint(t);
            lineRenderer.SetPosition(i, position);
        }
    }

    Vector3 CalculateQuadraticBSplinePoint(float t)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * P0; // (1-t)^2 * p0
        p += 2 * u * t * P1; // 2 * (1-t) * t * p1
        p += tt * P2; // t^2 * p2

        //if (tri > -1)
        //{
        //    Vector3 barycentricCoords = baryCoords(genScript.tris[tri].vertices[0], genScript.tris[tri].vertices[1], genScript.tris[tri].vertices[2], p);
        //    float height = barycentricCoords.x * genScript.tris[tri].vertices[0].y + barycentricCoords.y * genScript.tris[tri].vertices[1].y + barycentricCoords.z * genScript.tris[tri].vertices[2].y;
        //    p.y = height;
        //}

        //if (genScript == null)
        //{
        //    Debug.Log("script");
        //}
        //if (genScript.tris = null)
        //{
        //    Debug.Log("array");
        //}
        //Debug.Log(tri);
        //Debug.Log(genScript.tris[tri]);
        //Debug.Log(genScript.tris[tri].vertices[1]);
        //Debug.Log(genScript.tris[tri].vertices[2]);

        //Vector3 barycentricCoords = baryCoords(genScript.tris[tri].vertices[0], genScript.tris[tri].vertices[1], genScript.tris[tri].vertices[2], p);
        //float height = barycentricCoords.x * genScript.tris[tri].vertices[0].y + barycentricCoords.y * genScript.tris[tri].vertices[1].y + barycentricCoords.z * genScript.tris[tri].vertices[2].y;
        //p.y = height;

        //prevTri = tri;

        return p;
    }

    public Vector3 baryCoords(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
    {
        Vector2 a = new Vector2(A.x, A.z);
        Vector2 b = new Vector2(B.x, B.z);
        Vector2 c = new Vector2(C.x, C.z);
        Vector2 p = new Vector2(P.x, P.z);

        float s1 = c.y - a.y;
        float s2 = c.x - a.x;
        float s3 = b.y - a.y;
        float s4 = p.y - a.y;

        float w1 = (A.x * s1 + s4 * s2 - P.x * s1) / (s3 * s2 - (B.x - A.x) * s1);
        float w2 = (s4 - w1 * s3) / s1;

        Vector3 result = new Vector3();
        result.y = w1;
        result.z = w2;
        result.x = 1f - w1 - w2;

        return result;
    }

    private void Update()
    {

    }
}