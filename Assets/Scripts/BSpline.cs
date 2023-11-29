using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSpline : MonoBehaviour
{
    public Vector3 P0;
    public Vector3 P1;
    public Vector3 P2;

    public List<Vector3> ControlPoints;
    public int numberOfPoints = 10;

    private float time = 0f;

    private LineRenderer lineRenderer;

    void Start()
    {



    }

    public void DrawQuadraticBSpline()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = numberOfPoints;

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

        return p;
    }

    private void Update()
    {
        //time += Time.deltaTime;

        //Debug.Log(ControlPoints.Count);

        //if (time > 1f)
        //{
        //    ControlPoints.Add(transform.position);
        //    time = 0f;
        //}

        //if (ControlPoints.Count > 2 && ((ControlPoints.Count - 1) % 2) == 0)
        //{
        //    P0 = ControlPoints[ControlPoints.Count - 3];
        //    P1 = ControlPoints[ControlPoints.Count - 2];
        //    P2 = ControlPoints[ControlPoints.Count - 1];
        //    DrawQuadraticBSpline();
        //}
    }
}