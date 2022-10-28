using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hat : Accessory
{
    [SerializeField]
    public int STRmodifier;
    public int DEFmodifier;
    public int SPDmodifier;
    public int DEXmodifier;

    // Start is called before the first frame update
    void Start()
    {
        Type = 1;
        
    }


    //directly modifies the unit's stats
    public void statBonus(CharacterStats target){

        target.STR += STRmodifier;
        target.DEF += DEFmodifier;
        target.SPD += SPDmodifier;
        target.DEX += DEXmodifier;

    }


}

