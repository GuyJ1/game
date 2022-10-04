using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehavior : MonoBehaviour
{
    [SerializeField] public uint width, height; // Width and height of the grid
    [SerializeField] public int tilesize; // Size of each tile

    public GameObject Tile; // A cell or tile that makes up the grid
    public GameObject [,] grid; // The actual grid, represented by a 2d array

    // Things used for click detection
    private Camera cam;
    private Renderer rend;
    private Ray ray;
    private RaycastHit hit;
    private int layerMask = 1 << 6;

    // Materials
    [SerializeField] Material unselected, highlighted, selected;
    
    // Start is called before the first frame update
    void Start()
    {
        // Declare the grid to appropriate width and height
        grid = new GameObject[width, height];

        // Generate the grid by instantiating tile objects
        GenerateGrid();

        // Get camera
        cam = Camera.main;
    }

    // Select tiles using the mouse
    void Update()
    {
        // If mouse down
        if (Input.GetMouseButtonDown(0))
        {
            // Cast ray from screen point to mouse pos
            ray = cam.ScreenPointToRay(Input.mousePosition);

            // Use physics to detect a "collision" from a ray
            if (Physics.Raycast(ray, out hit, 1000f, layerMask) == true)
            {
                // Get the tile hit
                var objectHit = hit.transform;

                // Select the tile hit
                var selectRend = objectHit.GetComponent<Renderer>();
                selectRend.material = selected;
                Debug.Log(objectHit.name + " has been selected");
            }
        }
    }

    void GenerateGrid()
    {
        for (uint x = 0; x < width; x++)
        {
            for (uint y = 0; y < height; y++)
            {
                // Calculate the position of the tile to create
                Vector3 pos = new Vector3(transform.position.x + x*tilesize, transform.position.y, transform.position.z + y*tilesize);

                // Create the tile
                GameObject newTile = Instantiate(Tile, pos, transform.rotation, this.transform);

                // Set the data of the newly created tile
                newTile.name = "Tile " + x + " " + y;
                newTile.GetComponent<TileScript>().x = x;
                newTile.GetComponent<TileScript>().y = y;
                
                // Debug msg
                Debug.Log("Instantiating " + newTile.name);

                // Link the new tile to a position on the grid
                grid[x, y] = newTile;
            }
        }
    }
}
