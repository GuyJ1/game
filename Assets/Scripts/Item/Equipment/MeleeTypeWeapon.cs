using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeTypeWeapon : Weapon
{

    [SerializeField]
    int type; //0 = Sword, 1 = Scimitar, 2 = Saber
    List<GameObject> meleeAbilities; //depending on the rarity, the amount of abilities will = 1 to 3. Could possibly be randomly chosen
    


    // Start is called before the first frame update
    void Start()
    {

        //Determine modifiers
        switch(type){


            case 0:
                //Swords have no modifiers
                //Swords have Strong Siphon (get 20% HP when using Siphon)
                strongSiphon = true;
                strongSoul = true;
                betterHeal = true;
                earlyGambit = true;


                break;

            case 1:
                //The scimitar weapon sacrifices defenses for more mobility;
                //Scimitars can double attack
                /*HPmod = -5;
                DEFmod = -3;
                SPDmod = 5;
                DEXmod = 3;
                LCKmod = 2;*/
                doubleAttack = true;
                lastStand = true;
                shiningStar = true;
                certainty = true;
                

                break;
            case 2:

                //The saber weapon focuses on defensive/supportive tactics
                //Sabers have Deadly Pierce (+10 CRIT when using Pierce)
                /*HPmod = 5;
                DEFmod = 5;
                SPDmod = -3;
                DEXmod = -3;
                LCKmod = -2;*/
                deadlyPierce = true;
                safeSwap = true;
                betterHeal = true;
                lateGambit = true;

                
                

                break;
            default:
                //weapons unspecified will have no changes
                
               

                break;
        }






        
    }





}
