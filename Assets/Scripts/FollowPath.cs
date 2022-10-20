using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : MonoBehaviour
{
    // Movement
    public Stack<PathTreeNode> pathToFollow = new Stack<PathTreeNode>();
    private bool followingPath = false;
    public int travelSteps = 10;
    private int currentSteps = 0;
    private Vector3 targetPos;
    private Vector3 stepLength;

    // Update is called once per frame
    void LateUpdate()
    {
        // Follow Path
        if (pathToFollow.Count > 0 && currentSteps <= 0)
        {
            var currentNode = pathToFollow.Pop();
            Vector3 pos = currentNode.myTile.transform.position;
            targetPos = new Vector3(pos.x, pos.y + 0.5f, pos.z);
            stepLength = (targetPos - transform.position) / travelSteps;
            currentSteps = travelSteps;
        }

        if (currentSteps > 0)
        {
            followingPath = true;
            transform.position += stepLength;
        }
        else
        {
            followingPath = false;
        }

        currentSteps--;
    }
}
