using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bracelet : Accessory
{
    [SerializeField]
    public int APmodifier;
    public int LCKmodifier;

    // Start is called before the first frame update
    void Start()
    {
        Type = 4;
        
    }


    //directly modify the unit's stats
    public void statBonus(CharacterStats target){

        target.APMAX += APmodifier;
        target.LCK += LCKmodifier;
    }
}
