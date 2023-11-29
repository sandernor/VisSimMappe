using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UIElements;

public class Raindrop : MonoBehaviour
{
    private float g = -9.81f;
    private float m = 0.5f;
    private float G;
    private float F = 0.01f;

    private Vector3[] ControlPoints;
    private int round = 1;

    private int startTri;
    private int n1;
    private int n2;
    private int n3;
    private int tri;

    private Vector3 curVel;
    private Vector3 newVel;
    private Vector3 acceleration;
    private Vector3 newPos;
    private Vector3 prevPos;
    private Vector3 N;
    private Vector3 prevN;

    //public GameObject triGen;
    public TerrainGen genScript;
    public BSpline SplineScript;
    public GameObject BSpline;

    private Vector3 pos;
    float time;
    bool check = false;

    RaycastHit hit;

    private void Awake()
    {
        G = m * g;
        curVel = new Vector3(0f, G, 0f);
        acceleration = new Vector3(0f, G, 0f);
        //genScript = triGen.GetComponent("TerrainGen") as TerrainGen;
        //splineScript = gameObject.GetComponent("BSpline") as BSpline;

        ControlPoints = new Vector3[3];
    }

    private void Start()
    {
        tri = WhatTriStart();
        if (tri < 0 ) Destroy(gameObject);
        ControlPoints[0] = transform.position;
    }

    private void FixedUpdate()
    {
        time += Time.fixedDeltaTime;

        if (time > 1f)
        {
            ControlPoints[round]= transform.position;
            round += 1;
            if (round > 2)
            {
                GameObject BSplineInst = Instantiate(BSpline, new Vector3(0, 0, 0), Quaternion.identity);
                SplineScript = BSplineInst.GetComponent("BSpline") as BSpline;
                SplineScript.P0 = ControlPoints[0];
                SplineScript.P1 = ControlPoints[1];
                SplineScript.P2 = ControlPoints[2];
                SplineScript.DrawQuadraticBSpline();
                ControlPoints[0] = transform.position;
                round = 1;
            }

            time = 0f;
        }

        if ( !check )
        {
            if (Grounded())
            {
                check = true;
            }
            else
            {
                newVel = curVel + acceleration * Time.fixedDeltaTime;
                newPos = newVel * Time.fixedDeltaTime;
                curVel = newVel;
                transform.position += newPos;
            }
        }


        if (check)
        {
            transform.position += calcPos();
        }

        if (transform.position.y < -100f) 
        {
            Destroy(gameObject);
        }
        //Debug.Log(curVel.ToString());
    }

    private Vector3 calcPos()
    {
        //Debug.Log(WhatTri());
        tri = WhatTri();

        curVel.x = curVel.x * (1f - F);
        curVel.z = curVel.z * (1f - F);

        newVel = curVel + acceleration * Time.fixedDeltaTime;

        if (tri > -1)
        {
            N = Vector3.Dot(Normal(genScript.tris[tri].vertices[0], genScript.tris[tri].vertices[1], genScript.tris[tri].vertices[2]), -newVel) * Normal(genScript.tris[tri].vertices[0], genScript.tris[tri].vertices[1], genScript.tris[tri].vertices[2]);
            prevN = N;

        }
        //Debug.Log("2 : " + newVel);
        //Debug.DrawRay(transform.position, N * 2000f, Color.red);
        if (tri > -1)
        {
            if (Grounded())
            {
                newVel = newVel + N;
                //Debug.Log("1 : " + newVel);
            }
        }
        
        newPos = newVel * Time.fixedDeltaTime;
        curVel = newVel;
        return newPos;
    }

    private bool Grounded()
    {
        Vector3 P = transform.position;
        float r = 0.5f;
        if (tri < 0) return false;
        Vector3 C = genScript.tris[tri].vertices[0];
        Vector3 norm = Normal(genScript.tris[tri].vertices[0], genScript.tris[tri].vertices[1], genScript.tris[tri].vertices[2]);
        Vector3 y = Vector3.Dot(P - C, norm) * norm;
        if (y.magnitude <= r)
        {
            return true;
        }
        else
            return false;
    }

    private int WhatTri()
    {
        if (tri > -1)
        {
            n1 = genScript.tris[tri].neighbours[0];
            n2 = genScript.tris[tri].neighbours[1];
            n3 = genScript.tris[tri].neighbours[2];

            if (inTri(genScript.tris[tri].vertices[0], genScript.tris[tri].vertices[1], genScript.tris[tri].vertices[2], transform.position))
            {
                return tri;
            }
            if (n1 > -1)
            {
                if (inTri(genScript.tris[n1].vertices[0], genScript.tris[n1].vertices[1], genScript.tris[n1].vertices[2], transform.position))
                {
                    return n1;
                }
            }
            if (n2 > -1)
            {
                if (inTri(genScript.tris[n2].vertices[0], genScript.tris[n2].vertices[1], genScript.tris[n2].vertices[2], transform.position))
                {
                    return n2;
                }
            }
            if (n3 > -1)
            {
                if (inTri(genScript.tris[n3].vertices[0], genScript.tris[n3].vertices[1], genScript.tris[n3].vertices[2], transform.position))
                {
                    return n3;
                }
            }
        }

        for (int i = 0; i < genScript.tris.Length; i++)
        {
            if (inTri(genScript.tris[i].vertices[0], genScript.tris[i].vertices[1], genScript.tris[i].vertices[2], transform.position))
            {
                Debug.Log(i);
                return i;
            }
            else continue;
        }
        return -1;
    }

    private int WhatTriStart()
    {
        for (int i = 0; i < genScript.tris.Length; i++)
        {
            if (inTriStart(genScript.tris[i].vertices[0], genScript.tris[i].vertices[1], genScript.tris[i].vertices[2], transform.position))
            {
                Debug.Log(i);
                return i;
            }
            else continue;
        }
        return -1;
    }

    public bool inTriStart(Vector3 a, Vector3 b, Vector3 c, Vector3 p)
    {
        Vector2 A = new Vector2(a.x, a.z);
        Vector2 B = new Vector2(b.x, b.z);
        Vector2 C = new Vector2(c.x, c.z);
        Vector2 P = new Vector2(p.x, p.z);

        double s1 = C.y - A.y;
        double s2 = C.x - A.x;
        double s3 = B.y - A.y;
        double s4 = P.y - A.y;

        double w1 = (A.x * s1 + s4 * s2 - P.x * s1) / (s3 * s2 - (B.x - A.x) * s1);
        double w2 = (s4 - w1 * s3) / s1;

        return w1 >= 0 && w2 >= 0 && (w1 + w2) <= 1;
    }

    public bool inTri(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
    {
        Vector3 u = B - A;
        Vector3 v = C - A;
        Vector3 w = P - A;

        Vector3 vCrossW = Vector3.Cross(v, w);
        Vector3 vCrossU = Vector3.Cross(v, u);

        if (Vector3.Dot(vCrossW, vCrossU) < 0)
            return false;

        Vector3 uCrossW = Vector3.Cross(u, w);
        Vector3 uCrossV = Vector3.Cross(u, v);

        if (Vector3.Dot(uCrossW, uCrossV) < 0)
            return false;

        float denom = uCrossV.magnitude;
        float r = vCrossW.magnitude / denom;
        float t = uCrossW.magnitude / denom;

        return (r >= 0 && t >= 0 && r + t <= 1);
    }

    public Vector3 baryCoords(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
    {
        Vector3 u = B - A;
        Vector3 v = C - A;
        Vector3 w = P - A;

        Vector3 vCrossW = Vector3.Cross(v, w);
        Vector3 vCrossU = Vector3.Cross(v, u);

        Vector3 uCrossW = Vector3.Cross(u, w);
        Vector3 uCrossV = Vector3.Cross(u, v);

        Vector3 result = new Vector3();
        float denom = uCrossV.magnitude;
        result.y = vCrossW.magnitude / denom;
        result.z = uCrossW.magnitude / denom;
        result.x = 1f - result.y - result.z;

        return result;
    }

    public Vector3 Normal(Vector3 A, Vector3 B, Vector3 C)
    {
        Vector3 u = B - A;
        Vector3 v = C - A;

        Vector3 norm = Vector3.Normalize(Vector3.Cross(u, v));

        return norm;
    }
}
