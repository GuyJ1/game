/// @author: Bryson Squibb
/// @date: 10/08/2022
/// @description: this script is responsible for
/// holding the data of tile objects

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    // Position
    [SerializeField] public Vector2Int position;

    // Character game object reference
    public GameObject characterOn = null;

    // Flags
    public bool hasCharacter = false;
    public bool highlighted = false;
    public bool passable = true;
}
