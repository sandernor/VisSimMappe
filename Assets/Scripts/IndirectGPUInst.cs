using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndirectGPUInst : MonoBehaviour
{
    [SerializeField] private Mesh mesh;
    [SerializeField] private Material mat;

    private List<string> line;
    private int lineCount = 0;

    private Vector3[] _positions;
    private Vector3 v;
    private float xMax = 583200f;
    private float zMax = 6673300f;
    private float yMax = 400f;

    private readonly uint[] _args = { 0, 0, 0, 0, 0 };
    private ComputeBuffer _argsBuffer;
    private int count;

    private ComputeBuffer _positionBuffer1;// _positionBuffer2;
    private Vector4[] positions1;


    [SerializeField] public Color[] ColorArray;


    private void Awake()
    {
        line = new List<string>();
        ColorArray = new Color[3] {Color.red, Color.green, Color.blue};
    }
    private void Start()
    {
        FileToLines();
        count = int.Parse(line[0]);

        positions1 = new Vector4[count];

        _positionBuffer1?.Release();
        _positionBuffer1 = new ComputeBuffer(count, 16);


        GenerateVertices();

        _argsBuffer = new ComputeBuffer(1, _args.Length * sizeof(uint), ComputeBufferType.IndirectArguments);
        UpdateBuffers();
    }

    private void FileToLines()
    {
        System.IO.StreamReader file = new System.IO.StreamReader(Application.dataPath + "/xyzCoords - Copy.txt");

        while (file.ReadLine() != null)
        {
            lineCount += 1;
        }

        System.IO.StreamReader file2 = new System.IO.StreamReader(Application.dataPath + "/xyzCoords - Copy.txt");

        for (int i = 0; i < lineCount; i++)
        {
            line.Add(file2.ReadLine());
        }
    }

    private void GenerateVertices()
    {
        for (int i = 0; i < count; i++)
        {
            string[] splitLine = line[i + 1].Split(char.Parse(" "));

            float x, y, z;
            x = float.Parse(splitLine[0]);
            z = float.Parse(splitLine[1]);
            y = float.Parse(splitLine[2]);

            v = new Vector3(x - xMax, y - yMax, z - zMax);
            positions1[i] = v;
        }
    }

    private void Update()
    {
        Graphics.DrawMeshInstancedIndirect(mesh, 0, mat, new Bounds(Vector3.zero, Vector3.one * 1000), _argsBuffer);
    }

    private void OnDisable()
    {
        _positionBuffer1?.Release();
        _positionBuffer1 = null;

        //_positionBuffer2?.Release();
        //_positionBuffer2 = null;

        _argsBuffer?.Release();
        _argsBuffer = null;
    }

    private void UpdateBuffers()
    {
        // Positions
        //_positionBuffer1?.Release();
        //////_positionBuffer2?.Release();
        //_positionBuffer1 = new ComputeBuffer(count, 16);

        //// Positions
        //_positionBuffer1?.Release();
        ////_positionBuffer2?.Release();
        //_positionBuffer1 = new ComputeBuffer(count, 16);
        ////_positionBuffer2 = new ComputeBuffer(count, 16);

        //var positions1 = new Vector3[count];
        ////var positions2 = new Vector4[count];

        //for (var i = 0; i < count; i++)
        //{
        //    positions1[i] = _positions[i];
        //}

        //// Grouping cubes into a bunch of spheres
        //var offset = Vector3.zero;
        //var batchIndex = 0;
        //var batch = 0;
        //for (var i = 0; i < count; i++)
        //{
        //    var dir = Random.insideUnitSphere.normalized;
        //    positions1[i] = dir * Random.Range(10, 15) + offset;
        //    positions2[i] = dir * Random.Range(30, 50) + offset;

        //    positions1[i].w = Random.Range(-3f, 3f);
        //    positions2[i].w = batch;

        //    if (batchIndex++ == 250000)
        //    {
        //        batchIndex = 0;
        //        batch++;
        //        offset += new Vector3(90, 0, 0);
        //    }
        //}

        _positionBuffer1.SetData(positions1);
        //_positionBuffer2.SetData(positions2);
        mat.SetBuffer("position_buffer_1", _positionBuffer1);
        //mat.SetBuffer("position_buffer_2", _positionBuffer2);
        mat.SetColorArray("color_buffer", ColorArray);

        // Verts
        _args[0] = mesh.GetIndexCount(0);
        _args[1] = (uint)count;
        _args[2] = mesh.GetIndexStart(0);
        _args[3] = mesh.GetBaseVertex(0);

        _argsBuffer.SetData(_args);
    }
}
