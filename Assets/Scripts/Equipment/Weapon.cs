using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{


    public int Durability; //Each use of this weapon will decrease its durability by 1. When this reaches 0, the weapon breaks.
    public bool isBroken;


    //PASSIVE WEAPON ABILITIES (false by default)
    public bool doubleAttack = false; //certain weapons are able to attack again if the unit's SPD - target's SPD >= 5 
    public bool strongSiphon = false; //when using Siphon, user will heal 20% more HP
    public bool deadlyPierce = false; //when using Pierce, CRIT +10
    

    //Depdning on the type of weapon, these will be different. These only apply when the weapon is equipped
    public int HPmod;
    public int DEFmod;
    public int SPDmod;
    public int DEXmod;
    public int LCKmod;

    public int Rarity; //from 1 to 3.
    

    

    [SerializeField]
    public string Name;
    public string Description;
    public int MGT; //Might. Total attack is increased/decreased with this stat.
    public int DurabilityMAX;
    public int Range;
    

    // Start is called before the first frame update
    void Start()
    {

        Durability = DurabilityMAX;
        isBroken = false;

        
    }


    public void weaponDamage(){

        if(isBroken){
            return;
        }

        Durability--;
        if(Durability == 0){
            isBroken = true;
        }
    }

    public void weaponDamage(int value){

        if(isBroken){
            return;
        }

        Durability -= value;
        if(Durability <= 0){
            Durability = 0;
            isBroken = true;
        }
    }

    public void modifyStats(CharacterStats target, bool isPositive){

        
        if(isPositive){

            target.HPMAX += HPmod;
            target.DEF += DEFmod;
            target.SPD += SPDmod;
            target.DEX += DEXmod;
            target.LCK += LCKmod;
        }
        else{

            target.HPMAX -= HPmod;
            target.DEF -= DEFmod;
            target.SPD -= SPDmod;
            target.DEX -= DEXmod;
            target.LCK -= LCKmod;



        }
    }
}
