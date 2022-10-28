using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonControl : MonoBehaviour
{
    public GameObject cannonball;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            fireCannonball();
        }
    }

    public void fireCannonball()
    {
        GameObject newCannonball = Instantiate(cannonball, transform);
        var rb = newCannonball.GetComponent<Rigidbody>();

        float randX = Random.value * 20.0f;
        float randY = Random.value * 20.0f;
        float randZ = Random.value * 20.0f;

        rb.velocity = new Vector3(40.0f + randX, 40.0f + randY, randZ);
    }
}
