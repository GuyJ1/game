using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main controller for the battle system
//Note that this system must be activated and will not perform any logic until it is
public class BattleEngine : MonoBehaviour {
    public List<GameObject> units = new List<GameObject>();
    public GameObject grid;
    public bool active = false; //Activation flag to be set by other systems

    private bool init = false;
    private bool isPlayerTurn;
    private bool moved = false;
    private bool acted = false;
    private uint turnCount = 0;
    private GameObject activeUnit;
    private Vector2Int activeUnitPos;
    private List<GameObject> unitsBySpeed = new List<GameObject>(); //Units sorted from lowest to highest speed values
    private List<GameObject> turnQueue = new List<GameObject>(); //Stored units in the turn queue (units can repeat)

    // Click Detection
    private Camera cam;
    private Renderer rend;
    private Ray ray;
    private RaycastHit hit;
    private int gridMask = 1 << 6;

    // Selection Management
    private bool charSelected = false;
    private bool charHighlighted = false;
    private Vector2Int selectedCharPos;
    private Vector2Int highlightedCharPos;

    // Start is called before the first frame update
    void Start() {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update() {
        if(!active) return;
        else {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            if(!init) {
                Debug.Log("BattleEngine initializing...");
                var gridTiles = grid.GetComponent<GridBehavior>();
                int i = 0;
                //Populate units from grid
                for(int x = 0; x < gridTiles.width; x++) {
                    for(int y = 0; y < gridTiles.height; y++) {
                        GameObject tile = gridTiles.grid[x, y];
                        var tileScript = tile.GetComponent<TileScript>();
                        if(tileScript.characterOn != null) {
                            units.Add(tileScript.characterOn);
                            i++;
                        }
                    }
                }
                //Sort speed list
                foreach(GameObject unit in units) unitsBySpeed.Add(unit);
                unitsBySpeed.Sort(compareBySpeed);
                //Set up turns
                updateTurnOrder();
                pickNewTurn();
                //Make sure active tile is updated
                gridTiles.grid[activeUnitPos.x, activeUnitPos.y].GetComponent<Renderer>().material = gridTiles.activeUnselected;
                init = true;
                Debug.Log("BattleEngine initialized.");
            }
            else {
                var gridTiles = grid.GetComponent<GridBehavior>();
                // Use physics to detect a "collision" from a ray
                if (Physics.Raycast(ray, out hit, 1000f, gridMask) == true)
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
                                selectRend.material = isTileActive(tilePos) ? gridTiles.activeSelected : gridTiles.selected;
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
                                selectRend.material = isTileActive(tilePos) ? gridTiles.activeUnselected : gridTiles.unselected;
                                Debug.Log("Character on " + objectHit.name + " has been unselected");
                                charSelected = false;
                            }
                            // Case 3: if the character is different than the currently selected character,
                            // then deselect the current character and select the new one
                            else
                            {
                                // Unselect currently selected character
                                Debug.Log("Unselecting character on " + selectedCharPos.x + " " + selectedCharPos.y);
                                gridTiles.grid[selectedCharPos.x, selectedCharPos.y].GetComponent<Renderer>().material = isTileActive(selectedCharPos) ? gridTiles.activeUnselected : gridTiles.unselected;

                                // Select new tile at tile pos (Note: code is repeated above, not good practice)
                                HighlightValidMoves(tilePos, tileScript.characterOn.GetComponent<CharacterStats>().MV);
                                selectRend.material = isTileActive(selectedCharPos) ? gridTiles.activeSelected : gridTiles.selected;
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
                            Debug.Log(activeUnit);
                            Debug.Log(gridTiles.GetCharacterAtPos(selectedCharPos));
                            if (charSelected && activeUnit == gridTiles.GetCharacterAtPos(selectedCharPos) && tilePos != selectedCharPos)
                            {
                                // Unselect currently select character
                                Debug.Log("Unselecting character on " + selectedCharPos.x + " " + selectedCharPos.y);
                                gridTiles.grid[selectedCharPos.x, selectedCharPos.y].GetComponent<Renderer>().material = isTileActive(selectedCharPos) ? gridTiles.activeUnselected : gridTiles.unselected;
                                charSelected = false;
                                charHighlighted = false;

                                // Move character
                                if (gridTiles.MoveCharacterOnTile(selectedCharPos, tilePos, true) == false)
                                {
                                    Debug.Log("Cannot move character to tile " + tilePos.x + " " + tilePos.y);
                                }
                                else {
                                    ResetAllHighlights();
                                    moved = true;
                                    //TEMPORARY END TO TEST TURNS
                                    endTurn();
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
                                gridTiles.grid[highlightedCharPos.x, highlightedCharPos.y].GetComponent<Renderer>().material = isTileActive(highlightedCharPos) ? gridTiles.activeUnselected : gridTiles.unselected;
                            }

                            // Highlight mouse over tile if it's okay to highlight it
                            if (okayToHighlight)
                            {
                                selectRend.material = isTileActive(highlightedCharPos) ? gridTiles.activeHighlighted : gridTiles.highlighted;
                                highlightedCharPos = tilePos;
                                charHighlighted = true;
                            }
                        }
                    }
                }
                else {
                    if (charHighlighted)
                    {
                        gridTiles.grid[highlightedCharPos.x, highlightedCharPos.y].GetComponent<Renderer>().material = isTileActive(highlightedCharPos) ? gridTiles.activeUnselected : gridTiles.unselected;
                        charHighlighted = false;
                    }
                }
            }
        }
    }

    public void ResetAllHighlights()
    {
        GameObject currentTile;
        var gridTiles = grid.GetComponent<GridBehavior>();
        for (int x = 0; x < gridTiles.width; x++)
        {
            for (int y = 0; y < gridTiles.height; y++)
            {
                // Get data from tile at current pos
                currentTile = gridTiles.GetTileAtPos(new Vector2Int(x,y));
                var currentTileScript = currentTile.GetComponent<TileScript>();

                // Check if the tile is highlighted
                if (currentTileScript.highlighted)
                {
                    // Reset highlighting
                    currentTile.GetComponent<Renderer>().material = isTileActive(new Vector2Int(x,y)) ? gridTiles.activeUnselected : gridTiles.unselected;
                    currentTileScript.highlighted = false;
                }
            }
        }
    }

    // Highlights available move tiles from a provided position and range
    public void HighlightValidMoves(Vector2Int pos, int range)
    {
        var gridTiles = grid.GetComponent<GridBehavior>();
        if(gridTiles.GetCharacterAtPos(pos) != activeUnit) return; //Can't move anywhere unless unit is active
        ResetAllHighlights();
        var root = gridTiles.GetAllPathsFromTile(gridTiles.GetTileAtPos(pos), range);
        highlightPathTree(root);
    }

    public void highlightPathTree(PathTreeNode root) {
        // Get data from tile
        var gridTiles = grid.GetComponent<GridBehavior>();
        var tileRend = root.myTile.GetComponent<Renderer>();
        var tileScript = root.myTile.GetComponent<TileScript>();
        Vector2Int tilePos = tileScript.position;

        // Highlight tile
        tileRend.material = isTileActive(tilePos) ? gridTiles.activeHighlighted : gridTiles.highlighted;
        tileScript.highlighted = true;
        if(root.up != null) highlightPathTree(root.up);
        if(root.down != null) highlightPathTree(root.down);
        if(root.left != null) highlightPathTree(root.left);
        if(root.right != null) highlightPathTree(root.right);
    }

    public bool isTileActive(Vector2Int tilePos) {
        var gridTiles = grid.GetComponent<GridBehavior>();
        return gridTiles.grid[tilePos.x, tilePos.y].GetComponent<TileScript>().characterOn == activeUnit;
    }

    //Sorting function by speed. Returns -1 if unit2 is greater, 0 if equal, 1 if unit1 is greater.
    private static int compareBySpeed(GameObject unit1, GameObject unit2) {
        if(unit1 == null) {
            if(unit2 == null) return 0;
            else return -1;
        }
        else {
            if(unit2 == null) return 1;
            else {
                int speed1 = unit1.GetComponent<CharacterStats>().SPD;
                int speed2 = unit2.GetComponent<CharacterStats>().SPD;
                if(speed1 > speed2) return 1;
                else if(speed1 < speed2) return -1;
                else return 0;
            }
        }
    }

    public void pickNewTurn() {
        startTurn();
    }

    public void updateTurnOrder() {
        turnQueue.Clear();
        turnQueue.Add(units[(int) turnCount % units.Count]);
    }

    //Start a new turn for the active unit
    public void startTurn() {
        turnCount++;
        moved = false;
        acted = false;
        updateTurnOrder();
        activeUnit = turnQueue[0];
        var gridTiles = grid.GetComponent<GridBehavior>();
        //Search for unit on grid and save the position for later
        for(int x = 0; x < gridTiles.width; x++) {
            for(int y = 0; y < gridTiles.height; y++) {
                if(gridTiles.GetCharacterAtPos(new Vector2Int(x,y)) == activeUnit) {
                    activeUnitPos = new Vector2Int(x,y);
                    //Make sure active tile is updated
                    gridTiles.grid[activeUnitPos.x, activeUnitPos.y].GetComponent<Renderer>().material = gridTiles.activeUnselected;
                    break;
                }
            }
        }
    }

    //End the active unit's turn
    public void endTurn() {
        pickNewTurn();
    }

    public bool hasMoved() {
        return moved;
    }

    public bool hasActed() {
        return acted;
    }

    public void doAITurn() {

    }

    public uint getTurnCount() {
        return turnCount;
    }

    //Try to take an action at the specified position on the grid. Returns true if action succeeds.
    public bool actUnit(int x, int y) {
        acted = true;
        return true;
    }

    //Try to surrender the battle. Returns true if surrender is accepted.
    public bool trySurrender() {
        return true;
    }

    //Check for victory conditions and end the battle if met
    public void checkVictory() {

    }

    //Check for defeat conditions and end the battle if met
    public void checkDefeat() {

    }
}
