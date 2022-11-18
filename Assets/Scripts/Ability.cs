using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class Ability : MonoBehaviour
{
    public bool friendly; //Whether this ability targets allies or enemies (true for allies, false for enemies)
    public bool requiresTarget; //Whether this ability requires a selected target to execute
    public int totalDMG; //total damage dealt 
    public int totalHP; //total HP healed
    public int totalACC; //chance the ability hits
    public int baseDMG; //ability will always do a certain amount of damage regardless of DEF
    public int baseHP; //ability will always heal a certain amount of HP
    public int baseACC; //ability comes with a set accuracy
    public int cost; //AP cost
    public int range; //Distance from the user this ability can be used at
    public int movement; //Movement to apply to targets (positive is push, negative is pull)
    public string displayName; //Display name for UI
    public List<Vector2Int> shape; //Shape in tiles facing north (0,0) is the center
    public List<StatModifier> targetModifiers; //Stat modifiers to apply to targets
    public List<StatModifier> selfModifiers; //Stat modifiers to apply to self
    [SerializeField] public GameObject targetEffect; //visual effect

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // Return movement value rotated dependent on which way user is facing
    public Vector2Int getMovement(int xDist, int yDist) {
        if(Mathf.Abs(xDist) >= Mathf.Abs(yDist)) {
            if(xDist > 0) return new Vector2Int(-movement, 0);
            else return new Vector2Int(movement, 0);
        }
        else if(yDist > 0) return new Vector2Int(0, -movement);
        else return new Vector2Int(0, movement);
    }

    // Return shape rotated dependent on which way user is facing
    public List<Vector2Int> getRelativeShape(int xDist, int yDist) {
        List<Vector2Int> list = new List<Vector2Int>();
        foreach(Vector2Int pos in shape) {
            //Rotate position relative to user
            if(Mathf.Abs(xDist) >= Mathf.Abs(yDist))
            {
                if(xDist > 0) list.Add(new Vector2Int(-pos.y, pos.x)); //West
                else list.Add(new Vector2Int(pos.y, pos.x)); //East
            }
            else if(yDist > 0) list.Add(new Vector2Int(-pos.x, -pos.y)); //South
            else list.Add(pos); //North
        }
        return list;
    }

    // Apply ability to target character
    public void affectCharacter(GameObject user, GameObject target) {
        callAbility(user.GetComponent<CharacterStats>(), target.GetComponent<CharacterStats>());
    }

    // Apply ability to target list of characters
    public void affectCharacters(GameObject user, List<GameObject> targets) {
        foreach(GameObject target in targets) {
            callAbility(user.GetComponent<CharacterStats>(), target.GetComponent<CharacterStats>());
        }
    }

    public void callAbility(CharacterStats user, CharacterStats target){
        //switch statement with all IDs calling the specified function
        //Generic(user, target);
        //make use of displayName

        /*if(displayName == "Basic"){

            Generic(user, target);

        }
        else if(displayName == "Combo"){

            Combo(user, target);

        }
        else*/ if(displayName == "Pierce"){

            Pierce(user, target);

        }
        else if(displayName == "Shooting Star"){

            ShootingStar(user, target);

        }
        else if(displayName == "Siphon"){

             Siphon(user, target);

        }
        /*else if(displayName == "Pistol Shot"){

            PistolShot(user, target);

        }*/
        else Generic(user, target);



        /*switch(ID){
            case 0: 
                Generic(user,target);
                break;

            case 1:
                Combo(user, target);
                break;
            case 2:
                
                PistolShot(user, target);
                break;
            case 3:

                Siphon(user, target);
                break;
            case 4:

                Pierce(user, target);
                break;
            case 5:

                ShootingStar(user, target);
                break;
                
            default:
                return 1; //error

        }*/

        
            

    }

    //Ability implementations

//// GENERAL ABILITIES ///

    //Generic ability with no special effects
    void Generic(CharacterStats user, CharacterStats target) {
        if(!friendly)
        {//attack the enemy
            if(user.weapon != null && (user.weapon.doubleAttack && (user.getSpeed() - target.getSpeed() >= 5))){//if the equipped weapon can double attack

                for(int i = 0; i < 2; i++){

                    GameObject hitParticle = Instantiate(targetEffect);
                    hitParticle.transform.position = target.transform.position;

                    totalDMG = user.Attack(target, 1);

                    target.adjustHP(-totalDMG - baseDMG);

                }
            }
            else
            {
                GameObject hitParticle = Instantiate(targetEffect);
                hitParticle.transform.position = target.transform.position;

                totalDMG = user.Attack(target, 1);

                target.adjustHP(-totalDMG - baseDMG);
            }
        }
        else
        {//heal an ally

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            totalHP = user.Heal(target);

            target.adjustHP(totalHP + baseHP);
        }
        //Apply modifiers to self
        foreach(StatModifier modifier in selfModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) user.addModifier(modifier.clone());
        }
        //Apply modifiers to target
        foreach(StatModifier modifier in targetModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
        }
    }

    void Combo(CharacterStats user, CharacterStats target){

        //not sure how this will work yet so it will be blank for now

        target.adjustHP(-totalDMG);

        

    }

/// PIRATE CLASS ABILITIES ///

    void PistolShot(CharacterStats user, CharacterStats target){

        //see above

        

        target.adjustHP(-totalDMG - baseDMG);
        


    }



/// GENERAL WEAPON ABILITIES ///

    //Attack the enemy and then heal (= Attack Power)
    void Siphon(CharacterStats user, CharacterStats target){

        int modifier = 0;

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        totalDMG = user.Attack(target, 1);

        target.adjustHP(-totalDMG);

        if(user.weapon != null && user.weapon.strongSiphon){//Strong Siphon gives 20% more HP

            modifier = (totalDMG * 2) / 10;
        }

        user.adjustHP(totalDMG + modifier);

    }

    //attack while ignorning enemy defenses (base damage = 10)
    void Pierce(CharacterStats user, CharacterStats target){

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        totalDMG = user.Attack(target, 2);
        target.adjustHP(-totalDMG - baseDMG);

        

    }

    //attack the enemy 5 times, with each attack having 1/5 the power of a normal attack
    void ShootingStar(CharacterStats user, CharacterStats target){

        //attack 5 times for 1/5 of attack power, but with same HIT and CRIT

        GameObject hitParticle = null;

        for(int i = 0; i < 5; i++){

            hitParticle = Instantiate(targetEffect);//?
            hitParticle.transform.position = target.transform.position;

            totalDMG = user.Attack(target, 1) / 5;
            target.adjustHP(-totalDMG);
            //might need a sleep statement here
            
        }

        

    }

/// PIRATE CAPTAIN ABILITIES ///

    void WhirlingSteel(CharacterStats user, CharacterStats target){

        //blank for now



        target.adjustHP(-totalDMG - baseDMG);
    }

    void SecondWind(CharacterStats user){


        totalHP = user.Heal(user);

        user.adjustHP(totalHP + baseHP);
    }





    
}