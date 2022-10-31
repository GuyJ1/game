using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonControl : MonoBehaviour
{
    public GameObject cannonball;
    public GameObject muzzleFlash;

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
        // ----- Muzzle Flash -----
        GameObject mFlash = Instantiate(muzzleFlash);
        mFlash.transform.position = transform.GetChild(0).transform.position;

        // ----- Cannonball Spawning -----
        GameObject newCannonball = Instantiate(cannonball, transform.GetChild(0).transform);
        var rb = newCannonball.GetComponent<Rigidbody>();

        float randX = Random.value * 3.0f;
        float randY = Random.value * 3.0f;
        float randZ = Random.value * 2.0f;

        rb.velocity = new Vector3(30.0f + randX, 1.5f + randY, -1.0f + randZ);
    }
}
