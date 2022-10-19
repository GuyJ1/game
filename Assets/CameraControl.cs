using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    // Control Speed
    [SerializeField] public float ControlSpeed; // WASD Speed

    // Viewing offset
    private Vector3 offset;

    // Position camera is looking at
    private Vector3 target;

    private bool controlling = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set offset to it's starting position
        offset = transform.position;
        target = offset;
    }
    
    void Update()
    {
        float xDirection = Input.GetAxis("Horizontal");
        float zDirection = Input.GetAxis("Vertical");

        if (xDirection != 0 || zDirection != 0)
        {
            Vector3 moveVector = new Vector3(xDirection, 0.0f, zDirection);
            transform.position += moveVector * ControlSpeed;
            target = transform.position;

            controlling = true;
        }
        else
        {
            controlling = false;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (controlling == false)
        {
            Vector3 diff = (target - transform.position) / 64;
            transform.position += diff;
        }
    }

    public void LookAtPos(Vector3 pos)
    {
        target = pos + offset;
    }
}
