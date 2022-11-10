using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amulet : Accessory
{
    [SerializeField]
    public int HPmodifier;
    public int LCKmodifier;
    public int HealModifier;

    // Start is called before the first frame update
    void Start()
    {
        Type = 3;
        
    }

    //direcltly modifies the unit's stats
    public void statBonus(CharacterStats target){

        target.HPMAX += HPmodifier;
        target.LCK += LCKmodifier;
        
    }
}
