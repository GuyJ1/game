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
    public int ATK; //Attack power (= STR - Player's DEF) +- difficultyAdjustment
    public int HIT; //Hit Rate (= (((DEX*3 + LCK) / 2) - Player's AVO) +- difficultyAdjustment
    public int CRIT; //Critical Rate (= ((DEX / 2) - 5) - Player's LCK) +- difficultyAdjustment
    public int AVO; //Avoid  (= (SPD*3 + LCK) / 2) +- difficultyAdjustment

    public int difficultyAdjustment; // can be positive or negative. Will adjust enemy ATK, HIT, CRIT, and AVO parameters depending on difficulty setting.
    public int difficultySetting; //5 difficulties from -2 (Very Easy) to +2 (Very Hard). diffcultyAdjustment = difficultySetting * 3
    
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

    //Attack the player, possibly with a critical hit
    //Note: Critical hits triple the total damage
    //Note: diffcultyAdjustment should be in the range of -6 to +6
    public int Attack(CharacterStats target){

        difficultyAdjustment = difficultySetting * 3;

        HIT = ((((DEX * 3 + LCK) / 2) + (2 * (Morale / 5))) - target.AVO) + difficultyAdjustment;
        CRIT = ((((DEX / 2) - 5) + (Morale / 5)) - target.LCK) + difficultyAdjustment;

        if(determineCRIT(CRIT)){

            ATK = (((STR + (Morale / 5)) - target.DEF) + difficultyAdjustment) * 3; //CRITICAL HIT!
            target.adjustHP(-ATK);

        }
        else if(determineHIT(HIT)){
            ATK = (((STR + (Morale / 5)) - target.DEF) + difficultyAdjustment); //HIT!
            target.adjustHP(-ATK);

        }
        else{

            ATK = 0; //Miss...
            
        }


        return ATK;
    }

    public bool determineHIT(int HIT){

       
        if(HIT >= Random.Range(0, 100)){
            return true;
        }
        else{
            return false;
        }
    }

    public bool determineCRIT(int CRIT){

        
        if(CRIT >= Random.Range(0,100)){
            return true;
        }
        else{
            return false;
        }
    }
}
