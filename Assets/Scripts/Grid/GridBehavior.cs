/// @author: Bryson Squibb
/// @date: 10/08/2022
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
    [SerializeField] Material unselected, highlighted, selected, impassible;

    // Temp
    public GameObject characterToSpawn;
    
    // Start is called before the first frame update
    void Start()
    {
        // Generate the grid by instantiating tile objects
        GenerateGrid(1);

        // Get camera
        cam = Camera.main;

        // Spawn characters for testing purposes
        SpawnCharacter(characterToSpawn, new Vector2Int(0,0));
        SpawnCharacter(characterToSpawn);
        SpawnCharacter(characterToSpawn);
        SpawnCharacter(characterToSpawn);
        SpawnCharacter(characterToSpawn);
    }

    // --------------------------------------------------------------
    // @desc: Highlight, select and move characters using the mouse
    // --------------------------------------------------------------
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

            // ----------------------------
            // Character Selection and Movement
            // ----------------------------
            if (Input.GetMouseButtonDown(0))
            {
                // If a tile has a character on it, then we can only select it
                if (tileScript.hasCharacter)
                {
                    // Case 1: if there's no selected character, then just select this one
                    if (charSelected == false)
                    {
                        // Select new tile at tile pos
                        HighlightValidMoves(tilePos, tileScript.characterOn.GetComponent<CharacterStats>().MV);
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
                        ResetAllHighlights();
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

                        // Select new tile at tile pos (Note: code is repeated above, not good practice)
                        HighlightValidMoves(tilePos, tileScript.characterOn.GetComponent<CharacterStats>().MV);
                        selectRend.material = selected;
                        selectedCharPos = tilePos;
                        Debug.Log("Character on " + objectHit.name + " has been selected");
                        charSelected = true;
                        charHighlighted = false;
                    }
                }
                // If we have a selected character/tile, then we can move it to any
                // highlighted tile without a character already on it
                else
                {
                    if (charSelected && tilePos != selectedCharPos)
                    {
                        // Unselect currently select character
                        Debug.Log("Unselecting character on " + selectedCharPos.x + " " + selectedCharPos.y);
                        grid[selectedCharPos.x, selectedCharPos.y].GetComponent<Renderer>().material = unselected;
                        charSelected = false;
                        charHighlighted = false;

                        // Move character
                        if (MoveCharacterOnTile(selectedCharPos, tilePos, true) == false)
                        {
                            ResetAllHighlights();
                            Debug.Log("Cannot move character to tile " + tilePos.x + " " + tilePos.y);
                        }
                    }
                }
            }
            else
            // -------------------------------
            // Character and Tile Highlighting
            // -------------------------------
            {
                if (charSelected == false)
                {
                    bool okayToHighlight = true;

                    // Prevent highlighting the tile w/ a selected
                    // character on it
                    if (charSelected && selectedCharPos == tilePos)
                    {
                        okayToHighlight = false;
                    }

                    // Cannot highlight tiles without a character on it
                    if (tileScript.hasCharacter == false)
                    {
                        okayToHighlight = false;
                    }

                    // If the mouse moved away from the highlighted tile,
                    // then unhighlight it
                    if (charHighlighted && highlightedCharPos != tilePos)
                    {
                        grid[highlightedCharPos.x, highlightedCharPos.y].GetComponent<Renderer>().material = unselected;
                    }

                    // Highlight mouse over tile if it's okay to highlight it
                    if (okayToHighlight)
                    {
                        selectRend.material = highlighted;
                        highlightedCharPos = tilePos;
                        charHighlighted = true;
                    }
                }
            }
        }
        else {UnhighlightChar();}
    }

    // --------------------------------------------------------------
    // @desc: Create a grid object for every tile position
    // --------------------------------------------------------------
    void GenerateGrid(int gridMap = 0)
    {
        // Set Grid Map
        GridMaps.SelectGrid(gridMap);

        // Set dimensions
        width = GridMaps.GetGridWidth();
        height = GridMaps.GetGridHeight();

        // Declare the grid to appropriate width and height
        grid = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate the position of the tile to create
                Vector3 pos = new Vector3(transform.position.x + x*tilesize, transform.position.y, transform.position.z + y*tilesize);

                // Create the tile
                GameObject newTile = Instantiate(Tile, pos, transform.rotation);

                // Set the data of the newly created tile
                newTile.name = "Tile " + x + " " + y;
                var newTileScript = newTile.GetComponent<TileScript>();
                newTileScript.position.x = x;
                newTileScript.position.y = y;

                // Check the grid map for impassible tiles
                if (GridMaps.GetFlagAtPos(new Vector2Int(x, y)))
                {
                    newTileScript.passable = false;
                    newTile.GetComponent<Renderer>().material = impassible;
                }

                // Link the new tile to a position on the grid
                grid[x, y] = newTile;
            }
        }

        // Tiles valid for spawning characters
        availableTiles = width*height;
    }
    // --------------------------------------------------------------
    // @desc: Create a grid object for every tile position
    // @arg: character - the character prefab object to spawn
    // --------------------------------------------------------------
    void SpawnCharacter(GameObject character)
    {
        GameObject spawningTile;
        bool characterSpawned = false;
        int randX;
        int randY;
        
        // Check if there's a tile available
        if (availableTiles > 0)
        {
            // Warning: potential issue if the grid is full + performance issue if
            // grid is almost full
            while (characterSpawned == false)
            {
                // Get a random tile on the grid
                randX = Random.Range(0, (int)width);
                randY = Random.Range(0, (int)height);
                spawningTile = grid[randX, randY];

                // Get data from tile object
                var tilesScript = spawningTile.GetComponent<TileScript>();

            // Check whether the tile already has a character on it
            // + whether the tile is a passable tile
            if (tilesScript.hasCharacter == false && tilesScript.passable)
                {
                    // Spawn the character if valid
                    Vector3 pos = new Vector3(transform.position.x + randX*tilesize, transform.position.y+0.5f, transform.position.z + randY*tilesize);
                    Debug.Log("Spawning character as position " + randX + " " + randY);

                    // Set tile data
                    tilesScript.characterOn = Instantiate(character, pos, transform.rotation);
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

    // --------------------------------------------------------------
    // @desc: Create a grid object for every tile position
    // @arg: character - the character prefab object to spawn
    // @arg: spawnPos  - the logical grid position to spawn on
    // --------------------------------------------------------------
    void SpawnCharacter(GameObject character, Vector2Int spawnPos)
    {
        GameObject spawningTile;
        
        // Check if there's a tile available
        if (availableTiles > 0)
        {
            // Get the respective tile on the grid
            spawningTile = grid[spawnPos.x, spawnPos.y];

            // Get data from tile object
            var tilesScript = spawningTile.GetComponent<TileScript>();

            // Check whether the tile already has a character on it
            // + whether the tile is a passable tile
            if (tilesScript.hasCharacter == false && tilesScript.passable)
            {
                // Spawn the character if valid
                Vector3 pos = new Vector3(transform.position.x + spawnPos.x*tilesize, transform.position.y+0.5f, transform.position.z + spawnPos.y*tilesize);
                Debug.Log("Spawning character as position " + spawnPos.x + " " + spawnPos.y);

                // Set tile data
                tilesScript.characterOn = Instantiate(character, pos, transform.rotation);
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

    // If a character is highlighted, unhighlight it
    private void UnhighlightChar()
    {
        if (charHighlighted)
        {
            grid[highlightedCharPos.x, highlightedCharPos.y].GetComponent<Renderer>().material = unselected;
            charHighlighted = false;
        }
    }

    public bool MoveCharacterOnTile(Vector2Int sourcePos, Vector2Int destPos, bool onlyHighlighted)
    {
        GameObject charToMove = null;
        bool moveSuccess = false;

        // Check whether tiles are in range
        if (TilePosInRange(sourcePos) && TilePosInRange(destPos))
        {
            // Get tile on source position
            GameObject sourceTile = grid[sourcePos.x, sourcePos.y];
            var sourceTileScipt = sourceTile.GetComponent<TileScript>();

            // Get tile on dest position
            GameObject destTile = grid[destPos.x, destPos.y];
            var destTileScript = destTile.GetComponent<TileScript>();

            // Get character on source tile
            if (sourceTileScipt.hasCharacter)
            {
                // Only move to highlighted tiles
                if (onlyHighlighted && destTileScript.highlighted)
                {
                    Debug.Log("Moving character to tile " + destPos.x + " " + destPos.y);
                    charToMove = sourceTileScipt.characterOn;

                    // Move character to destPos
                    Vector3 pos = new Vector3(destTile.transform.position.x, destTile.transform.position.y+0.5f, destTile.transform.position.z);
                    charToMove.transform.position = pos;

                    // Set source tile data
                    sourceTileScipt.hasCharacter = false;
                    sourceTileScipt.characterOn = null;

                    // Set destination tile data
                    destTileScript.hasCharacter = true;
                    destTileScript.characterOn = charToMove;

                    ResetAllHighlights();
                    moveSuccess = true;
                }
            }
            else
            {
                Debug.Log("MoveCharacterOnTile: Error! source tile does not have a character");
            }
        }
        else
        {
            Debug.Log("MoveCharacterOnTile: Error! tile source or dest position is out of range");
        }
        
        return moveSuccess;
    }

    // Checks whether a logical tile position is in the grid
    private bool TilePosInRange(Vector2Int pos)
    {
        bool inRange = false;

        if (pos.x >= 0 && pos.x <= width-1)
        {
            if (pos.y >= 0 && pos.y <= height-1)
            {
                inRange = true;
            }
        }

        return inRange;
    }

    // Highlights available move tiles from a provided position and range
    private void HighlightValidMoves(Vector2Int pos, int range)
    {
        ResetAllHighlights();
        GetAllPathsFromTile(GetTileAtPos(pos), range);
    }

    private PathTreeNode GetAllPathsFromTile(GameObject tile, int range)
    {
        // Create root node
        PathTreeNode root = new PathTreeNode();
        root.myTile = tile;
        root.tileRange = range;

        // Temp vars
        PathTreeNode tempNode;

        // Create queue
        Queue<PathTreeNode> queue = new Queue<PathTreeNode>();
        queue.Enqueue(root);

        // Populate the tree based on the range value
        // this must be done in levelorder traversal
        while (queue.Count != 0)
        {
            // Highlight all nodes in the queue
            tempNode = queue.Dequeue();

            // Get data from tile
            var tileRend = tempNode.myTile.GetComponent<Renderer>();
            var tileScript = tempNode.myTile.GetComponent<TileScript>();
            Vector2Int tilePos = tileScript.position;

            // Highlight tile
            tileRend.material = highlighted;
            tileScript.highlighted = true;

            // Get neighboring tiles
            GameObject upTile = GetTileAtPos(new Vector2Int(tilePos.x, tilePos.y - 1));
            GameObject downTile = GetTileAtPos(new Vector2Int(tilePos.x, tilePos.y + 1));
            GameObject leftTile = GetTileAtPos(new Vector2Int(tilePos.x - 1, tilePos.y));
            GameObject rightTile = GetTileAtPos(new Vector2Int(tilePos.x + 1, tilePos.y));
        
            // Add neighboring tiles to queue if in range
            if (tempNode.tileRange > 0)
            {
                if (ValidHighlightTile(upTile))
                {
                    tempNode.up = new PathTreeNode(tempNode, upTile, tempNode.tileRange-1);
                    queue.Enqueue(tempNode.up);
                }

                if (ValidHighlightTile(downTile))
                {
                    tempNode.down = new PathTreeNode(tempNode, downTile, tempNode.tileRange-1);
                    queue.Enqueue(tempNode.down);
                }

                if (ValidHighlightTile(leftTile))
                {
                    tempNode.left = new PathTreeNode(tempNode, leftTile, tempNode.tileRange-1);
                    queue.Enqueue(tempNode.left);
                }

                if (ValidHighlightTile(rightTile))
                {
                    tempNode.right = new PathTreeNode(tempNode, rightTile, tempNode.tileRange-1);
                    queue.Enqueue(tempNode.right);
                }
            }
        }

        return root;
    }

    private void ResetAllHighlights()
    {
        GameObject currentTile;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Get data from tile at current pos
                currentTile = GetTileAtPos(new Vector2Int(x,y));
                var currentTileScript = currentTile.GetComponent<TileScript>();

                // Check if the tile is highlighted
                if (currentTileScript.highlighted)
                {
                    // Reset highlighting
                    currentTile.GetComponent<Renderer>().material = unselected;
                    currentTileScript.highlighted = false;
                }
            }
        }
    }

    private bool ValidHighlightTile(GameObject tileToCheck)
    {
        bool valid = false;

        // null check
        if (tileToCheck != null)
        {
            // Get data from tile
            var tileScript = tileToCheck.GetComponent<TileScript>();

            // Check if already highlighted
            if (tileScript.highlighted == false && tileScript.passable)
            {
                valid = true;
            }
        }

        return valid;
    }

    // Get a tile game object from a grid position
    private GameObject GetTileAtPos(Vector2Int pos)
    {
        GameObject tileAtPos = null;

        if (TilePosInRange(pos))
        {
            tileAtPos = grid[pos.x, pos.y];
        }

        return tileAtPos;
    }

    // Get a character game object from a grid position (if there is one)
    public GameObject GetCharacterAtPos(Vector2Int pos)
    {
        GameObject charaterAtPos = null;
        GameObject tileAtPos = GetTileAtPos(pos);

        if (tileAtPos != null)
        {
            // Get character from tile
            var tileScript = tileAtPos.GetComponent<TileScript>();
            charaterAtPos = tileScript.characterOn;
        }

        return charaterAtPos;
    }
}

public class PathTreeNode
{
    // Parent node
    public PathTreeNode parent = null;

    // Cardinal directions
    public PathTreeNode up = null;
    public PathTreeNode down = null;
    public PathTreeNode left = null;
    public PathTreeNode right = null;

    // My tile
    public GameObject myTile = null;

    // Tile range
    public int tileRange;

    public PathTreeNode() {}

    public PathTreeNode(PathTreeNode p, GameObject t, int range)
    {
        parent = p;
        myTile = t;
        tileRange = range;
    }
}