using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    [SerializeField] public float moveAmount;

    // Move Forward
    void Update()
    {
        Vector3 myPos = transform.position;

        transform.position = new Vector3(myPos.x, myPos.y, myPos.z + moveAmount);
    }
}
