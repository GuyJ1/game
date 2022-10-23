using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    // Movespeed
    [SerializeField] public float moveSpeed = 3.0f;

    // A stack of path nodes that link to the position of tiles
    public Stack<PathTreeNode> pathToFollow = null;

    // Current target position and direction
    private Vector3 targetPos;
    private Vector3 targetDirection;

    // Flags
    private bool tileSet = false;

    // Update is called once per frame
    void Update()
    {
        // Get a new tile to travel to when these conditions are met
        if (pathToFollow != null && pathToFollow.Count > 0 && tileSet == false)
        {
            // Get the next node
            var currentNode = pathToFollow.Pop();

            // Get the position of the corresponding tile
            Vector3 pos = currentNode.myTile.transform.position;

            // Set the target position to the position of the tile
            targetPos = new Vector3(pos.x, pos.y + 0.5f, pos.z);
            targetDirection = (targetPos - transform.position).normalized;

            // Target tile is set
            tileSet = true;
        }

        // When we have a tile to travel to
        if (tileSet)
        {
            float distToTarget = Vector3.Distance(transform.position, targetPos);
            float moveDist = Vector3.Distance(new Vector3(0,0,0), targetDirection * moveSpeed * Time.deltaTime);

            // Check whether we reached the target
            if (distToTarget <= moveDist)
            {
                transform.position = targetPos;
                tileSet = false;
            }
            // Else, move this object towards it
            else
            {
                transform.position += targetDirection * moveSpeed * Time.deltaTime;
            }
        }
    }
}
