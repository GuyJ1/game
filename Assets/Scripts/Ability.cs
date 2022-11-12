using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ability : MonoBehaviour
{
    public bool friendly; //Whether this ability targets allies or enemies (true for allies, false for enemies)
    public bool requiresTarget; //Whether this ability requires a selected target to execute
    public int totalDMG; //total damage dealt 
    public int totalHP; //total HP healed
    public int cost; //raw cost value
    public int costType; //Depending on this value, the a different resource will be used fot this ability
                         //AP = 0, HP = 1, Morale = 2, Gold = 3, Food = 4, etc.
    public int range;
    public int ID; //every ability has a unique ID. Can be used for randomly assigning abilities to characters/enemies
    public string displayName; //Display name for UI
    public List<Vector2Int> shape; //Shape in tiles facing north (0,0) is the center

    [SerializeField] public GameObject targetEffect; //visual effect

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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
        callAbility(this.ID, user.GetComponent<CharacterStats>(), target.GetComponent<CharacterStats>());
    }

    // Apply ability to target list of characters
    public void affectCharacters(GameObject user, List<GameObject> targets) {
        foreach(GameObject target in targets) {
            callAbility(this.ID, user.GetComponent<CharacterStats>(), target.GetComponent<CharacterStats>());
        }
    }

    public int callAbility(int ID, CharacterStats user, CharacterStats target){
        //switch statement with all IDs calling the specified function

        if(costType == 0 && user.AP < cost){

            return 1; //not enough AP
        }
  
        switch(ID){
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

        }

        return 0;
            

    }

    //Ability implementations

    //Generic ability with no special effects
    void Generic(CharacterStats user, CharacterStats target) {
        if(!friendly)
        {//attack the enemy
            if(user.weapon != null && (user.weapon.doubleAttack && (user.SPD - target.SPD >= 5))){//if the equipped weapon can double attack

                for(int i = 0; i < 2; i++){

                    GameObject hitParticle = Instantiate(targetEffect);
                    hitParticle.transform.position = target.transform.position;

                    totalDMG = user.Attack(target, 1);

                    target.adjustHP(-totalDMG);

                }
            }
            else{


                GameObject hitParticle = Instantiate(targetEffect);
                hitParticle.transform.position = target.transform.position;

                totalDMG = user.Attack(target, 1);

                target.adjustHP(-totalDMG);
            }
        }
        else
        {//heal an ally

            //particle effect here

            totalHP += user.Heal(target);

            target.adjustHP(totalHP);
        }
        
    }

    void Combo(CharacterStats user, CharacterStats target){

        //not sure how this will work yet so it will be blank for now

        target.adjustHP(-totalDMG);

        

    }

    void PistolShot(CharacterStats user, CharacterStats target){

        //see above

        

        target.adjustHP(-totalDMG);
        


    }

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

    //attack while ignorning enemy defenses
    void Pierce(CharacterStats user, CharacterStats target){

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        totalDMG = user.Attack(target, 2);
        target.adjustHP(-totalDMG);

        

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





    
}
