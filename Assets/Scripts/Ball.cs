
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Ball : MonoBehaviour
{
    private float g = -9.81f;
    private float m = 0.1f;
    private float G;

    private int startTri;
    private int n1;
    private int n2;
    private int n3;
    private int tri;

    private List<int> triangles;
    private List<int> used;

    private Vector3 curVel;
    private Vector3 newVel;
    private Vector3 acceleration;
    private Vector3 newPos;
    private Vector3 prevPos;
    private Vector3 N;
    private Vector3 prevN;

    public GameObject pointGen;
    private TerrainGen genScript;

    private Vector3 pos;
    float time;
    bool check = false;

    RaycastHit hit;

    private void Awake()
    {
        G = m * g;
        curVel = new Vector3(0f, G, 0f);
        acceleration = new Vector3(0f, G, 0f);
        genScript = pointGen.GetComponent("TerrainGen") as TerrainGen;

        triangles = new List<int>();
        used = new List<int>();

    }

    private void Start()
    {
        startTri = WhatTriStart();
    }
    private void FixedUpdate()
    {
        time += Time.fixedDeltaTime;

        transform.position += calcPos();

        //if (time > 2f && check == false)
        //{
        //    Debug.Log("NormalVektor : " + Vector3.Dot(Normal(genScript.tris[WhatTri()].vertices[0], genScript.tris[WhatTri()].vertices[1], genScript.tris[WhatTri()].vertices[2]), -newVel) * Normal(genScript.tris[WhatTri()].vertices[0], genScript.tris[WhatTri()].vertices[1], genScript.tris[WhatTri()].vertices[2]) + "Akselerasjon : " + acceleration + "Hastighet : " + newVel + "Posisjon i trekant : " + baryCoords(genScript.tris[WhatTri()].vertices[0], genScript.tris[WhatTri()].vertices[1], genScript.tris[WhatTri()].vertices[2], transform.position));
        //    check = true;
        //}
        //Debug.Log(curVel.ToString());
    }

    private Vector3 calcPos()
    {
        //Debug.Log(WhatTri());
        newVel = curVel + acceleration * Time.fixedDeltaTime;

        if (startTri != -1)
            tri = WhatTri();
        else tri = -1;

        if (tri != -1)
        {
            N = Vector3.Dot(Normal(genScript.tris[tri].vertices[0], genScript.tris[tri].vertices[1], genScript.tris[tri].vertices[2]), -newVel) * Normal(genScript.tris[tri].vertices[0], genScript.tris[tri].vertices[1], genScript.tris[tri].vertices[2]);
            prevN = N;

        }
        //Debug.Log("2 : " + newVel);
        Debug.DrawRay(transform.position, N * 2000f, Color.red);
        if (tri != -1)
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
        float r = 0.02f;
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

    private int WhatTriStart()
    {
        //Debug.Log(genScript.triCount);
        for (int i = 0; i < genScript.tris.Length; i++)
        {
            if (inTri(genScript.vertexArray[genScript.tris[i].x], genScript.vertexArray[genScript.tris[i].y], genScript.vertexArray[genScript.tris[i].z], transform.position))
            {
                //Debug.Log(i);
                return i;
            }
            else continue;
        }
        return -1;
    }

    private int WhatTri()
    {
        //Debug.Log(genScript.tris[startTri].vertices[0] + " : " + genScript.tris[startTri].vertices[1] + " : " + genScript.tris[startTri].vertices[2]);
        //Debug.Log(startTri);
        if (inTri(genScript.vertexArray[genScript.tris[startTri].x], genScript.vertexArray[genScript.tris[startTri].y], genScript.vertexArray[genScript.tris[startTri].z], transform.position))
        {
            return startTri;
        }

        //n1 = genScript.tris[startTri].neighbours[0];
        //n2 = genScript.tris[startTri].neighbours[1];
        //n3 = genScript.tris[startTri].neighbours[2];

        int nTri = -1;
        triangles.Add(startTri);

        while (nTri == -1)
        {
            if (triangles.Count < 1)
            {
                return -1;
            }
            //Debug.Log("Tris : " + triangles.Count + "Used : " + used.Count);
            if (used.Count > 0)
            {
                for (int i = 0; i < triangles.Count; i++)
                {
                    for (int j = 0; j < used.Count; j++)
                    {
                        //Debug.Log("Tris : " + triangles.Count + "Used : " + used.Count);
                        if (triangles[i] == used[j])
                        {
                            triangles.RemoveAt(i);
                        }
                    }
                }
            }


            n1 = genScript.tris[triangles[0]].neighbours[0];
            n2 = genScript.tris[triangles[0]].neighbours[1];
            n3 = genScript.tris[triangles[0]].neighbours[2];

            used.Add(triangles[0]);

            nTri = neighbourCheck();

            if (nTri != -1)
            {
                startTri = nTri;
                triangles.Clear();
                used.Clear();
                return startTri;
            }
        }

        return -1;
    }

    private int neighbourCheck()
    {
        triangles.RemoveAt(0);

        if (n1 != -1)
        {
            if (inTri(genScript.vertexArray[genScript.tris[startTri].x], genScript.vertexArray[genScript.tris[startTri].y], genScript.vertexArray[genScript.tris[startTri].z], transform.position))
            {
                return n1;
            }
            triangles.Add(n1);
        }
        if (n2 != -1)
        {
            if (inTri(genScript.vertexArray[genScript.tris[startTri].x], genScript.vertexArray[genScript.tris[startTri].y], genScript.vertexArray[genScript.tris[startTri].z], transform.position))
            {
                return n2;
            }
            triangles.Add(n2);
        }
        if (n3 != -1)
        {
            if (inTri(genScript.vertexArray[genScript.tris[startTri].x], genScript.vertexArray[genScript.tris[startTri].y], genScript.vertexArray[genScript.tris[startTri].z], transform.position))
            {
                return n3;
            }
            triangles.Add(n3);
        }

        return -1;
    }

    public bool inTri(Vector3 A, Vector3 B, Vector3 C, Vector3 P)
    {
        Vector3 u = B - A;
        Vector3 v = C - A;
        Vector3 w = P - A;

        Vector3 vCrossW = Vector3.Cross(v, w);
        Vector3 vCrossU = Vector3.Cross(v, u);

        //if (Vector3.Dot(vCrossW, vCrossU) < 0)
        //    return false;

        Vector3 uCrossW = Vector3.Cross(u, w);
        Vector3 uCrossV = Vector3.Cross(u, v);

        //if (Vector3.Dot(uCrossW, uCrossV) < 0)
        //    return false;

        float denom = uCrossV.magnitude;
        float r = vCrossW.magnitude / denom;
        float t = uCrossW.magnitude / denom;

        return (r >= 0 && t >= 0 && r + t <= 1);

        //double s1 = C.z - A.z;
        //double s2 = C.x - A.x;
        //double s3 = B.z - A.z;
        //double s4 = P.z - A.z;

        //double w1 = (A.x * s1 + s4 * s2 - P.x * s1) / (s3 * s2 - (B.x - A.x) * s1);
        //double w2 = (s4 - w1 * s3) / s1;
        //return w1 >= 0 && w2 >= 0 && (w1 + w2) <= 1;
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
