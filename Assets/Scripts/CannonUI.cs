using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CannonUI : MonoBehaviour
{
    [SerializeField] public GameObject BattleEngine;
    [SerializeField] public Button fireCannonball;
    [SerializeField] public Button fireCharacter;
    [SerializeField] public float battleSleepTime;

    private BattleEngine battleScript;
    private GameObject currCannon = null;
    private GameObject grid;
    private Vector2Int activePos = new Vector2Int(-1,-1);
    private float sleepTimer;
    private bool setSleep = false;

    // Start is called before the first frame update
    void Start()
    {
        battleScript = BattleEngine.GetComponent<BattleEngine>();
    }

    // Update is called once per frame
    void Update()
    {
        getAdjacentCannon();

        if (currCannon == null)
        {
            fireCannonball.interactable = false;
            fireCharacter.interactable = false;
        }
        else
        {
            fireCannonball.interactable = true;
            fireCharacter.interactable = true;
        }
        
        if (setSleep)
        {
            if (sleepTimer <= 0.0f)
            {
                battleScript.active = true;
                battleScript.endTurn();
                setSleep = false;
            }
            else
            {
                sleepTimer -= Time.deltaTime;
            }
        }
    }

    public void fireCannon()
    {
        if (battleScript.active)
        {
            // Fire Cannon
            currCannon.transform.GetChild(0).GetComponent<CannonControl>().fireCannonball();

            // Sleep Battle Engine for a Moment
            battleScript.active = false;
            sleepTimer = battleSleepTime;
            setSleep = true;
        }
    }

    public void fireSelf()
    {

    }

    private void getAdjacentCannon()
    {
        // Only get if the active character grid pos has changed
        if (battleScript.isPlayerTurn && activePos != battleScript.activeUnitPos)
        {
            Debug.Log("Searching for cannon at " + activePos.ToString());

            activePos = battleScript.activeUnitPos;
            grid = battleScript.grid;
            var gridScript = grid.GetComponent<GridBehavior>();
        
            if (checkIfCannon(gridScript.GetTileNorth(activePos)))
            {
                currCannon = gridScript.GetTileNorth(activePos).GetComponent<TileScript>().objectOn;
            }
            else if (checkIfCannon(gridScript.GetTileEast(activePos)))
            {
                currCannon = gridScript.GetTileEast(activePos).GetComponent<TileScript>().objectOn;
            }
            else if (checkIfCannon(gridScript.GetTileSouth(activePos)))
            {
                currCannon = gridScript.GetTileSouth(activePos).GetComponent<TileScript>().objectOn;
            }
            else if (checkIfCannon(gridScript.GetTileWest(activePos)))
            {
                currCannon = gridScript.GetTileWest(activePos).GetComponent<TileScript>().objectOn;
            }
            else
            {
                currCannon = null;
            }
        }
    }

    private bool checkIfCannon(GameObject tile)
    {
        bool isCannon = false;

        if (tile != null)
        {
            var tileScript = tile.GetComponent<TileScript>();
            if (tileScript.objectOn != null && tileScript.objectOn.tag == "Cannon")
            {
                isCannon = true;
            }
        }

        return isCannon;
    }
}
