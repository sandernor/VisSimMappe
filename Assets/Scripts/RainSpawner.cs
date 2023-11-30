using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainSpawner : MonoBehaviour
{
    Vector2 genBox;
    private int prodRate;
    public GameObject raindrop;
    public GameObject triGen;
    public Raindrop rainScript;


    void Start()
    {
        genBox = new Vector2(400, 700);
        prodRate = 100;

        for (int i = 0; i < prodRate; i++)
        {
            GameObject raindropInst = Instantiate(raindrop, new Vector3(Random.Range(150f, genBox.x), 60f, Random.Range(100f, genBox.y)), Quaternion.identity);
            rainScript = raindropInst.GetComponent("Raindrop") as Raindrop;
            rainScript.genScript = triGen.GetComponent("TerrainGen") as TerrainGen;
        }

    }

}
