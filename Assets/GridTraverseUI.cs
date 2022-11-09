using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridTraverseUI : MonoBehaviour
{
    [SerializeField] public BattleEngine battleScript;
    [SerializeField] public Button button;
    [SerializeField] public float battleSleepTime;

    private TileScript currTile = null;
    private Vector2Int activePos = new Vector2Int(-1,-1);
    private float sleepTimer;
    private bool setSleep = false;

    // Camera Ref
    [SerializeField] public CameraControl camScript;

    // Update is called once per frame
    void Update()
    {
        getCurrentTile();

        if (checkLinkTile())
        {
            button.interactable = true;
            button.interactable = true;
        }
        else
        {
            button.interactable = false;
            button.interactable = false;
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

    public void traverseGrid()
    {
        //if (battleScript.active)
        //{
            // ----- Move the active character to the other grid -----
            Debug.Log("Moving character to another grid");

            // Add character to target grid
            currTile.targetGrid.SpawnCharacter(currTile.characterOn, currTile.targetTile.position, false);

            // Remove character from current grid
            currTile.characterOn = null;
            currTile.hasCharacter = false;

            // Sleep Battle Engine for a Moment
            battleScript.active = false;
            battleScript.interactable = false;
            sleepTimer = battleSleepTime;
            setSleep = true;
        //}
    }

    public void getCurrentTile()
    {
        if (activePos != battleScript.activeUnitPos && battleScript.init == true)
        {
            activePos = battleScript.activeUnitPos;
            currTile = battleScript.activeUnitTile.GetComponent<TileScript>();
        }
    }

    private bool checkLinkTile()
    {
        bool isLinkTile = false;

        if (currTile != null)
        {
            if (currTile.hasGridLink == true)
            {
                isLinkTile = true;
            }
        }

        return isLinkTile;
    }
}
