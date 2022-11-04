using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField]
    public string Name;
    public string Description;
    public int MGT; //Might. Total attack is increased/decreased with this stat.
    public int Durability; //Each use of this weapon will decrease its durability by 1. When this reaches 0, the weapon breaks.
    public int DurabilityMAX;
    public int Range;
    public bool isBroken;

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
}
