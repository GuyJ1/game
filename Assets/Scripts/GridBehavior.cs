/// @author: Bryson Squibb
/// @date: 10/04/2022
/// @description: this script spawns a grid of tile objects, then
/// it provides an interface to interact with said spawned grid

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehavior : MonoBehaviour
{
    [SerializeField] public uint width, height; // Width and height of the grid
    [SerializeField] public int tilesize; // Size of each tile

    // Grid vars
    public GameObject Tile; // A cell or tile that makes up the grid
    public GameObject [,] grid; // The actual grid, represented by a 2d array
    private uint availableTiles;

    // Things used for click detection
    private Camera cam;
    private Renderer rend;
    private Ray ray;
    private RaycastHit hit;
    private int layerMask = 1 << 6;

    // Selection Management
    private bool tileSelected = false;
    private Vector2Int selectedTilePos;

    // Materials
    [SerializeField] Material unselected, highlighted, selected;

    // Temp
    public GameObject characterToSpawn;
    
    // Start is called before the first frame update
    void Start()
    {
        // Declare the grid to appropriate width and height
        grid = new GameObject[width, height];

        // Generate the grid by instantiating tile objects
        GenerateGrid();

        // Get camera
        cam = Camera.main;

        // Spawn characters for testing purposes
        SpawnCharacter(characterToSpawn, new Vector2Int(0,0));
        SpawnCharacter(characterToSpawn);
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
                // Get data from tile hit
                var objectHit   = hit.transform;
                var selectRend  = objectHit.GetComponent<Renderer>();
                var tileScript  = objectHit.GetComponent<TileScript>();
                Vector2Int tilePos = tileScript.position;

                // Case 1: if there's no selected tile, then just select this one
                if (tileSelected == false)
                {
                    selectRend.material = selected;
                    selectedTilePos = tilePos;
                    Debug.Log(objectHit.name + " has been selected");
                    tileSelected = true;
                }
                // Case 2: if the tile is the same as the already selected,
                // tile then unselect it
                else if (tilePos == selectedTilePos)
                {
                    selectRend.material = unselected;
                    Debug.Log(objectHit.name + " has been unselected");
                    tileSelected = false;
                }
                // Case 3: if the tile is different than the currently selected tile,
                // then deselect the current tile and select the new one
                else
                {
                    // Unselect currently select tile
                    Debug.Log("Unselecting tile " + selectedTilePos.x + " " + selectedTilePos.y);
                    grid[selectedTilePos.x, selectedTilePos.y].GetComponent<Renderer>().material = unselected;

                    // Select new one
                    selectRend.material = selected;
                    selectedTilePos = tilePos;
                    Debug.Log(objectHit.name + " has been selected");
                    tileSelected = true;
                }
            }
        }
    }

    // Create a grid object for every tile position
    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate the position of the tile to create
                Vector3 pos = new Vector3(transform.position.x + x*tilesize, transform.position.y, transform.position.z + y*tilesize);

                // Create the tile
                GameObject newTile = Instantiate(Tile, pos, transform.rotation, this.transform);

                // Set the data of the newly created tile
                newTile.name = "Tile " + x + " " + y;
                newTile.GetComponent<TileScript>().position.x = x;
                newTile.GetComponent<TileScript>().position.y = y;

                // Link the new tile to a position on the grid
                grid[x, y] = newTile;
            }
        }

        // Tiles valid for spawning characters
        availableTiles = width*height;
    }

    void SpawnCharacter(GameObject character)
    {
        GameObject spawningTile;
        GameObject tilesCharacter;
        bool characterSpawned = false;
        int randX;
        int randY;
        
        // Warning: potential issue if the grid is full + performance issue if
        // grid is almost full
        if (availableTiles > 0)
        {
            while (characterSpawned == false)
            {
                // Get a random tile on the grid
                randX = Random.Range(0, (int)width);
                randY = Random.Range(0, (int)height);
                spawningTile = grid[randX, randY];

                // Get data from tile object
                var tilesScript = spawningTile.GetComponent<TileScript>();
                tilesCharacter = tilesScript.characterOn;

                // Check whether the tile already has a character on it
                if (tilesScript.hasCharacter == false)
                {
                    // Spawn the character if valid
                    Vector3 pos = new Vector3(transform.position.x + randX*tilesize, transform.position.y, transform.position.z + randY*tilesize);
                    Debug.Log("Spawning character as position " + randX + " " + randY);

                    // Set tile data
                    tilesCharacter = Instantiate(character, pos, transform.rotation, this.transform);
                    tilesScript.hasCharacter = true;

                    // Update flag and # of available tiles
                    availableTiles--;
                    characterSpawned = true;
                }
            }
        }
        else
        {
            Debug.Log("Grid spawnCharacter(): Error! Couldn't spawn character since there are no available tiles");
        }
    }

    void SpawnCharacter(GameObject character, Vector2Int spawnPos)
    {
        GameObject spawningTile;
        GameObject tilesCharacter;
        
        // Warning: potential issue if the grid is full + performance issue if
        // grid is almost full
        if (availableTiles > 0)
        {
            // Get the respective tile on the grid
            spawningTile = grid[spawnPos.x, spawnPos.y];

            // Get data from tile object
            var tilesScript = spawningTile.GetComponent<TileScript>();
            tilesCharacter = tilesScript.characterOn;

            // Check whether the tile already has a character on it
            if (tilesScript.hasCharacter == false)
            {
                // Spawn the character if valid
                Vector3 pos = new Vector3(transform.position.x + spawnPos.x*tilesize, transform.position.y, transform.position.z + spawnPos.y*tilesize);
                Debug.Log("Spawning character as position " + spawnPos.x + " " + spawnPos.y);

                // Set tile data
                tilesCharacter = Instantiate(character, pos, transform.rotation, this.transform);
                tilesScript.hasCharacter = true;

                // Update flag and # of available tiles
                availableTiles--;
            }
            else
            {
                Debug.Log("Grid spawnCharacter(): Error! Can't spawn a character on top of another character");
            }
        }
        else
        {
            Debug.Log("Grid spawnCharacter(): Error! Couldn't spawn character since there are no available tiles");
        }
    }
}
