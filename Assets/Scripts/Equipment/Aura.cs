using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aura : Accessory
{
    [SerializeField]
    public int STRmodifier;
    public int DEFmodifier;
    public int SPDmodifier;
    public int DEXmodifier;
    public int ATKmodifier;
    public int HITmodifier;
    public int CRITmodifier;
    public int AVOmodifier;
    public int HPmodifier;
    public int APmodifier;
    public int LCKmodifier;
    public int MVmodifier;
    //public int MoraleModifier;
    public int HealModifier;


    // Start is called before the first frame update
    void Start()
    {
        Type = 6;
        
    }

    //directly modifies the unit's stats
    public void statBonus(CharacterStats target){

        target.STR += STRmodifier;
        target.DEF += DEFmodifier;
        target.SPD += SPDmodifier;
        target.DEX += DEXmodifier;
        target.HPMAX += HPmodifier;
        target.APMAX += APmodifier;
        target.LCK += LCKmodifier;
        target.MV += MVmodifier;
        //target.MoraleMIN += MoraleModifier;
        

    }

    public void removeBonus(CharacterStats target){

        target.STR -= STRmodifier;
        target.DEF -= DEFmodifier;
        target.SPD -= SPDmodifier;
        target.DEX -= DEXmodifier;
        target.HPMAX -= HPmodifier;
        target.APMAX -= APmodifier;
        target.LCK -= LCKmodifier;
        target.MV -= MVmodifier;
        //target.MoraleMIN -= MoraleModifier;
        target.AVO -= AVOmodifier;
        

    }

    //return either ATK (0), HIT (1), or CRIT (2)
    public int battleBonus(int type){

        switch(type){
            case 0:
                return ATKmodifier;
            case 1:
                return HITmodifier;
            case 2:
                return CRITmodifier;
            default:
                return 0; //error
        }

    }

}
