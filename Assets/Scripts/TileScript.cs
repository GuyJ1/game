using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    // Position and Flags
    [SerializeField] public uint x, y;

    // Start is called before the first frame update
    void Start()
    {
        // Debug msg
        Debug.Log(name + " has been created");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
