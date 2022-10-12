using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMap
{
    public int gridSelected = 0;

    // ----- Default Grid -----
    public const uint DefaultGridWidth = 8;
    public const uint DefaultGridHeight = 16;
    public static bool[,] DefaultGridPassableFlags = 
    {
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},   
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
    };
    // ------------------------

    // ------- Grid One -------
    public const uint GridOneWidth = 8;
    public const uint GridOneHeight = 16;
    public static bool[,] GridOnePassableFlags = 
    {
        {true , true , false, false, false, false, true , true },
        {true , true , false, false, false, false, true , true },
        {true , false, false, false, false, false, false, true },
        {true , false, false, false, false, false, false, true },
        {true , false, false, false, false, false, false, true },
        {false, false, false, false, false, false, false, false},
        {false, false, false, true , true , false, false, false},
        {false, false, false, true , true , false, false, false},
        {false, false, false, true , true , false, false, false},
        {false, false, false, false, false, false, false, false},
        {false, false, false, false, false, false, false, false},
        {true , false, false, false, false, false, false, true },
        {true , false, false, false, false, false, false, true },
        {true , false, false, false, false, false, false, true },
        {true , true , false, false, false, false, true , true },
        {true , true , true,  false, false, true , true , true }
    };
    // ------------------------

    // Constructors
    public GridMap() {}
    public GridMap(int gridNumber) {gridSelected = gridNumber;}

    // Select a grid map
    public void SelectGrid(int gridNumber)
    {
        switch (gridNumber)
        {
            case 0:
                gridSelected = 0;
                break;
            case 1:
                gridSelected = 1;
                break;
            default:
                Debug.Log("SelectGrid(): Error! Grid number is out of range. Selecting default grid instead");
                gridSelected = 0;
                break;
        }
    }

    // Get the width of a grid map
    public uint GetGridWidth()
    {  
        uint widthToReturn = 0;

        switch(gridSelected)
        {
            case 0:
                widthToReturn = DefaultGridWidth;
                break;
            case 1:
                widthToReturn = GridOneWidth;
                break;
            default:
                Debug.Log("GetGridWidth(): Error! Cannot find selected grid");
                break;
        }

        return widthToReturn;
    }

    // Get the height of a grid map
    public uint GetGridHeight()
    {  
        uint heightToReturn = 0;

        switch(gridSelected)
        {
            case 0:
                heightToReturn = DefaultGridHeight;
                break;
            case 1:
                heightToReturn = GridOneHeight;
                break;
            default:
                Debug.Log("GetGridHeight(): Error! Cannot find selected grid");
                break;
        }

        return heightToReturn;
    }

    // Get the flag at a given grid map position
    public bool GetFlagAtPos(Vector2Int pos)
    {
        bool flagAtPos = false;

        switch(gridSelected)
        {
            case 0:
                flagAtPos = DefaultGridPassableFlags[pos.y, pos.x];
                break;
            case 1:
                flagAtPos = GridOnePassableFlags[pos.y, pos.x];
                break;
            default:
                Debug.Log("GetFlagAtPos(): Error! Cannot find selected grid");
                break;
        }

        return flagAtPos;
    }
}
