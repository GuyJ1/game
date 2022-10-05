/// @author: Bryson Squibb
/// @date: 10/04/2022
/// @description: this script is responsible for
/// holding the data of tile objects

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    // Position and Flags
    [SerializeField] public Vector2Int position;

    public GameObject characterOn = null;
    public bool hasCharacter = false;
    public bool passable = true;
}
