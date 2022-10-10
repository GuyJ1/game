using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{

    [SerializeField]
    public int HP;
    public int HPMAX;
    public int STR;
    public int DEF;
    public int SPD;
    public int MV;
    public string Name;

    // Character's logical position on the grid
    public Vector2Int gridPosition;

    // Start is called before the first frame update
    void Start()
    {


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
