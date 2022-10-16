using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{

    [SerializeField]
    public int HP; //Current health
    public int HPMAX; //Maximum health
    public int STR; //Strength
    public int DEF; //Defense
    public int SPD; //Speed
    public int DEX; //Dexterity
    public int LCK; //Luck
    public int MV; //Movement
    public int Morale; //Morale (from 0 - 100), 
                        //depending on this value, ATK/CRIT are boosted from +1 to +5 and HIT/AVO is boosted by +2 to +10
    public int MoraleMAX; //Maximum Morale
    public int ATK; //Attack power (= STR - Player's DEF)
    public int HIT; //Hit  (= (DEX*3 + LCK) / 2)
    public int CRIT; //Critical  (= (DEX / 2) - 5)
    public int AVO; //Avoid  (= (SPD*3 + LCK) / 2)

    public int HitRate; // = HIT - Player's AVO
    public int CritRate; // = CRIT - Player's LCK
    
    public string Name;


    // Start is called before the first frame update
    void Start()
    {

        // Set health and morale
        HP = HPMAX;

        Morale = MoraleMAX;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // HP changed (either taking damage (negative) or healing (positive))
    public void adjustHP(int change)
    {
        HP += change;

        if(HP < 0){
            HP = 0;
        }

        if(HP > HPMAX){
            HP = HPMAX;
        }


    }

    //Morale changed (either positive or negative)
    public void adjustMorale(int change){
        Morale += change;
        if(Morale < 0){
            Morale = 0;
        }
        if(Morale > MoraleMAX){
            Morale = MoraleMAX;
        }
    }

    public int Attack(CharacterStats target){

        ATK = (STR + (Morale / 5)) - target.DEF;
        target.adjustHP(-ATK);

        return ATK;
    }
}
