using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    public float height;
    // Start is called before the first frame update
    void Awake()
    {
        height = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, height, transform.position.z);
    }
}
