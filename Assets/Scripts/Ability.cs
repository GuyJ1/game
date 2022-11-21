using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

  [CreateAssetMenu] //allows creating of item assets directly unity by right clicking project
public class Ability : ScriptableObject
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
    public int knockback; //Movement to apply to targets (positive is push, negative is pull)
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

    // Return knockback value rotated dependent on which way user is facing
    public Vector2Int getMovement(int xDist, int yDist) {
        if(Mathf.Abs(xDist) >= Mathf.Abs(yDist)) {
            if(xDist > 0) return new Vector2Int(-knockback, 0);
            else return new Vector2Int(knockback, 0);
        }
        else if(yDist > 0) return new Vector2Int(0, -knockback);
        else return new Vector2Int(0, knockback);
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

    //call an ability based on given display name
    public void callAbility(CharacterStats user, CharacterStats target){


        /// COMBO ///

        if(displayName == "Combo")                  Combo(user, target);


        /// WEAPON ///


        else if(displayName == "Pierce")            Pierce(user, target);
        else if(displayName == "Shooting Star")     ShootingStar(user, target);
        else if(displayName == "Siphon")            Siphon(user, target);
        else if(displayName == "Life Swap")         LifeSwap(user, target);
        else if(displayName == "Soul Bash")         SoulBash(user, target);
        else if(displayName == "Persistence")       Persistence(user, target);

        /// PIRATE ////

        else if(displayName == "Pistol Shot")       PistolShot(user, target);
        else if(displayName == "Head Shot")         HeadShot(user, target);
        else if(displayName == "Flank")             Flank(user);

        /// BARD ///

        else if(displayName == "Inspiring Shanty")  InspiringShanty(user, target);
        else if(displayName == "Reckless Tune")     RecklessTune(user, target);
        else if(displayName == "Maddening Jig")     MaddeningJig(user, target);

        /// MERCENARY ///


        else if(displayName == "Provoke")           Provoke(user, target);
        else if(displayName == "Reeling Chains")    ReelingChains(user, target);
        else if(displayName == "Incarcerate")       Incarcerate(user, target);


        /// MERCHANT ///


        else if(displayName == "Disparage")         Disparage(user, target);
        else if(displayName == "Promise of Coin")   PromiseOfCoin(user, target);
        else if(displayName == "On The House")      OnTheHouse(user, target);


        /// BUCCANEER ///


        else if(displayName == "Rifle Shot")        RifleShot(user, target);
        else if(displayName == "Cheap Shot")        CheapShot(user, target);
        else if(displayName == "Explosive Blast")   ExplosiveBlast(user, target);


        /// ACOLYTE ///


        else if(displayName == "Replenish")         Replenish(user, target);
        else if(displayName == "Noble Rite")        NobleRite(user, target);
        else if(displayName == "Purge")             Purge(user, target);


        /// PIRATE CAPTAIN ///


        else if(displayName == "Pistol Volley")     PistolVolley(user, target);
        else if(displayName == "Death Wish")        DeathWish(user, target);
        else if(displayName == "Second Wind")       SecondWind(user);
        else if(displayName == "Whirling Steel")    WhirlingSteel(user, target);


        /// MERCHANT CAPTAIN ///


        else if(displayName == "Tempt")             Tempt(user, target);
        else if(displayName == "Forceful Lashing")  ForcefulLashing(user, target);
        else if(displayName == "Harrowing Speech")  HarrowingSpeech(user, target);
        else if(displayName == "Silver Tongue")     SilverTongue(user, target);


        /// MONARCHY CAPTAIN ///


        else if(displayName == "Command")           Command(user, target);
        else if(displayName == "Shield Bash")       ShieldBash(user, target);
        else if(displayName == "Enrage")            Enrage(user, target);
        else if(displayName == "King's Will")       KingsWill(user, target);


        /// BASIC ///
        else Generic(user, target);

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

                    target.adjustHP(-totalDMG - baseDMG, false);

                }
            }
            else
            {
                GameObject hitParticle = Instantiate(targetEffect);
                hitParticle.transform.position = target.transform.position;

                totalDMG = user.Attack(target, 1);

                target.adjustHP(-totalDMG - baseDMG, false);
            }
        }
        else
        {//heal an ally

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            totalHP = user.Heal(target);

            target.adjustHP(totalHP + baseHP, true);
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

    //combo attacks will ignore defense if they hit
    void Combo(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            totalDMG = (baseDMG + user.getATK(target, true));

            target.adjustHP(-totalDMG, false);


        }

    }

/// PIRATE CLASS ABILITIES ///

    //attack the enemy, does not crit
    void PistolShot(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            totalDMG = user.getATK(target, false) + baseDMG;

            target.adjustHP(-totalDMG, false);


        }
        


    }

    //attack the enemy with high damage, ignores defense, does not crit
    void HeadShot(CharacterStats user, CharacterStats target){


        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            totalDMG = user.getATK(target, true) + baseDMG;

            target.adjustHP(-totalDMG, false);


        }



        
    }



    //Move to any tile around adjacent enemeies, also grants +2 SPD and DEX for 2 turns
    void Flank(CharacterStats user){

        //effect

        foreach(StatModifier modifier in selfModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) user.addModifier(modifier.clone());
        }

    }


/// BARD CLASS ABILITIES ///

    //buffs the target's STR and SPD for 3 turns (+5). Friendly ability
    void InspiringShanty(CharacterStats user, CharacterStats target){

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        foreach(StatModifier modifier in targetModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
        }



    }

    //heal the target and grant +1 AP, but debuff defense for 3 turns (-3). Friendly ability
    void RecklessTune(CharacterStats user, CharacterStats target){

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        totalHP = baseHP + user.Heal(target);

        target.adjustHP(totalHP, true);

        target.adjustAP(1,0);

        
        foreach(StatModifier modifier in targetModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
        }



    }


    //if ability hits, debuff target's DEF (-5) and accuraccy (DEX) (-5) for 3 turns
    void MaddeningJig(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            //possible effect

            foreach(StatModifier modifier in targetModifiers) {
                if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
            }
        }

    }


/// MERCENARY CLASS ABILITIES ///

    //Taunt: +5 STR/SPD / -10 DEF/LCK/DEX to target. Lasts for 1 turn
    void Provoke(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            foreach(StatModifier modifier in targetModifiers) {
                if(modifier.chance > Random.Range(0.0F, 1.0F)) target.addModifier(modifier.clone());
            }
        }

    }

    //Low damage, pull enemy towards the user, also grants +3 DEX to user for 3 turns
    void ReelingChains(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            totalDMG = baseDMG + (user.getDexterity() / 3);

            target.adjustHP(-totalDMG, false);

            foreach(StatModifier modifier in selfModifiers) {
                if(modifier.chance > Random.Range(0.0F, 1.0F)) user.addModifier(modifier.clone());
            }
        }

    }



    //medium damage, debuff SPD (-8) and MV (-3) to target for 2 turns
    void Incarcerate(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            totalDMG = baseDMG + (user.getDexterity() / 2);

            target.adjustHP(-totalDMG, false);

            foreach(StatModifier modifier in targetModifiers) {
                if(modifier.chance > Random.Range(0.0F, 1.0F)) user.addModifier(modifier.clone());
            }
        }
 
    }



/// MERCHANT CLASS ABILITIES ///

    // -6 STR -4 DEX to target for 1 turns
    void Disparage(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            //effect

            foreach(StatModifier modifier in targetModifiers) {
                if(modifier.chance > Random.Range(0.0F, 1.0F)) user.addModifier(modifier.clone());
            }
        }
        
    }



    // +6 DEF +4 LCK to target for 2 turns
    void PromiseOfCoin(CharacterStats user, CharacterStats target){

            //effect

            foreach(StatModifier modifier in targetModifiers) {
                if(modifier.chance > Random.Range(0.0F, 1.0F)) user.addModifier(modifier.clone());
            }
    
    }

    
    //Grants one random buff to target for 3 turns (if move is successful)
    //Can choose from any stat (minus MV, AP, or HP)
    // +5 STR (90% chance)
    // +5 DEF (70% chance)
    // +5 SPD (50% chance)
    // +5 DEX (30% chance)
    // +5 LCK (10% chance)
    void OnTheHouse(CharacterStats user, CharacterStats target){

        //effect

        foreach(StatModifier modifier in targetModifiers) {
            if(modifier.chance > Random.Range(0.0F, 1.0F)) user.addModifier(modifier.clone());
        }
    
    }





/// BUCCANEER CLASS ABILITIES ///

    //moderate damage, can convert remaining movement to +5 ACC
    void RifleShot(CharacterStats user, CharacterStats target){






        
    }


    //low damage, -2 MV to target
    void CheapShot(CharacterStats user, CharacterStats target){






        
    }


    //medium damage if move successful
    void ExplosiveBlast(CharacterStats user, CharacterStats target){






        
    }




/// ACOLYTE CLASS ABILITIES ///

    //heal the target (moderate)
    void Replenish(CharacterStats user, CharacterStats target){






        
    }



    //heal the target (strong) and grant +6 LCK
    void NobleRite(CharacterStats user, CharacterStats target){






        
    }


    //clear all debuffs and grant 1 AP to target
    void Purge(CharacterStats user, CharacterStats target){






        
    }






/// GENERAL WEAPON ABILITIES ///

    //Attack the enemy and then heal (= Attack Power)
    void Siphon(CharacterStats user, CharacterStats target){

        int modifier = 0;

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        totalDMG = user.Attack(target, 1);

        target.adjustHP(-totalDMG, false);

        if(user.weapon != null && user.weapon.strongSiphon){//Strong Siphon gives 20% more HP

            modifier = (totalDMG * 2) / 10;
        }

        user.adjustHP(totalDMG + modifier, true);

    }

    //attack while ignorning enemy defenses (base damage = 10)
    void Pierce(CharacterStats user, CharacterStats target){

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        totalDMG = user.Attack(target, 2);
        target.adjustHP(-totalDMG - baseDMG, false);

        

    }

    //attack the enemy 5 times, with each attack having 1/5 the power of a normal attack
    void ShootingStar(CharacterStats user, CharacterStats target){

        //attack 5 times for 1/5 of attack power, but with same HIT and CRIT

        GameObject hitParticle = null;

        int numAttack = 0;

        if(user.weapon != null && user.weapon.shiningStar){

            numAttack = 10;
        }
        else{

            numAttack = 5;
        }

        for(int i = 0; i < numAttack; i++){

            hitParticle = Instantiate(targetEffect);//?
            hitParticle.transform.position = target.transform.position;

            totalDMG = user.Attack(target, 1) / numAttack;
            target.adjustHP(-totalDMG, false);
            //might need a sleep statement here
            
        }

        

    }

    //Swap current HP with the target.
    //This move does not kill the user/target (reduces HP to a max of 1)
    void LifeSwap(CharacterStats user, CharacterStats target){

        int DEX = user.getDexterity(), STR = user.getStrength(), LCK = user.getLuck();
        totalACC = ((((DEX * 3 + LCK) / 2) + (2 * (user.Morale / 20))) - target.AVO) + user.accessoryBonus(1) + baseACC;

        int modifier = 0;

        if(user.determineHIT(totalACC)){

            totalHP = target.HP - user.HP;

            if(user.weapon != null && user.weapon.safeSwap){

                modifier = user.Heal(user);

            }

            user.adjustHP(totalHP + modifier, true);
            target.adjustHP(totalHP, true);
        }
    }

    //Attack the target. Bonus dmg is applied depending on current HP (more HP, more dmg)
    void SoulBash(CharacterStats user, CharacterStats target){

        baseDMG = user.Attack(target, 1);

        if(user.weapon != null && user.weapon.strongSoul && user.isFullHealth()){

            totalDMG = baseDMG + user.HP;

        }
        else{

            totalDMG = baseDMG + (user.HP / 3);

        }

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        target.adjustHP(-totalDMG, false);


    }

    //Attack the target. Bonus dmg is applied depending on current HP (less HP, more dmg)
    void Persistence(CharacterStats user, CharacterStats target){

        baseDMG = user.Attack(target, 1);

        totalDMG = baseDMG + (user.getMaxHP() - user.HP);

        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;

        target.adjustHP(-totalDMG, false);
    }

    


/// PIRATE CAPTAIN ABILITIES ///

    void WhirlingSteel(CharacterStats user, CharacterStats target){

        //blank for now
        GameObject hitParticle = Instantiate(targetEffect);
        hitParticle.transform.position = target.transform.position;


        target.adjustHP(-totalDMG - baseDMG, false);
    }

    void SecondWind(CharacterStats user){


        totalHP = user.Heal(user) + baseHP;

        user.adjustHP(totalHP, true);
    }

    void PistolVolley(CharacterStats user, CharacterStats target){

        for(int i = 0; i < 3; i++){

            if(user.determineHIT(baseACC)){

                GameObject hitParticle = Instantiate(targetEffect);
                hitParticle.transform.position = target.transform.position;

                target.adjustHP(-totalDMG - baseDMG, false);

            }

        }
    }

    void DeathWish(CharacterStats user, CharacterStats target){

        if(user.determineHIT(baseACC)){

            GameObject hitParticle = Instantiate(targetEffect);
            hitParticle.transform.position = target.transform.position;

            target.adjustHP(-totalDMG - baseDMG, false);

            if(target.isDead()){

                user.adjustAP(1,0);
            }




        }





    }




/// MERCHANT CAPTAIN ABILITIES ///



    //-10 DEF, resist enemy Charm
    void Tempt(CharacterStats user, CharacterStats target){






        
    }



    //Low damage, knockback 1, inflicts [Taunt]
    //Taunt: +5 STR/SPD / -10 DEF/LCK/DEX to target
    void ForcefulLashing(CharacterStats user, CharacterStats target){






        
    }


    //Lower enemy morale by 3 for each target hit
    void HarrowingSpeech(CharacterStats user, CharacterStats target){






        
    }



    //Inflict [Charm]
    //Charm: -8 STR/DEF for 2 turns, Can't use abilities on user for 1 turn
    void SilverTongue(CharacterStats user, CharacterStats target){






        
    }








/// MONARCHY CAPTAIN ABILITIES ///



    //+7 STR/DEX/LCK to target
    void Command(CharacterStats user, CharacterStats target){






        
    }


    //medium damage, knockback 3, self forward 1
    //if armor is equipped, bonus damage = user's DEF / 3
    void ShieldBash(CharacterStats user, CharacterStats target){






        
    }


    //inflicts [Taunt]
    //Taunt: +5 STR/SPD / -10 DEF/LCK/DEX to target
    void Enrage(CharacterStats user, CharacterStats target){






        
    }



    //immune to damage for 1 turn
    void KingsWill(CharacterStats user, CharacterStats target){






        
    }





    
}