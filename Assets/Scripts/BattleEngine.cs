using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//Main controller for the battle system
//Note that this system must be activated and will not perform any logic until it is
public class BattleEngine : MonoBehaviour 
 {
    //Prefabs
    public GameObject buttonPrefab;

    public List<GameObject> units = new List<GameObject>(); //All units
    public List<GameObject> grids = new List<GameObject>(); //All grids
    public PathTreeNode gridPaths;
    public bool active = false; //Activation flag to be set by other systems

    public bool moving = false; //Whether the current mode is moving or acting
    private Ability selectedAbility;
    private bool init = false;
    public  bool isPlayerTurn;
    private bool moved = false; //Whether movement was taken
    private bool acted = false; //Whether an action was taken
    private uint turnCount = 0;
    public GameObject activeUnit, activeUnitTile;
    public GameObject grid; //Active grid that unit is on
    public Vector2Int activeUnitPos;
    public List<GameObject> deadUnits = new List<GameObject>();
    private List<GameObject> unitsBySpeed = new List<GameObject>(); //Units sorted from lowest to highest speed values
    private List<GameObject> turnQueue = new List<GameObject>(); //Stored units in the turn queue (units can repeat)

    //UI references
    public GameObject canvas;
    private Button attackButton, moveButton, endButton;
    private List<GameObject> actionButtons = new List<GameObject>();
    private GameObject victoryText, defeatText;

    //Click Detection
    private Camera cam;
    private Renderer rend;
    private Ray ray;
    private RaycastHit hit;
    private int gridMask = 1 << 6;

    //Selection Management
    private bool charSelected = false;
    private bool charHighlighted = false;
    private Vector2Int selectedCharPos;
    private Vector2Int highlightedCharPos;

    //AI Variables
    private PlayerActionList playerActions = new PlayerActionList();
    private GameObject playerTarget = null; //Might be unnecessary
    private Ability playerAbility = null; //Might not be needed due to 'selectedAbility'

    // Start is called before the first frame update
    void Start() 
    {
        // Get Camera
        cam = Camera.main;

        // Get Button Options
        attackButton = GameObject.Find("AttackButton").GetComponent<Button>();
        moveButton = GameObject.Find("MoveButton").GetComponent<Button>();
        endButton = GameObject.Find("EndButton").GetComponent<Button>();

        // Get and Set Victory and Defeat Text
        victoryText = GameObject.Find("VictoryText");
        victoryText.SetActive(false);
        defeatText = GameObject.Find("DefeatText");
        defeatText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Ability to quit game on esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Exiting Adrift..."); //For testing purposes, text shows in log
            Application.Quit(); //exits and quits the game application
        }

        // Keyboard control
        if (Input.GetKeyDown(KeyCode.Alpha1)) { selectAction(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { selectMove(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { endTurn(); }
        
        // Check whether the battle system is active
        if(active)
        {
            ray = cam.ScreenPointToRay(Input.mousePosition);
            
            if(!init)
            {
                InitializeBattleSystem();
            }
            else 
            {
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
                    // Character Movement & Actions
                    // ----------------------------
                    if (Input.GetMouseButtonDown(0))
                    {
                        // Movement
                        if(moving) 
                        {
                            // If a tile has a character on it, then we can only select it
                            if (tileScript.hasCharacter)
                            {
                                // Move Camera to clicked character
                                cam.GetComponent<CameraControl>().LookAtPos(objectHit.position);

                                // Case 1: if there's no selected character, then just select this one
                                if (charSelected == false) 
                                {
                                    if(moving) setupMove(objectHit.gameObject);
                                    else setupAction(objectHit.gameObject);
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
                                    if(moving) setupMove(objectHit.gameObject);
                                    else setupAction(objectHit.gameObject);
                                }
                            }
                            // If we have a selected character/tile, then we can move it to any
                            // highlighted tile without a character already on it
                            else
                            {
                                Debug.Log("Moving " + activeUnit);
                                Debug.Log(gridTiles.GetCharacterAtPos(selectedCharPos));
                                moveUnit(tilePos, false);
                            }
                        }
                        // Action
                        else actUnit(tilePos, false);
                    }
                    else
                    // -------------------------------
                    // Character and Tile Highlighting
                    // -------------------------------
                    {
                        if(charSelected == false && moving) //Moving
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
                        else if(!moving && !acted) { //Acting
                            if (charHighlighted && highlightedCharPos != tilePos) {
                                gridTiles.grid[highlightedCharPos.x, highlightedCharPos.y].GetComponent<Renderer>().material = isTileActive(highlightedCharPos) ? gridTiles.activeUnselected : gridTiles.abilityHighlighted;
                            }
                            if(tileScript.passable && tileScript.highlighted) {
                                if(tileScript.characterOn != activeUnit) {
                                    tileScript.highlighted = true;
                                    highlightedCharPos = tilePos;
                                    charHighlighted = true;
                                    gridTiles.grid[tilePos.x, tilePos.y].GetComponent<Renderer>().material = gridTiles.ability;
                                }
                            }
                        }
                    }
                }
                else {
                    if (charHighlighted)
                    {
                        gridTiles.grid[highlightedCharPos.x, highlightedCharPos.y].GetComponent<Renderer>().material = isTileActive(highlightedCharPos) ? gridTiles.activeUnselected : (moving ? gridTiles.unselected : gridTiles.abilityHighlighted);
                        charHighlighted = false;
                    }
                }
            }
        }
    }

    private void InitializeBattleSystem()
    {
        Debug.Log("BattleEngine initializing...");
        // ------------------------------------

        // Add all characters to be controlled by the battle system
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");
        foreach (GameObject character in characters)
        {
            units.Add(character);
        }

        //Sort speed list
        foreach(GameObject unit in units)
        {
            unitsBySpeed.Add(unit);
        }
        unitsBySpeed.Sort(compareBySpeed);

        //Set up turns
        updateTurnOrder();
        pickNewTurn();

        //Make sure active tile is updated
        var activeGrid = grid.GetComponent<GridBehavior>();
        activeGrid.grid[activeUnitPos.x, activeUnitPos.y].GetComponent<Renderer>().material = activeGrid.activeUnselected;
        init = true;

        // ------------------------------------
        Debug.Log("BattleEngine initialized.");
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

    // Highlights available action tiles for abilities
    public void highlightActionTiles(Vector2Int pos, int range) {
        var gridTiles = grid.GetComponent<GridBehavior>();
        ResetAllHighlights();
        gridPaths = gridTiles.GetAllPathsFromTile(gridTiles.GetTileAtPos(pos), range);
        highlightPathTree(gridPaths, true);
    }

    // Highlights available move tiles from a provided position and range
    public void highlightValidMoves(Vector2Int pos, int range)
    {
        // Get script from grid
        var gridScript = grid.GetComponent<GridBehavior>();

        // Only hightlight if the character as pos is the active unit
        if(gridScript.GetCharacterAtPos(pos) == activeUnit)
        {
            // Highlight move tiles
            ResetAllHighlights();
            gridPaths = gridScript.GetAllPathsFromTile(gridScript.GetTileAtPos(pos), range);
            highlightPathTree(gridPaths, false);
        }
    }

    public void highlightPathTree(PathTreeNode root, bool action) {
        // Get data from tile
        var gridTiles = grid.GetComponent<GridBehavior>();
        var tileRend = root.myTile.GetComponent<Renderer>();
        var tileScript = root.myTile.GetComponent<TileScript>();
        Vector2Int tilePos = tileScript.position;

        // Highlight tile
        if(action) tileRend.material = isTileActive(tilePos) ? gridTiles.activeHighlighted : gridTiles.abilityHighlighted;
        else tileRend.material = isTileActive(tilePos) ? gridTiles.activeHighlighted : gridTiles.highlighted;
        if(root.up != null) highlightPathTree(root.up, action);
        if(root.down != null) highlightPathTree(root.down, action);
        if(root.left != null) highlightPathTree(root.left, action);
        if(root.right != null) highlightPathTree(root.right, action);
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

    public void selectAction() {
        moving = false;
        attackButton.Select();
        showActionsList();
        selectedAbility = getBasicAttack(activeUnit);
        setupAction(activeUnitTile);
        actionButtons.ToArray()[0].GetComponent<Button>().Select(); //Temp highlight for basic attack button
    }

    public void selectMove() 
    {
        hideActionsList();
        moving = true;
        moveButton.Select();
        setupMove(activeUnitTile);
    }

    public void pickNewTurn() {
        startTurn();
    }

    public void updateTurnOrder() {
        turnQueue.Clear();
        turnQueue.Add(unitsBySpeed[(int) turnCount % unitsBySpeed.Count]);
    }

    //Start a new turn for the active unit
    public void startTurn()
    {
        // Increment turn count and update turn order
        turnCount++;
        updateTurnOrder();

        // Update flags
        moved = false;
        acted = false;

        // Get active unit script
        activeUnit = turnQueue[0];
        var activeUnitScript = activeUnit.GetComponent<CharacterStats>();

        // Get data from active unit
        grid = activeUnitScript.myGrid;
        var gridScript = grid.GetComponent<GridBehavior>();
        activeUnitPos = activeUnitScript.gridPosition;

        // Deselect ability
        selectedAbility = null;
        
        // Set the tile of which the character is on to be active
        activeUnitTile = gridScript.grid[activeUnitPos.x, activeUnitPos.y];
        activeUnitTile.GetComponent<Renderer>().material = gridScript.activeUnselected;
        cam.GetComponent<CameraControl>().LookAtPos(activeUnitTile.transform.position);

        // Setup buttons based on whether it's the player's turn
        isPlayerTurn = isAllyUnit(activeUnit);
        attackButton.interactable = isPlayerTurn;
        moveButton.interactable = isPlayerTurn;
        endButton.interactable = isPlayerTurn;

        if(!isPlayerTurn)
        {
            doAITurn();
        }
        else 
        {
            int count = 0;

            //Setup action buttons
            foreach(GameObject abilityObject in activeUnit.GetComponent<CharacterStats>().getBattleAbilities()) {
                Ability ability = abilityObject.GetComponent<Ability>();
                GameObject actionButton = Instantiate(buttonPrefab); //This is copying stuff from AttackButton idk why
                actionButton.transform.SetParent(canvas.transform, false);
                actionButton.transform.position = new Vector3(actionButton.transform.position.x + 110, actionButton.transform.position.y - 30 * count, actionButton.transform.position.z);
                Button button = actionButton.GetComponent<Button>();
                button.onClick.SetPersistentListenerState(0, UnityEventCallState.Off);
                button.onClick.AddListener(() => { //Listen to setup ability when clicked
                    selectedAbility = ability;
                    setupAction(activeUnitTile);
                });
                var tmp = actionButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                tmp.text = ability.displayName; //Set button name to ability name
                actionButtons.Add(actionButton);
                count++;
            }
            selectMove(); //Default to move (generally units move before acting)
        }
    }

    //End the active unit's turn
    public void endTurn() 
    {
        var gridTiles = grid.GetComponent<GridBehavior>();
        gridTiles.GetTileAtPos(activeUnitPos).GetComponent<TileScript>().highlighted = false;
        gridTiles.GetTileAtPos(activeUnitPos).GetComponent<Renderer>().material = gridTiles.unselected;

        //Player turn enqueue handling - passes in the current active unit (if it is a player controlled unit), 
        //the target of this turn's action (if any), the type of action taken this turn (if any), and whether the character moved
        if(isPlayerTurn && selectedAbility != null)
        {
            //Add the new PlayerAction to the playerActions queue, using the overloaded constructor
            playerActions.add(new PlayerAction(activeUnit.GetComponent<CharacterStats>(), selectedAbility, moved));
        }

        //Logging to display what is being enqueued
        //Debug.Log("AI Enqueue: " + activeUnit.GetComponent<CharacterStats>().Name + " " + playerTarget.GetComponent<CharacterStats>().Name + " " + selectedAbility + " " + moved);

        //Logging to show what is at the top of the playerActions queue
        if(!playerActions.isEmpty())
        {
            //Usually shows the same thing over and over again, since player actions aren't being dequeued until the 50th turn
            Debug.Log("AI Enqueue: " + playerActions.Peek().GetCharacter().Name + " " + playerActions.Peek().GetAbility().ID + " " + playerActions.Peek().GetMovement() 
            + "\n" + "Queue Size: " + playerActions.Count());
        }
        foreach(GameObject button in actionButtons) Destroy(button);
        actionButtons.Clear();
        hideActionsList();
        pickNewTurn();
    }

    public void setupMove(GameObject objectTile) 
    {
        setupActionOrMove(objectTile, false);
    }

    public void setupAction(GameObject objectTile)
    {
        setupActionOrMove(objectTile, true);
    }

    private void setupActionOrMove(GameObject tile, bool isAction)
    {
        var gridTiles = grid.GetComponent<GridBehavior>();
        var selectRend  = tile.GetComponent<Renderer>();
        var tileScript  = tile.GetComponent<TileScript>();
        Vector2Int tilePos = tileScript.position;

        // Unselect currently selected character
        if(charSelected) {
            Debug.Log("Unselecting character on " + selectedCharPos.x + " " + selectedCharPos.y);
            gridTiles.grid[selectedCharPos.x, selectedCharPos.y].GetComponent<Renderer>().material = isTileActive(selectedCharPos) ? gridTiles.activeUnselected : gridTiles.unselected;
        }

        ResetAllHighlights();

        if (isAction)
        {
            if(!acted) { highlightActionTiles(tilePos, selectedAbility.range); }
        }
        else
        {
            if(!moved) { highlightValidMoves(tilePos, tileScript.characterOn.GetComponent<CharacterStats>().MV); }
        }

        // Select new tile at tile pos
        selectRend.material = isTileActive(tilePos) ? gridTiles.activeSelected : gridTiles.selected;
        selectedCharPos = tilePos;

        Debug.Log("Character on " + tile.name + " has been selected");

        // Selection Logic
        charSelected = true;
        charHighlighted = false;
    }

    //Try to use the selected ability at the specified position on the grid. Returns true if action succeeds. Will not affect game state if simulate is true.
    public bool actUnit(Vector2Int tilePos, bool simulate) 
    {
        if(moving || acted || selectedAbility == null) return false;
        var gridTiles = grid.GetComponent<GridBehavior>();
        var tileScript = gridTiles.GetTileAtPos(tilePos).GetComponent<TileScript>();
        if(selectedAbility.requiresTarget) 
        { 
            //Check for valid target
            if(!tileScript.hasCharacter) return false;
            if(selectedAbility.friendly && !isAllyUnit(gridTiles.GetCharacterAtPos(tilePos))) return false;
            if(!selectedAbility.friendly && isAllyUnit(gridTiles.GetCharacterAtPos(tilePos))) return false;
            int dist = Mathf.Abs(activeUnitPos.x - tilePos.x) + Mathf.Abs(activeUnitPos.y - tilePos.y); //Take Manhattan distance
            if(dist > selectedAbility.range) return false;
        }
        if(simulate) return true;
        List<GameObject> characters = new List<GameObject>();
        if(tileScript.hasCharacter) characters.Add(tileScript.characterOn); //TODO: Make this dependent on ability. Just uses selected target for now.
        ResetAllHighlights();
        acted = true;
        attackButton.interactable = false;
        selectedAbility.affectCharacters(activeUnit, characters);

        // ----- Combo Attacks ------
        int xDist = activeUnitPos.x - tilePos.x;
        int yDist = activeUnitPos.y - tilePos.y;

        //No diagonals
        if(selectedAbility.requiresTarget && xDist != yDist)
        {
            //Horizontal cases
            if(Mathf.Abs(xDist) > Mathf.Abs(yDist))
            { 
                if(xDist > 0)
                {
                    tryComboAttack(gridTiles.GetTileWest(tilePos), tileScript.characterOn);
                }
                else
                {
                    tryComboAttack(gridTiles.GetTileEast(tilePos), tileScript.characterOn);
                }
            }
            //Vertical cases
            else
            {
                if(yDist > 0)
                {
                    tryComboAttack(gridTiles.GetTileSouth(tilePos), tileScript.characterOn);
                }
                else
                {
                    tryComboAttack(gridTiles.GetTileNorth(tilePos), tileScript.characterOn);
                }
            }
        }
        // --------------------------

        //AI: Forward the target(s) to AI handler for enqueue. Currently only forwards one character - for refactoring later
        playerTarget = characters[0];

        if(!moved) selectMove(); //Move to move state if available
        update();
        return true;
    }

    //Try to perform a combo attack from the specified character to the target
    public void tryComboAttack(GameObject characterTile, GameObject targetCharacter) {
        if(characterTile != null && characterTile.GetComponent<TileScript>()) {
            TileScript tile = characterTile.GetComponent<TileScript>();
            if(tile.hasCharacter && isAllyUnit(tile.characterOn) != isAllyUnit(targetCharacter)) { //Need target on opposite team
                Debug.Log("Triggering combo attack...");
                getComboAttack(tile.characterOn).affectCharacter(tile.characterOn, targetCharacter);
            }
        }
    }

    //Try to move the unit to the specified position on the grid. Returns true if move succeeds. Will not affect game state if simulate is true.
    public bool moveUnit(Vector2Int tilePos, bool simulate) 
    {
        var gridTiles = grid.GetComponent<GridBehavior>();
        if(!moved && moving && charSelected && activeUnit == gridTiles.GetCharacterAtPos(selectedCharPos) && tilePos != selectedCharPos) 
        {
            // Move character
            if (gridTiles.MoveCharacterOnTile(gridPaths, selectedCharPos, tilePos, true) == false)
            {
                Debug.Log("Cannot move character to tile " + tilePos.x + " " + tilePos.y);
                return false;
            }
            else 
            {
                if(simulate) return true;
                // Unselect currently select character
                Debug.Log("Unselecting character on " + selectedCharPos.x + " " + selectedCharPos.y);
                activeUnitPos = tilePos;
                activeUnitTile = gridTiles.GetTileAtPos(tilePos);
                gridTiles.grid[selectedCharPos.x, selectedCharPos.y].GetComponent<Renderer>().material = isTileActive(selectedCharPos) ? gridTiles.activeUnselected : gridTiles.unselected;
                charSelected = false;
                charHighlighted = false;
                ResetAllHighlights();
                moved = true;
                moveButton.interactable = false;
                if(!acted) selectAction(); //Move to action state if available
                update();
                return true;
            }
        }
        return false;
    }

    //Try to surrender the battle. Returns true if surrender is accepted.
    public bool trySurrender() {
        return true;
    }

    //Perform all end-of-turn logic
    public void update() {
        //Collect any dead units
        foreach(GameObject unit in units) {
            if(!isUnitAlive(unit)) {
                if(grid.GetComponent<GridBehavior>().RemoveCharacter(unit)) {
                    deadUnits.Add(unit);
                    unitsBySpeed.Remove(unit);
                }
            }
        }
        checkOutcome();
        if(moved && acted) endTurn();
        checkOutcome();
    }

    //Check whether victory or defeat conditions are met
    public void checkOutcome() {
        checkDefeat();
        checkVictory();
    }

    //Check for victory conditions and end the battle if met
    private void checkVictory() {
        bool won = true;
        //Check if any enemies are alive
        foreach(GameObject unit in units) {
            if(isUnitAlive(unit) && !isAllyUnit(unit)) {
                won = false;
                break;
            }
        }
        //End on win
        if(won) {
            onEnd();
            victoryText.SetActive(true);
        }
    }

    //Check for defeat conditions and end the battle if met
    private void checkDefeat() {
        bool loss = true;
        //Check if any allies are alive
        foreach(GameObject unit in units) {
            if(isUnitAlive(unit) && isAllyUnit(unit)) {
                loss = false;
                break;
            }
        }
        //End on loss
        if(loss) {
            onEnd();
            defeatText.SetActive(true);
        }
    }

    private void onEnd() {
        active = false;
        attackButton.interactable = false;
        moveButton.interactable = false;
        endButton.interactable = false;
        deadUnits.Clear();
    }

    public static bool isUnitAlive(GameObject unit) {
        return !unit.GetComponent<CharacterStats>().isDead();
    }

    public static bool isAllyUnit(GameObject unit) {
        return unit.GetComponent<CharacterStats>().crew.GetComponent<CrewSystem>().isPlayer;
    }

    public static Ability getBasicAttack(GameObject unit) {
        return unit.GetComponent<CharacterStats>().basicAttack.GetComponent<Ability>();
    }

    public static Ability getComboAttack(GameObject unit) {
        return unit.GetComponent<CharacterStats>().comboAttack.GetComponent<Ability>();
    }

    public void showActionsList() {
        foreach(GameObject button in actionButtons) {
            button.SetActive(true);
        }
    }

    public void hideActionsList() {
        foreach(GameObject button in actionButtons) {
            button.SetActive(false);
        }
    }

    public bool hasMoved() {
        return moved;
    }

    public bool hasActed() {
        return acted;
    }

    public void doAITurn() {
        //TODO: AI stuff here
        endTurn();
    }

    public uint getTurnCount() {
        return turnCount;
    }
}
