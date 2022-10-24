using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectPanelScript : MonoBehaviour
{
    //for relative position
    RectTransform rt;

    // Update is called once per frame
    void Update()
    {
        //fetch rect transform info during each frame
        rt = GetComponent<RectTransform>();

        //pseudo-scroll rect. Making this by scratch in order to maybe make it compatible with a selection arrow.
        //scroll up
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && rt.offsetMin.y > -85f)
        {
            transform.Translate(0, -5000 * Time.deltaTime, 0, Space.World);
        }
        //scroll down
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && rt.offsetMin.y < 210f)
        {
            transform.Translate(0, 5000 * Time.deltaTime, 0, Space.World);
        } 
    }
}
