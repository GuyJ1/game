using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpinWheel : MonoBehaviour
{
    private float speedofWheel = 0f;
    //private bool isReverse = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        //default state is speed = 0, otherwise update speed
        if (speedofWheel != 0f)
        {
            //rotate by current speed, decrease speed by 0.05 each frame
            //to guarantee the wheel stops, set to 0 if it goes below 1
            transform.Rotate(0f, 0f, speedofWheel * Time.deltaTime, Space.Self);
            speedofWheel -= 0.05f;
            if (speedofWheel < 1f)
            {
                speedofWheel = 0f;
            }
        }
            
    }
   
    //update speed from 0
    public void SetWheel()
    {
        speedofWheel = 50f;
    }

    //stop the wheel / reset
    public void StopWheel()
    {
        speedofWheel = 0f;
    } 

}
