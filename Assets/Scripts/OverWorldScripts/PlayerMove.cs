using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameObject BOAT;
    public float zAxis;
    public float xAxis;
    public float yAxis;
    public float originalYAxis;
    public bool FloatUp = true;
    public GameObject destination;
    public static GameObject destination2;
    public bool travel = false;

    // Start is called before the first frame update
    void Start()
    {
        originalYAxis = 1;
        BOAT = this.gameObject;
        zAxis = BOAT.transform.position.z;
        xAxis = BOAT.transform.position.x;
        yAxis = originalYAxis;
        //originalYAxis = yAxis;
        BOAT.transform.position = new Vector3(xAxis, originalYAxis, zAxis);
        StartCoroutine(floating());
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(floating());
        BOAT.transform.position = new Vector3(xAxis, yAxis, zAxis);
        if (Input.GetMouseButtonDown(0))
        {
            if (travel != true)
            {
                if (destination != null)
                {
                    destination2 = NodeClick.GetClickObj();
                    if (destination == destination2)
                    {
                        travel = true;
                        StartCoroutine(Sail());
                    }
                    else
                    {
                        travel = false;
                    }
                }
                destination = NodeClick.GetClickObj();
            }
        }
    }

    public IEnumerator Sail()
    {
        float distanceX = xAxis-destination.transform.position.x;
        float distanceZ = zAxis-destination.transform.position.z;        
        float velocity = 3f;
        Vector3 look = new Vector3(destination.transform.position.x, BOAT.transform.position.y, destination.transform.position.z);

        while (travel == true)
        {
            yield return new WaitForSeconds(0.0f);
            BOAT.transform.LookAt(look);
            BOAT.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            
            if (distanceX < -0.05f && xAxis < destination.transform.position.x) 
            { 
                xAxis += velocity*Time.deltaTime; 
            }
            if (distanceX > 0.05f && xAxis > destination.transform.position.x)
            {
                xAxis -= velocity * Time.deltaTime;
            }
            if (distanceZ < -0.05f && zAxis < destination.transform.position.z)
            {
                zAxis += velocity *Time.deltaTime;
            }
            if (distanceZ > 0.05f && zAxis > destination.transform.position.z)
            {
                zAxis -= velocity * Time.deltaTime;
            }
            if(distanceZ < 0.05f && distanceZ > -0.05f && distanceX <0.05f && distanceX > -0.05f)
            {
            zAxis = destination.transform.position.z;
            xAxis = destination.transform.position.x;
            travel = false;
            }
            distanceX = xAxis - destination.transform.position.x;
            distanceZ = zAxis - destination.transform.position.z;

        }

    }

    public IEnumerator floating()
    {
        yield return new WaitForSeconds(0.5f);
        if (FloatUp == true)
        {
            yAxis += (0.25f * Time.deltaTime);
            if (yAxis > (originalYAxis + 0.25f))
            {

                FloatUp = false;
            }
        }
        if (FloatUp == false)
        {
            yAxis -= (0.25f * Time.deltaTime);
            if (yAxis < (originalYAxis - .25f))
            {               
                FloatUp = true;
            }
        }
    }

}