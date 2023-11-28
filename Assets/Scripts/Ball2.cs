
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Ball2 : MonoBehaviour
{
    private float g = -9.81f;
    private float m = 0.2f;
    private float G;

    private int startTri;
    private int n1;
    private int n2;
    private int n3;
    private int tri = -2;
    private int prevTri;

    private Vector3 curVel;
    private Vector3 newVel;
    private Vector3 acceleration;
    private Vector3 newPos;
    private Vector3 prevPos;
    private Vector3 N;
    private Vector3 prevN;

    public GameObject triGen;
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
        genScript = triGen.GetComponent("TerrainGen") as TerrainGen;

    }

    private void Start()
    {
        //startTri = WhatTriStart();
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
        tri = WhatTri();

        newVel = curVel + acceleration * Time.fixedDeltaTime;

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
        float r = 0.5f;
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
            if (n1 != -1)
            {
                if (inTri(genScript.tris[n1].vertices[0], genScript.tris[n1].vertices[1], genScript.tris[n1].vertices[2], transform.position))
                {
                    return n1;
                }
            }
            if (n2 != -1)
            {
                if (inTri(genScript.tris[n2].vertices[0], genScript.tris[n2].vertices[1], genScript.tris[n2].vertices[2], transform.position))
                {
                    return n2;
                }
            }
            if (n3 != -1)
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

    //private int WhatTriStart()
    //{
    //    //Debug.Log(genScript.triCount);
    //    for (int i = 0; i < genScript.tris.Length; i++)
    //    {
    //        if (inTri(genScript.tris[i].vertices[0], genScript.tris[i].vertices[1], genScript.tris[i].vertices[2], transform.position))
    //        {
    //            Debug.Log(i);
    //            return i;
    //        }
    //        else continue;
    //    }
    //    return -1;
    //}

    //private int WhatTri()
    //{
    //    if (inTri(genScript.tris[startTri].vertices[0], genScript.tris[startTri].vertices[1], genScript.tris[startTri].vertices[2], transform.position))
    //    {
    //        return startTri;
    //    }
    //    else
    //    {
    //        n1 = genScript.tris[startTri].neighbours[0];
    //        n2 = genScript.tris[startTri].neighbours[1];
    //        n3 = genScript.tris[startTri].neighbours[2];

    //        if (n1 != -1)
    //        {
    //            if (inTri(genScript.tris[n1].vertices[0], genScript.tris[n1].vertices[1], genScript.tris[n1].vertices[2], transform.position))
    //            {
    //                startTri = n1;
    //                return n1;
    //            }
    //        }
    //        if (n2 != -1)
    //        {
    //            if (inTri(genScript.tris[n2].vertices[0], genScript.tris[n2].vertices[1], genScript.tris[n2].vertices[2], transform.position))
    //            {
    //                startTri = n2;
    //                return n2;
    //            }
    //        }
    //        if (n3 != -1)
    //        {
    //            if (inTri(genScript.tris[n3].vertices[0], genScript.tris[n3].vertices[1], genScript.tris[n3].vertices[2], transform.position))
    //            {
    //                startTri = n3;
    //                return n3;
    //            }
    //        }
    //    }


    //    for (int i = 0; i < genScript.tris.Length; i++)
    //    {
    //        if (inTri(genScript.tris[i].vertices[0], genScript.tris[i].vertices[1], genScript.tris[i].vertices[2], transform.position))
    //        {
    //            startTri = i;
    //            return i;
    //        }
    //        else continue;
    //    }
    //    return -1;
    //}

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
