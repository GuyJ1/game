using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    // Materials
    [SerializeField] Material unselected, highlighted, selected;

    // Things used for click detection
    private Camera cam;
    private Renderer rend;
    private Ray ray;
    private RaycastHit hit;

    // Position and Flags
    [SerializeField] public uint x, y;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(name + " has been created");

        cam = Camera.main;
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // If mouse down
        if (Input.GetMouseButtonDown(0))
        {
            // Cast ray from screen point to mouse pos
            ray = cam.ScreenPointToRay(Input.mousePosition);

            // Use physics to detect a "collision" from a ray
            if (Physics.Raycast(ray, out hit, 1000f))
            {
                rend.sharedMaterial = selected;
                Debug.Log(name + " has been clicked");
            }
        }
    }
}
