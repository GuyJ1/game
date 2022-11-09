using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CannonUI : MonoBehaviour
{
    [SerializeField] public BattleEngine battleScript;
    [SerializeField] public Button fireCannonball;
    [SerializeField] public Button fireCharacter;
    [SerializeField] public float battleSleepTime;

    private GameObject currCannon = null;
    private GameObject grid;
    private Vector2Int activePos = new Vector2Int(-1,-1);
    private float sleepTimer;
    private bool setSleep = false;

    // Camera Ref
    [SerializeField] public CameraControl camScript;

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
                battleScript.interactable = true;
                battleScript.endTurn();
                setSleep = false;
                //camScript.StopCameraFollow();
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
            battleScript.interactable = false;
            sleepTimer = battleSleepTime;
            setSleep = true;
        }
    }

    public void fireSelf()
    {
        if (battleScript.active)
        {
            // Fire Effect
            var cannonScript = currCannon.transform.GetChild(0).GetComponent<CannonControl>();
            //cannonScript.fireEffect();

            // Fire Active Character
            Transform modelTrans = battleScript.activeUnit.transform.GetChild(0);
            //modelTrans.GetComponent<Animation>().Play();
            var modelScript = modelTrans.GetComponent<PlayerCollision>();
            modelScript.wakeRigidBody();
            cannonScript.fireObject(modelTrans.gameObject);

            // Set Camera Follow
            camScript.SetCameraFollow(modelTrans.gameObject);

            // Sleep Battle Engine for a Moment
            //battleScript.active = false;
            battleScript.interactable = false;
            sleepTimer = battleSleepTime;
            setSleep = true;
        }
    }

    private void getAdjacentCannon()
    {
        // Only get if the active character grid pos has changed
        if (activePos != battleScript.activeUnitPos && battleScript.init == true)
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
