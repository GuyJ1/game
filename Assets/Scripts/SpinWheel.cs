using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpinWheel : MonoBehaviour
{
    //public bool isRotating = false;
    private float speedofWheel = 0f;
    //public bool isActive = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (speedofWheel != 0f)
        {
            transform.Rotate(0f, 0f, speedofWheel * Time.deltaTime, Space.Self);
            speedofWheel -= 0.05f;
            if (speedofWheel < 1f)
            {
                //isRotating = false;
                speedofWheel = 0f;
            }
        }
            
    }
   
    //update speed from 0, set isROtating to true
    public void SetWheel()
    {
        speedofWheel = 50f;
        //isRotating = true;
    }

    //stop the wheel, reset everything
    public void StopWheel()
    {
        speedofWheel = 0f;
        //isRotating = false;
    } 

}
