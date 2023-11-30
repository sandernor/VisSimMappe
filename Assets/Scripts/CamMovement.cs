using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    //Movement
    float x;
    float y;
    float z;
    public float speed = 5;

    //Camera
    float camX;
    float camY;
    [SerializeField] float sens = 500;
    public GameObject cam;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        x = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        z = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        y = Input.GetAxis("Jump") * Time.deltaTime * speed;
        transform.Translate(x, y, z);

        camX = Input.GetAxis("Mouse X") * Time.deltaTime * sens;
        camY = Input.GetAxis("Mouse Y") * Time.deltaTime * sens;
        transform.Rotate(0, camX, 0);
        cam.transform.Rotate(-camY, 0, 0);
        transform.Translate(x, y, z);
    }
}
