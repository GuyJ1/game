using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoes : Accessory
{
    [SerializeField]
    public int MVmodifier;
    public int SPDmodifier;

    // Start is called before the first frame update
    void Start()
    {
        Type = 5;
        
    }


    //directly modify the unit's stats
    public void statBonus(CharacterStats target){

        target.MV += MVmodifier;
        target.SPD += SPDmodifier;
    }

    public void removeBonus(CharacterStats target){

        target.MV -= MVmodifier;
        target.SPD -= SPDmodifier;
    }
}
