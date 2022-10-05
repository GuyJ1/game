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
    private bool charSelected = false;
    private bool charHighlighted = false;
    private Vector2Int selectedCharPos;
    private Vector2Int highlightedCharPos;

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
        SpawnCharacter(characterToSpawn);
        SpawnCharacter(characterToSpawn);
        SpawnCharacter(characterToSpawn);

        SpawnCharacter(characterToSpawn);
        SpawnCharacter(characterToSpawn);
        SpawnCharacter(characterToSpawn);
        SpawnCharacter(characterToSpawn);
        SpawnCharacter(characterToSpawn);
    }

    // Select and move characters using the mouse
    void Update()
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

            if (tileScript.hasCharacter)
            {
                // ----------------------------
                // Character and Tile Selection
                // ----------------------------
                if (Input.GetMouseButtonDown(0))
                {
                    if (tileScript.hasCharacter)
                    {
                        // Case 1: if there's no selected character, then just select this one
                        if (charSelected == false)
                        {
                            selectRend.material = selected;
                            selectedCharPos = tilePos;
                            Debug.Log("Character on " + objectHit.name + " has been selected");
                            charSelected = true;
                            charHighlighted = false;
                        }
                        // Case 2: if the character is the same as the already selected,
                        // then unselect it
                        else if (tilePos == selectedCharPos)
                        {
                            selectRend.material = unselected;
                            Debug.Log("Character on " + objectHit.name + " has been unselected");
                            charSelected = false;
                        }
                        // Case 3: if the character is different than the currently selected character,
                        // then deselect the current character and select the new one
                        else
                        {
                            // Unselect currently select character
                            Debug.Log("Unselecting character on " + selectedCharPos.x + " " + selectedCharPos.y);
                            grid[selectedCharPos.x, selectedCharPos.y].GetComponent<Renderer>().material = unselected;

                            // Select new one
                            charHighlighted = false;
                            selectRend.material = selected;
                            selectedCharPos = tilePos;
                            Debug.Log("Character on " + objectHit.name + " has been selected");
                            charSelected = true;
                            charHighlighted = false;
                        }
                    }
                }
                else
                // -------------------------------
                // Character and Tile Highlighting
                // -------------------------------
                {
                    // Selected tiles cannot be highlighted
                    bool okayToHighlight = true;

                    if (charSelected && selectedCharPos == tilePos)
                    {
                        okayToHighlight = false;
                    }

                    if (okayToHighlight)
                    {
                        // Case 1: if there's no highlighted character, then just select this one
                        if (charHighlighted == false)
                        {
                            selectRend.material = highlighted;
                            highlightedCharPos = tilePos;
                            charHighlighted = true;
                        }
                        // Case 2: if there's already a highlighted character, then switch the highlighting
                        else
                        {
                            // Unhighlight currently select character
                            grid[highlightedCharPos.x, highlightedCharPos.y].GetComponent<Renderer>().material = unselected;

                            // Highlight new one
                            selectRend.material = highlighted;
                            highlightedCharPos = tilePos;
                            charHighlighted = true;
                        }
                    }
                }
            }
            else {UnhighlightChar();}
        }
        else {UnhighlightChar();}
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
                    Vector3 pos = new Vector3(transform.position.x + randX*tilesize, transform.position.y+0.5f, transform.position.z + randY*tilesize);
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
                Vector3 pos = new Vector3(transform.position.x + spawnPos.x*tilesize, transform.position.y+0.5f, transform.position.z + spawnPos.y*tilesize);
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

    private void UnhighlightChar()
    {
        if (charHighlighted)
        {
            grid[highlightedCharPos.x, highlightedCharPos.y].GetComponent<Renderer>().material = unselected;
            charHighlighted = false;
        }
    }
}
