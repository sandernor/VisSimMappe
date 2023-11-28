using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using JetBrains.Annotations;

public class TerrainGen : MonoBehaviour
{
    [SerializeField] private Material _material;
    private Matrix4x4[] matrix;

    private Vector3[] _positions;
    private RenderParams _rp;

    private Mesh mesh;

    private List<string> line;
    private int lineCount = 0;
    public Vector3[] vertexArray;
    private Vector2[] uvs;
    private Vector3 v;

    private int numVertices;
    private int multi = 1000;
    private float scale = 1f;

    private float xMax = 0f;
    private float zMax = 0f;
    private float xMin = 100000000f;
    private float zMin = 100000000f;

    private Vector3 v0;
    private Vector3 v1;
    private Vector3 v2;
    private Vector3 v3;

    private float xMax2 = 583600f;
    private float zMax2 = 6673700f;
    private float yMax2 = 600f;

    private float Res = 25f;

    private float[] xArr;
    private float[] zArr;
    private int[] triVerts;
    private int[] triVerts2;

    private int n;
    private int m;
    private int gridSize;
    private int triangles;

    public Triangle[] tris;

    public struct Triangle
    {
        public int index;

        public int x;
        public int y;
        public int z;

        public int[] neighbours;

        public Vector3[] vertices;

        public Triangle(int i, int x, int y, int z)
        {
            this.x = x; this.y = y; this.z = z; this.index = i;

            vertices = new Vector3[3];

            for (int j = 0; j < vertices.Length; j++)
            {
                vertices[j] = new Vector3(0, 0, 0);
            }

            neighbours = new int[3];
            for (int j = 0; j < neighbours.Length; j++)
            {
                neighbours[j] = -1;
            }
        }
    }

    private void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        line = new List<string>();

        FileToLines();

        numVertices = int.Parse(line[0]);
        _positions = new Vector3[(numVertices / multi) + multi - 1];
        matrix = new Matrix4x4[1];
        _rp = new RenderParams(_material);

        GenerateGrid();

        var j = gridSize - n - (m - 1);
        vertexArray = new Vector3[j];
        //Debug.Log(vertexArray.Length);
        uvs = new Vector2[j];
        triangles = 2 * (gridSize - (2 * n) - (2 * (m - 2)));
        triVerts = new int[triangles * 3];
        triVerts2 = new int[triangles * 3];
        tris = new Triangle[triangles];

        GenerateVerts();
        GenerateMesh();
        MeshUpdate();
    }

    private void Start()
    {
        //FileToLines();

        //numVertices = int.Parse(line[0]);
        //_positions = new Vector3[(numVertices / multi) + multi - 1];
        //matrix = new Matrix4x4[1];
        //_rp = new RenderParams(_material);

        //GenerateGrid();

        //var j = gridSize - n - (m - 1);
        //vertexArray = new Vector3[j];
        ////Debug.Log(vertexArray.Length);
        //uvs = new Vector2[j];
        //triangles = 2 * (gridSize - (2 * n) - (2 * (m - 2)));
        //triVerts = new int[triangles * 3];
        //triVerts2 = new int[triangles * 3];
        //tris = new Triangle[triangles];

        //GenerateVerts();
        //GenerateMesh();
        //MeshUpdate();
    }

    private void FileToLines()
    {
        System.IO.StreamReader file = new System.IO.StreamReader(Application.dataPath + "/merged.txt");

        while (file.ReadLine() != null)
        {
            lineCount += 1;
        }

        System.IO.StreamReader file2 = new System.IO.StreamReader(Application.dataPath + "/merged.txt");

        for (int i = 0; i < lineCount; i++)
        {
            line.Add(file2.ReadLine());
        }
    }

    private void GenerateGrid()
    {
        for (int i = 0; i < numVertices; i++)
        {
            string[] splitLine = line[i + 1].Split(char.Parse(" "));

            float x, y, z;
            x = float.Parse(splitLine[0]);
            z = float.Parse(splitLine[1]);
            y = float.Parse(splitLine[2]);

            if (x > xMax)
            {
                xMax = x;
            }
            if (z > zMax)
            {
                zMax = z;
            }
            if (x < xMin)
            {
                xMin = x;
            }
            if (z < zMin)
            {
                zMin = z;
            }
        }

        v0 = new Vector3(xMax, 0.0f, zMax);
        v1 = new Vector3(xMax, 0.0f, zMin);
        v2 = new Vector3(xMin, 0.0f, zMax);
        v3 = new Vector3(xMin, 0.0f, zMin);

        n = Mathf.CeilToInt((xMax - xMin) / Res);
        xArr = new float[n]; // + 1 ?

        m = Mathf.CeilToInt((zMax - zMin) / Res);
        zArr = new float[m]; // + 1 ?

        for (int i = 0; i < n; i++)
        {
            if (i != n - 1)
                xArr[i] = xMin + (Res * i) - xMax2;
            else
                xArr[i] = xMax - xMax2;
        }

        for (int i = 0; i < m; i++)
        {
            if (i != m - 1)
                zArr[i] = zMin + (Res * i) - zMax2;
            else
                zArr[i] = zMax - zMax2;
        }

        gridSize = n * m;
    }

    private void GenerateVerts()
    {
        int index = 0;
        float yV = 0;
        float prev = 0;

        List<Vector3> points4;
        points4 = new List<Vector3>();

        for (int k = 0; k < n - 1; k++)
        {
            for (int l = 0; l < m - 1; l++)
            {
                for (int i = 0; i < numVertices; i += multi)
                {
                    string[] splitLine = line[i + 1].Split(char.Parse(" "));

                    float x, y, z;
                    x = float.Parse(splitLine[0]);
                    z = float.Parse(splitLine[1]);
                    y = float.Parse(splitLine[2]);

                    v = new Vector3(x - xMax2, y - yMax2, z - zMax2);


                    if (v.x > xArr[k] && v.x < xArr[k + 1] && v.z > zArr[l] && v.z < zArr[l + 1])
                    {
                        points4.Add(v);
                    }
                }

                for (int i = 0; i < points4.Count; i++)
                {
                    yV += points4[i].y;
                }
                yV = points4.Count > 0 ? yV / points4.Count : prev;
                //Debug.Log(yV);

                if (yV == 0)
                {
                    yV = prev;
                }
                vertexArray[index] = new Vector3(xArr[k] + (Res / 2), yV, zArr[l] + (Res / 2));

                index++;
                points4.Clear();
                prev = yV;
                yV = 0;
            }
        }
    }

    private void GenerateMesh()
    {
        int z = m - 1;
        int x = n - 1;
        int y = 0;

        for (int i = 0; i < triangles / 2; i++)
        {
            if (i == z * (y + 1) - (y + 1))
            {
                y += 1;
            }

            tris[i * 2] = new Triangle(i * 2, i + y, i + 1 + y, z + i + 1 + y);
            tris[i * 2 + 1] = new Triangle(i * 2 + 1, i + y, z + i + 1 + y, z + i + y);

            triVerts[i * 6] = i + y;
            triVerts[i * 6 + 1] = i + 1 + y;
            triVerts[i * 6 + 2] = z + i + 1 + y;

            triVerts[i * 6 + 3] = i + y;
            triVerts[i * 6 + 4] = z + i + 1 + y;
            triVerts[i * 6 + 5] = z + i + y;

            //Debug.Log((i + y) + " : " + (i + 1 + y) + " : " + (z + i + 1 + y));
            //Debug.Log((i + y) + " : " + (i + z + y + 1) + " : " + (z + i + y));
        }

        for (int i = 0; i < triangles / 2; i++)
        {
            tris[i * 2].neighbours[0] = i * 2 + 1;

            if (i * 2 != (((z - 1) * 2) - 2) && i * 2 + 2 % (z - 1) * 2 != 0)
                tris[i * 2].neighbours[1] = i * 2 + 3;
            else
                tris[i * 2].neighbours[1] = -1;

            if (i * 2 - (((z - 1) * 2) - 1)! < 0)
                tris[i * 2].neighbours[2] = i * 2 - (((z - 1) * 2) - 1);
            else
                tris[i * 2].neighbours[2] = -1;



            tris[i * 2 + 1].neighbours[0] = i * 2;

            if (i * 2 + 1 !< 0 && i * 2 - 1 % (z - 1) * 2 != 0)
                tris[i * 2 + 1].neighbours[1] = i * 2 + 1 - 3;
            else
                tris[i * 2 + 1].neighbours[1] = -1;

            if ((i * 2 + 1) + (((z - 1) * 2) - 1) !> triangles - 1)
                tris[i * 2 + 1].neighbours[2] = (i * 2 + 1) + (((z - 1) * 2) - 1);
            else
                tris[i * 2 + 1].neighbours[2] = -1;
        }

        //for (int i = 0; i < tris.Length * 3; i += 3)
        //{
        //    triVerts[i] = tris[i - ((i / 3) * 2)].x;
        //    triVerts[i + 1] = tris[i - ((i / 3) * 2)].y;
        //    triVerts[i + 2] = tris[i - ((i / 3) * 2)].z;
        //}

        for (int i = 0; i < tris.Length; i++)
        {
            tris[i].vertices[0] = vertexArray[triVerts[i * 3]];
            tris[i].vertices[1] = vertexArray[triVerts[i * 3 + 1]];
            tris[i].vertices[2] = vertexArray[triVerts[i * 3 + 2]];

            Debug.Log(tris[i].vertices[0] + " : " + tris[i].vertices[1] + " : " + tris[i].vertices[2]);
        }
    }

    private Vector3[] CalcVNormals()
    {
        Vector3[] vNormals = new Vector3[vertexArray.Length];

        for (int i = 0; i < triVerts.Length / 3; i++)
        {
            int index = i * 3;
            int vertIndex1 = triVerts[index];
            int vertIndex2 = triVerts[index + 1];
            int vertIndex3 = triVerts[index + 2];

            Vector3 triNorm = CalcSurfNorm(vertIndex1, vertIndex2, vertIndex3);
            vNormals[vertIndex1] = triNorm;
            vNormals[vertIndex2] = triNorm;
            vNormals[vertIndex3] = triNorm;
        }

        for (int i = 0; i < vNormals.Length; i++)
        {
            vNormals[i].Normalize();
        }

        return vNormals;
    }

    private Vector3 CalcSurfNorm(int a, int b, int c)
    {
        Vector3 vertA = vertexArray[a];
        Vector3 vertB = vertexArray[b];
        Vector3 vertC = vertexArray[c];

        Vector3 AB = vertB - vertA;
        Vector3 AC = vertC - vertA;
        return Vector3.Cross(AB, AC).normalized;
    }

    private void MeshUpdate()
    {
        for (int i = 0; i < vertexArray.Length; i++)
        {
            uvs[i] = new Vector2((vertexArray[i].x / (xMax - xMin)) * Res, (vertexArray[i].z / (zMax - zMin)) * Res);
            //vertexArray[i] *= 0.5f;   
        }
        mesh.Clear();
        mesh.vertices = vertexArray;
        mesh.triangles = triVerts;
        mesh.uv = uvs;
        mesh.normals = CalcVNormals();

    }
}