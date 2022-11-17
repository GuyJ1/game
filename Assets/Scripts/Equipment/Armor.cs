using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : MonoBehaviour
{

    [SerializeField]

    public string Name;
    public string Description;
    public int RES; //Resistance. Total defense is increased/decreased with this stat.
    public int CON; //Constitution. Depending on this value compared to unit's STR, total movement allowed will be decreased, from 1 to 5.


    public void removeRES(CharacterStats target){


        target.DEF -= RES;
    }

}
