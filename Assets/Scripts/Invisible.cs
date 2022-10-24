using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invisible : MonoBehaviour
{
    [SerializeField] private bool visible = false;

    public Renderer rend;

    // Make this game object invisible on start
    void Start()
    {
        rend = GetComponent<Renderer>();

        if (visible == false)
        {
            rend.enabled = false;
        }
    }

    // Toggle visibility
    public void SetVisibility(bool isVisible)
    {
        if (isVisible)
        {
            rend.enabled = true;
        }
        else
        {
            rend.enabled = false;
        }
    }
}
