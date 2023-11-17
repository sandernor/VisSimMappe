using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using JetBrains.Annotations;

public class LowPoint : MonoBehaviour
{
    private Vector3[] _positions;
    private RenderParams _rp;
    private List<string> line;
    private int lineCount = 0;
    private List<Vector3> vertexArray;
    private Vector3 v;
    private float lowV = 1000f;
    private int numVertices;


    private void Awake()
    {
        line = new List<string>();
        vertexArray = new List<Vector3>();
    }

    private void Start()
    {
        FileToLines();
        numVertices = int.Parse(line[0]);
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
        for (int i = 0; i < numVertices; i++)
        {
            string[] splitLine = line[i + 1].Split(char.Parse(" "));

            float y;
            y = float.Parse(splitLine[2]);
            if (y < lowV)
                lowV = y;
        }
        Debug.Log("Lowest point = " + lowV);
    }
}