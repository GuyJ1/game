using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    [SerializeField]
    public GameObject gridReference;
    public Vector2Int gridPosition;
    public bool isPlayerOwned;
    public bool overlapCharacter;

    private bool isOnGrid = false;

    // Update is called once per frame
    void Update()
    {
        if (isOnGrid == false && gridReference != null)
        {
            gridReference.GetComponent<GridBehavior>().AttachObjectToGrid(this.gameObject, gridPosition);
            isOnGrid = true;
        }
    }
}
