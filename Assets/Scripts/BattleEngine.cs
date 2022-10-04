using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main controller for the battle system
//NOT FUNCTIONAL YET, missing some classes
public class BattleEngine : MonoBehaviour {
    [SerializeField] public uint unitCount;

    public GameObject [] units;
    public GameObject grid;

    public bool active = false;
    private bool isPlayerTurn;
    private bool moved = false;
    private bool acted = false;
    private uint turnCount = 0;
    private GameObject activeUnit;
    private List<GameObject> unitsBySpeed = new List<GameObject>(); //Units sorted from lowest to highest speed values
    private List<GameObject> turnQueue = new List<GameObject>(); //Stored units in the turn queue (units can repeat)

    // Start is called before the first frame update
    void Start() {
        units = new GameObject[unitCount];
        foreach(GameObject unit in units) unitsBySpeed.Add(unit);
        unitsBySpeed.Sort(compareBySpeed);
        updateTurnOrder();
        pickNewTurn();
    }

    // Update is called once per frame
    void Update() {
        if(!active) return;
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
        
    }

    //Start a new turn for the active unit
    public void startTurn() {
        turnCount++;
        moved = false;
        acted = false;
        updateTurnOrder();
        activeUnit = turnQueue[0];
    }

    //End the active unit's turn
    public void endTurn() {
        
    }

    public bool hasMoved() {
        return moved;
    }

    public bool hasActed() {
        return acted;
    }

    public void doAITurn() {

    }

    //Try to move the unit to a specified position on the grid. Returns true if movement succeeds.
    public bool moveUnit(int x, int y) {
        moved = true;
        return true;
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
