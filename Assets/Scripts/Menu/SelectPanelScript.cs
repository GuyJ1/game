using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class SelectPanelScript : MonoBehaviour
{
    //for relative position
    RectTransform rt;

    //variablez
    private float scrollSpeed = 0f;
    int tPos;

    public Button Button1;
    public Button Button2;
    public Button Button3;
    public Button Button4;

    // Update is called once per frame
    void Update()
    {
        //fetch rect transform info during each frame
        rt = GetComponent<RectTransform>();

        //pseudo-scroll rect. Making this by scratch in order to maybe make it compatible with a selection arrow.
        //scroll up
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && rt.offsetMin.y > -85f)
        {
            scrollSpeed = -250f;
            //transform.Translate(0, -1f * scrollSpeed * Time.deltaTime, 0, Space.World);
        }
        //scroll down
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && rt.offsetMin.y < 210f)
        {
            scrollSpeed = 250f;
            //transform.Translate(0, scrollSpeed * Time.deltaTime, 0, Space.World);
        }

        //translate panel each frame
        if (scrollSpeed != 0f)
        {
            transform.Translate(0, scrollSpeed * Time.deltaTime, 0, Space.World);
            scrollSpeed = scrollSpeed > 0f ? scrollSpeed - 0.8f : scrollSpeed + 0.8f;
            if (Math.Abs(scrollSpeed) < 1f || rt.offsetMin.y <= -85f || rt.offsetMin.y >= 210f)
                scrollSpeed = 0f;

        }

        tPos = (int)rt.offsetMin.y;
        switch (tPos)
        {
            case int tPos when (tPos > 128):
                Button4.Select();
                //Button1.DoStateTransition(SelectionState.Highlighted, false);
                break;
            case int tPos when (tPos <= 128 && tPos > 55):
                Button3.Select();
                //Button2.DoStateTransition(SelectionState.Highlighted, false);
                break;
            case int tPos when (tPos <= 55 && tPos > -16):
                Button2.Select(); 
                //Button3.DoStateTransition(SelectionState.Highlighted, false);
                break;
            case int tPos when (tPos <= 16):
                Button1.Select();
                //Button4.DoStateTransition(SelectionState.Highlighted, false);
                break;
        }

    }
}
