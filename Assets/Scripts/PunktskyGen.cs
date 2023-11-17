using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using JetBrains.Annotations;

public class TriangleGen : MonoBehaviour
{
    //private static string path = "Assets/VertexData.txt";

    //public TextAsset vertexData = Resources.Load<TextAsset>(path);
    private List<string> line;
    private int lineCount = 0;
    private List<Vector3> vertexArray;
    private Vector3 v;
    public GameObject quad;
    private float xMax = 583100f;
    private float zMax = 6672500f;
    private float yMax = 400f;

    private void Awake()
    {
        line = new List<string>();
        vertexArray = new List<Vector3>();
    }


    private void FileToLines()
    {
        System.IO.StreamReader file = new System.IO.StreamReader(Application.dataPath + "/verts.txt");

        while (file.ReadLine() != null)
        {
            lineCount += 1;
        }

        System.IO.StreamReader file2 = new System.IO.StreamReader(Application.dataPath + "/verts.txt");

        for (int i = 0; i < lineCount; i++)
        {
            line.Add(file2.ReadLine());
            //Debug.Log("Linje " + i + " : " + line[i]);
        }
    }

    private void GenerateVertices()
    {
        int numVertices = int.Parse(line[0]);
        for (int i = 1; i < numVertices + 1; i++)
        {
            string[] splitLine = line[i].Split(char.Parse(" "));

            float x, y, z;
            x = float.Parse(splitLine[0]);
            z = float.Parse(splitLine[1]);
            y = float.Parse(splitLine[2]);

            v = new Vector3(x-xMax, y-yMax, z-zMax);
            Instantiate(quad, v, Quaternion.identity);

            //Debug.Log(v.position);

        }
    }

    private void Start()
    {
        FileToLines();
        GenerateVertices();
    }
}
