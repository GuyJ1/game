/// @author: Bryson Squibb
/// @date: 10/08/2022
/// @description: this script spawns a grid of tile objects, then
/// it provides an interface to interact with said spawned grid

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehavior : MonoBehaviour
{
    [SerializeField] public int gridNum; // Grid number 
    [SerializeField] public int tilesize; // Size of each tile

    // Grid vars
    public GameObject Tile; // A cell or tile that makes up the grid
    public GameObject [,] grid; // The actual grid, represented by a 2d array
    private uint availableTiles;
    public uint height, width;

    // Things used for click detection
    private Camera cam;

    // Materials
    [SerializeField] public Material unselected, highlighted, selected, activeUnselected, activeHighlighted, activeSelected, ability, abilityHighlighted, impassible;

    // Crews
    public List<GameObject> crews;
    
    // Start is called before the first frame update
    void Start()
    {
        // Generate the grid by instantiating tile objects
        GenerateGrid(gridNum);

        // Get camera
        cam = Camera.main;

        // Spawn characters (random positions)
        foreach(GameObject crew in crews) {
            List<GameObject> characters = crew.GetComponent<CrewSystem>().characters;
            foreach(GameObject character in characters) {
                crew.GetComponent<CrewSystem>().initCharacters();
                SpawnCharacter(character);
            }
        }
    }

    // --------------------------------------------------------------
    // @desc: Create a grid object for every tile position
    // --------------------------------------------------------------
    void GenerateGrid(int gridNumber = 0)
    {
        // Set Grid Map
        GridMap passableTiles = new GridMap(gridNumber);

        // Set dimensions
        width = passableTiles.GetGridWidth();
        height = passableTiles.GetGridHeight();

        // Declare the grid to appropriate width and height
        grid = new GameObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Calculate the position of the tile to create
                Vector2Int xy = new Vector2Int(x, y);
                Vector3 pos = new Vector3(transform.position.x + x*tilesize, transform.position.y + passableTiles.GetHeightOffsetAtPos(xy), transform.position.z + y*tilesize);
                
                // Create the tile
                GameObject newTile = Instantiate(Tile, pos, transform.rotation, this.transform);

                // Set the data of the newly created tile
                newTile.name = "Tile " + x + " " + y;
                var newTileScript = newTile.GetComponent<TileScript>();
                newTileScript.position.x = x;
                newTileScript.position.y = y;

                // Check the grid map for impassible tiles
                if (passableTiles.GetFlagAtPos(xy))
                {
                    SetTileToImpassible(newTile);
                }

                // Link the new tile to a position on the grid
                grid[x, y] = newTile;
            }
        }

        // Tiles valid for spawning characters
        availableTiles = width*height;
    }
    // --------------------------------------------------------------
    // @desc: Spawn a character on the grid
    // @arg: character - the character prefab object to spawn
    // --------------------------------------------------------------
    public bool SpawnCharacter(GameObject character)
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
                    Vector3 tilePos = spawningTile.transform.position;
                    Vector3 pos = new Vector3(tilePos.x, tilePos.y+0.5f, tilePos.z);
                    Debug.Log("Spawning character as position " + randX + " " + randY);
                    character.GetComponent<CharacterStats>().gridPosition = new Vector2Int(randX, randY);
                    character.GetComponent<CharacterStats>().myGrid = this.gameObject;

                    // Set tile data
                    tilesScript.characterOn = Instantiate(character, pos, transform.rotation, this.transform);
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

        return characterSpawned;
    }

    // --------------------------------------------------------------
    // @desc: Spawn a character on the grid
    // @arg: character - the character prefab object to spawn
    // @arg: spawnPos  - the logical grid position to spawn on
    // --------------------------------------------------------------
    public bool SpawnCharacter(GameObject character, Vector2Int spawnPos)
    {
        GameObject spawningTile;
        bool characterSpawned = false;
        
        // Get the respective tile on the grid
        spawningTile = grid[spawnPos.x, spawnPos.y];

        // Get data from tile object
        var tilesScript = spawningTile.GetComponent<TileScript>();

        // Check whether the tile already has a character on it
        // + whether the tile is a passable tile
        if (tilesScript.hasCharacter == false && tilesScript.passable)
        {
            // Spawn the character if valid
            Vector3 tilePos = spawningTile.transform.position;
            Vector3 pos = new Vector3(tilePos.x, tilePos.y+0.5f, tilePos.z);
            Debug.Log("Spawning character as position " + spawnPos.x + " " + spawnPos.y);
            character.GetComponent<CharacterStats>().gridPosition = spawnPos;
            character.GetComponent<CharacterStats>().myGrid = this.gameObject;

            // Set tile data
            tilesScript.characterOn = Instantiate(character, pos, transform.rotation, this.transform);
            tilesScript.hasCharacter = true;

            // Update flag and # of available tiles
            availableTiles--;

            // Whether the character has spawned
            characterSpawned = true;
        }
        else
        {
            Debug.Log("Grid spawnCharacter(): Error! Can't spawn a character on top of another character");
        }

        return characterSpawned;
    }

    // --------------------------------------------------------------
    // @desc: Attach an object to the grid
    // @arg: obj     - the object to attach
    // @arg: gridPos - the logical grid position to move the object to
    // @arg: overlapCharacter - whether this object can overlap a character
    // @ret: bool    - whether this method was successful or not
    // --------------------------------------------------------------
    public bool AttachObjectToGrid(GameObject obj, Vector2Int gridPos, bool overlapCharacter = false)
    {
        GameObject tile;
        bool objectAttached = false;
        bool isValid = true;

        // Get the respective tile on the grid
        tile = grid[gridPos.x, gridPos.y];
        Debug.Log("Attaching object to position " + gridPos.x + " " + gridPos.y);

        // Get data from tile object
        var tilesScript = tile.GetComponent<TileScript>();
    
        // Check range
        if (TilePosInRange(gridPos))
        {
            // If this object cannot overlap a character, then check whether
            // this tile has a character on it
            if (overlapCharacter == false && tilesScript.hasCharacter == true)
            {
                isValid = false;
            }
        }
        
        // If the object can spawn
        if (isValid)
        {
            // Get tile pos
            Vector3 tilePos = tile.transform.position;

            // Move object to tile pos
            obj.transform.parent   = tile.transform;
            obj.transform.position = tilePos;

            // Attach object to tile
            tilesScript.objectOn = obj;
            tilesScript.hasObject = true;

            // Set impasssible if applicable
            if (overlapCharacter == false)
            {
                SetTileToImpassible(tile);
            }

            // Set flag
            objectAttached = true;
        }
        else
        {
            Debug.Log("Grid spawnCharacter(): Error! Object cannot spawn here");
        }

        return objectAttached;
    }

    // --------------------------------------------------------------

    public bool RemoveCharacter(GameObject character) {
        Vector2Int tilePos = character.GetComponent<CharacterStats>().gridPosition;
        var tile = GetTileAtPos(tilePos).GetComponent<TileScript>();
        if(tile.characterOn == character) {
            character.GetComponent<CharacterStats>().removeFromGrid();
            tile.characterOn = null;
            tile.hasCharacter = false;
            return true;
        }
        return false;
    }

    // --------------------------------------------------------------
    // @desc: Move a character on the grid based on tile positions
    // @arg: pathTreeRef - reference to the path tree
    // @arg: sourcePos - logical grid position with a character on it
    // @arg: destPos   - logical grid position to move the character to
    // @ret: bool      - whether the move is successful or not
    // --------------------------------------------------------------
    public bool MoveCharacterOnTile(PathTreeNode pathTreeRef, Vector2Int sourcePos, Vector2Int destPos, bool onlyHighlighted)
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
                    destTileScript.pathRef.PathToRootOnStack(charToMove.GetComponent<FollowPath>().pathToFollow);

                    // Move camera to destPos
                    cam.GetComponent<CameraControl>().SetCameraFollow(charToMove);

                    // Set source tile data
                    sourceTileScipt.hasCharacter = false;
                    sourceTileScipt.characterOn = null;

                    // Set destination tile data
                    destTileScript.hasCharacter = true;
                    destTileScript.characterOn = charToMove;

                    charToMove.GetComponent<CharacterStats>().gridPosition = destPos;

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

    // --------------------------------------------------------------
    // @desc: Makes a tile impassible
    // @arg: GameObject - tile to make impassible
    // --------------------------------------------------------------
    public void SetTileToImpassible(GameObject tile)
    {
        // Get script from tile
        var tileScript = tile.GetComponent<TileScript>();

        // Set tile to impassible
        tileScript.passable = false;

        // Change material
        tile.GetComponent<Renderer>().material = impassible;

        // Update available tiles
        availableTiles--;
    }

    // --------------------------------------------------------------
    // @desc: Checks whether a logical tile position is in the grid
    // @arg: pos  - the position to check
    // @ret: bool - whether the position is in the grid
    // --------------------------------------------------------------
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

    // --------------------------------------------------------------
    // @desc: Creates a tree data structure based on what moves a 
    // character can take from a given tile position
    // @arg: pos          - the root tile position
    // @arg: range        - how many tiles the character can move
    // @ret: PathTreeNode - the constructed tree
    // --------------------------------------------------------------
    public PathTreeNode GetAllPathsFromTile(GameObject tile, int range)
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
            //tileRend.material = highlighted;
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

    // --------------------------------------------------------------
    // @desc: Tests whether a tile can be highlighted
    // @arg: tileToCheck - the tile to check
    // @ret: bool        - whether the tile can be highlighted
    // --------------------------------------------------------------
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
    public GameObject GetTileAtPos(Vector2Int pos)
    {
        GameObject tileAtPos = null;

        if (TilePosInRange(pos))
        {
            tileAtPos = grid[pos.x, pos.y];
        }

        return tileAtPos;
    }

    public GameObject GetTileNorth(Vector2Int pos) {
        return GetTileAtPos(new Vector2Int(pos.x, pos.y + 1));
    }

    public GameObject GetTileSouth(Vector2Int pos) {
        return GetTileAtPos(new Vector2Int(pos.x, pos.y - 1));
    }

    public GameObject GetTileEast(Vector2Int pos) {
        return GetTileAtPos(new Vector2Int(pos.x + 1, pos.y));
    }

    public GameObject GetTileWest(Vector2Int pos) {
        return GetTileAtPos(new Vector2Int(pos.x - 1, pos.y));
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
