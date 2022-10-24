using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ability : MonoBehaviour
{
    public bool friendly; //Whether this ability targets allies or enemies
    public bool requiresTarget; //Whether this ability requires a selected target to execute
    public int totalDMG;
    public int totalHP;
    public int cost;
    public int costType; //Depending on this value, the a different resource will be used fot this ability
                         //AP = 0, HP = 1, Morale = 2, Gold = 3, Food = 4, etc.
    public int range;
    public int ID; //every ability has a unique ID. Can be used for randomly assigning abilities to characters/enemies

    public Ability(bool friendly, int totalDMG, int totalHP, int cost, int costType, int range, int ID) {
        this.friendly = friendly;
        this.totalDMG = totalDMG;
        this.totalHP = totalHP;
        this.cost = cost;
        this.cost = costType;
        this.range = range;
        this.ID = ID;
        
    }

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Apply ability to this list of characters
    public void affectCharacters(GameObject user, List<GameObject> targets) {
        foreach(GameObject target in targets) {
            callAbility(this.ID, user.GetComponent<CharacterStats>(), target.GetComponent<CharacterStats>());
        }
    }

    public int callAbility(int ID, CharacterStats user, CharacterStats target){
        //switch statement with all IDs calling the specified function
        switch(ID){
            case 0: case 1: 
                if(Generic(user, target) == 1) return 1;
                break;
            case 2:
                if(Siphon(user, target) == 1){
                    return 1; //Not enough AP
                }
                
            break;
            case 3:
                if(Pierce(user, target) == 1){
                    return 1;
                }
                
            break;
            case 4:
                if(ShootingStar(user, target) == 1){
                    return 1;
                }
            
            break;
            default:
                return 1; //error

        }

        return 0;
            

    }

    //ability implementations

    //Generic ability with no special effects
    int Generic(CharacterStats user, CharacterStats target) {
        if(!friendly) target.adjustHP(-totalDMG);
        else target.adjustHP(totalHP);
        return 0;
    }

    //Attack the enemy and then heal (= Attack Power)
    int Siphon(CharacterStats user, CharacterStats target){

        if(user.adjustAP(-10, 1) == 1){
            return 1; //Not enough AP!
        }

        user.adjustHP(user.Attack(target));

        return 0;


    }

    //attack while ignorning enemy defenses
    int Pierce(CharacterStats user, CharacterStats target){

        if(user.adjustAP(-25, 1) == 1){
            return 1; //Not enough AP!
        }

        user.ATK = (user.STR + (user.Morale / 5));
        target.adjustHP(-(user.ATK));

        return 0;

    }

    //attack the enemy 5 times, with each attack having 1/5 the power of a normal attack
    int ShootingStar(CharacterStats user, CharacterStats target){

        if(user.adjustAP(-15, 1) == 1){
            return 1; //Not enough AP!
        }

        //attack 5 times for 1/5 of attack power, but with same HIT and CRIT
        return 0;

    }





    
}
