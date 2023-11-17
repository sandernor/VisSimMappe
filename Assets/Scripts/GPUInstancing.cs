using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using JetBrains.Annotations;

public class GPUInstancing : MonoBehaviour
{
    [SerializeField] private Mesh _mesh;
    [SerializeField] private Material _material;
    private Matrix4x4[] _matrices;

    private Vector3[] _positions;
    private RenderParams _rp;

    private List<string> line;
    private int lineCount = 0;
    private List<Vector3> vertexArray;
    private Vector3 v;
    private float xMax = 583200f;
    private float zMax = 6673100f;
    private float yMax = 500f;
    private int numVertices;
    private int multi = 150;
    private float scale = 3f;

    private void Awake()
    {
        line = new List<string>();
        vertexArray = new List<Vector3>();
    }

    private void Start()
    {
        FileToLines();

        numVertices = int.Parse(line[0]);
        _positions = new Vector3[(numVertices / multi) + multi - 1];
        _matrices = new Matrix4x4[_positions.Length];
        _rp = new RenderParams(_material);

        GenerateVertices();
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
            //Debug.Log("Linje " + i + " : " + line[i]);
        }
    }

    private void GenerateVertices()
    {
        for (int i = 0; i < numVertices; i += multi)
        {
            string[] splitLine = line[i + 1].Split(char.Parse(" "));

            float x, y, z;
            x = float.Parse(splitLine[0]);
            z = float.Parse(splitLine[1]);
            y = float.Parse(splitLine[2]);

            if (y > 505f)
            {
                v = new Vector3(x - xMax, y - yMax, z - zMax);
                Debug.Log("i = " + i / multi);
                _positions[i / multi] = v;
            }
        }
    }

    private void Update()
    {
        for (var i = 0; i < _positions.Length; i++)
        {
            _matrices[i].SetTRS(_positions[i], Quaternion.identity, new Vector3(scale, scale, scale));
        }

        Graphics.RenderMeshInstanced(_rp, _mesh, 0, _matrices);
    }
}