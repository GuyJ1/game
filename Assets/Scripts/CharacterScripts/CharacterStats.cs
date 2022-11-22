using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat
{
    public int baseValue, min, max;
    public StatType type;

    //Return value adjusted for modifiers
    public int getValue(List<StatModifier> modifiers)
    {
        float value = baseValue;
        //Add flat values first
        foreach(StatModifier modifier in modifiers)
        {
            if(modifier.type != type || modifier.op == OpType.MULTIPLY) continue;
            value += modifier.value;
        }
        float scalar = 1;
        //Combine into one large scalar for multiplication
        foreach(StatModifier modifier in modifiers)
        {
            if(modifier.type != type || modifier.op == OpType.ADD) continue;
            scalar += modifier.value;
        }
        //Multiply by scalar
        value *= scalar;
        return (int) Mathf.Round(Mathf.Clamp(value, min, max));
    }
}

[Serializable]
public class StatModifier
{
    public StatType type; //Target stat
    public OpType op; //Whether to do addition or multiplication
    public int duration; //Duration in turns (-1 for infinite)
    public float value; //Value for operation
    public float chance; //Chance to apply (0.0 to 1.0)
    public string id = ""; //Optional ID to identify this modifier after it's applied

    public StatModifier(StatType type, OpType op, int duration, float value, float chance) {
        this.type = type;
        this.op = op;
        this.duration = duration;
        this.value = value;
        this.chance = chance;
    }

    public StatModifier(StatType type, OpType op, int duration, float value, float chance, string id) : this(type, op, duration, value, chance) {
        this.id = id;
    }

    //Return true if this modifier is a buff, false if a debuff
    public bool isBuff() {
        if(op == OpType.ADD) return value >= 0;
        else return value >= 1;
    }

    public StatModifier clone() {
        return new StatModifier(type, op, duration, value, chance, id);
    }
}

public enum StatType {
    HPMAX, APMAX, STR, DEF, SPD, DEX, LCK, MV
}

public enum OpType
{
    ADD, MULTIPLY
}

public class CharacterStats : MonoBehaviour
{
    [SerializeField]
    public Stat HPMAX; //Maximum health
    public Stat APMAX; //Maximum ability points, default value is 3
    public Stat STR; //Strength
    public Stat DEF; //Defense
    public Stat SPD; //Agility
    public Stat DEX; //Dexterity
    public Stat LCK; //Luck
    public Stat MV; //Movement

    public int HP; //Current health
    public int AP; //Current ability points
    /*
    public int MoraleMAX; //Maximum Morale
    public int MoraleMIN; //Minimum Morale 
    */
    public int ATK; //Attack power (= STR - Enemy's DEF)
    public int HIT; //Hit Rate (= (((DEX*3 + LCK) / 2) - Enemy's AVO)
    public int CRIT; //Critical Rate (= ((DEX / 2) - 5) - Enemy's LCK)
    public int AVO; //Avoid  (= (SPD*3 + LCK) / 2)



    //Status Effects (Resist)
    public bool charmRES = false;
    public bool tauntRES = false;
    public bool DMGimmune = false;

    public List<StatModifier> statModifiers = new List<StatModifier>();

    public string Name;

    // Actions
    public Ability basicAttack, comboAttack; //Generic actions
    public List<Ability> abilities; //Unique abilities

    // Healthbar
    [SerializeField] public float healthBarScale;
    public HealthBar healthBar;
    public float healthBarYOffset;

    // Canvas reference
    public GameObject canvas;

    // UI
    public GameObject DamageText;

    // Model reference
    public GameObject model;

    // Character's logical position on the grid
    public Vector2Int gridPosition;
    public GameObject myGrid;

    // Crew that this character belongs to (should be set by CrewSystem, contains morale value)
    public GameObject crew;

    //Equipment
    public Weapon weapon; //increases ATK and can modify max HP, DEF, SPD, DEX, and LCK (either melee or ranged)
    public Armor armor; //increases DEF
    public Hat hat; //can increase STR, DEF, SPD, and/or DEX
    public Ring ring; //can increase ATK, HIT, CRIT, and/or AVO
    public Amulet amulet; //can increase max HP, LCK, and/or healing ability
    public Bracelet bracelet; //can increase max AP, LCK, and/or healing ability
    public Shoes shoes; //can increase MV and/or SPD
    public Aura aura; //can increase any stat

    public int Morale;

    // Start is called before the first frame update
    void Start()
    {
        // Attach healthbar to canvas
        canvas = GameObject.Find("UI Menu");
        healthBar = Instantiate(healthBar, canvas.transform);

        // Set gradient color
        bool isPlayer = crew.GetComponent<CrewSystem>().isPlayer;
        healthBar.gradient = HealthBarGradient(isPlayer);

        // Morale is now pulled from CrewSystem.morale. All characters now use a univeral morale value, with the same boosts
        Morale = crew.GetComponent<CrewSystem>().morale; //Morale (from 0 - 100), (from crew)
        //depending on this value, ATK/CRIT are boosted from +1 to +5 , HIT/AVO is boosted by +2 to +10 , and Healing is boosted by +1 to +10

        //MoraleMIN = 0;

        //Temporary name for player-controlled characters for logging: Will need to have functionality added later
        Name = "Pupperton";

        // Set health, ability points, etc.

        refreshStats();

        /*if(armor != null){
            DEF += armor.RES;
            if(STR < armor.CON){

                if(((armor.CON - STR) / 10) > 5){

                    MV -= 5;

                }
                else{

                    MV -= ((armor.CON - STR) / 10);
                }

            }

            if(MV < 1){
                MV = 1;
            }
            
        }*/

        HP = getMaxHP();

        healthBar.SetMaxHealth(getMaxHP());

        //Morale = MoraleMAX;

        AP = getMaxAP();

        updateAVO(ring, aura);
        HIT = getHIT(null, true);
        CRIT = getCRIT(null, true);
        ATK = getATK(null, true);





    }

    // Get a gradient for healthbar colors
    public static Gradient HealthBarGradient(bool isFriendy)
    {
        Gradient grad = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = isFriendy ? Color.yellow : new Color(255f/255f, 165f/255f, 0f, 1f);
        colorKey[0].time = 0.0f;
        colorKey[1].color = isFriendy ? Color.green : Color.red;
        colorKey[1].time = 1.0f;

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[1];
        alphaKey[0].alpha = 0.5f;

        grad.SetKeys(colorKey, alphaKey);

        return grad;
    }

    // Update is called once per frame
    void Update()
    {
        // Health Testing
        if (Input.GetKeyDown(KeyCode.Space))
        {

            //damage
            adjustHP(-20, false);
        }

        //updateAVO(ring, aura);
        //update equipment

    }

    // Update healthbar position
    void LateUpdate()
    {
        // Offset
        Vector3 posOffset = new Vector3(0, healthBarYOffset, 0);

        // --------- Update healthbar position ---------
        healthBar.transform.position = Camera.main.WorldToScreenPoint(model.transform.position + posOffset);

        // --------- Update scale based on camera position ---------
        float camDist = Vector3.Distance(Camera.main.transform.position, this.transform.position);
        float newScale = 0.0f;

        if (camDist != 0.0f)
        {
            newScale = healthBarScale / camDist;
        }

        healthBar.transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    // Disable the character and associated elements
    public void removeFromGrid() {
        this.gameObject.SetActive(false);
        healthBar.gameObject.SetActive(false);
    }

    // Whether this character is considered dead in battle
    public bool isDead() {
        return HP <= 0;
    }

    public bool isFullHealth(){

        return HP == getMaxHP();
    }

    // Return selectable abilities in battle (combo attack not included)
    public List<Ability> getBattleAbilities() {
        List<Ability> list = new List<Ability>();
        list.Add(basicAttack);
        foreach(Ability ability in abilities) list.Add(ability);
        return list;
    }

    // Clear all modifiers and re-apply equipment modifiers
    public void refreshStats() {
        clearModifiers();
        if(weapon != null) weapon.applyModifiers(this);
        if(hat != null) hat.applyModifiers(this);
        if(ring != null) ring.applyModifiers(this);
        if(amulet != null) amulet.applyModifiers(this);
        if(bracelet != null) bracelet.applyModifiers(this);
        if(shoes != null) shoes.applyModifiers(this);
        if(aura != null) aura.applyModifiers(this);
        if(armor != null) armor.applyModifiers(this);
    }

    // HP changed (either taking damage (negative) or healing (positive))
    // if dontKill is true, lethal damage will only reduce HP to 1
    public void adjustHP(int change, bool dontKill)
    {

        if(change < 0 && DMGimmune){

            change = 0;
        }

        HP += change;

        if(HP < 0 && !dontKill){
            HP = 0;
        }
        else if(HP <= 0 && dontKill){

            HP = 1;
        }

        if(HP > getMaxHP()){
            HP = getMaxHP();
        }

        // Display Damage
        DamageText.GetComponent<TextDamage>().damageToDisplay = change;
        GameObject damageDisplay = Instantiate(DamageText, canvas.transform);
        damageDisplay.transform.position = Camera.main.WorldToScreenPoint(model.transform.position);

        // Update Healthbar
        healthBar.SetHealth(HP);
    }



    //AP changed (either positive or negative)
    //subType is primarily for subtractions (either ability used (1) or AP drained (2) [0 if not subtraction])
    //returns 0 if adjustment was successful, 1 otherwise
    public int adjustAP(int change, int subType){

        int oldAP = AP;
        AP += change;

        if(subType != 0){

            if(AP < 0){

                if(subType == 1){

                    AP = oldAP; //not enough AP!
                    return 1;

                }
                else{

                    AP = 0;
                    return 0;

                }

            }

            return 0;
        }
        if(AP > getMaxAP()){
            AP = getMaxAP();
        }

        return 0;
    }

    public void tickModifiers() {
        List<StatModifier> removals = new List<StatModifier>();
        foreach(StatModifier modifier in statModifiers) {
            if(modifier.duration > -1) modifier.duration--;
            if(modifier.duration == 0) removals.Add(modifier);
        }
        foreach(StatModifier modifier in removals){

            statModifiers.Remove(modifier);
            if(modifier.id == "Immune"){
                DMGimmune = false;
            }

        } 
    }

    public void addModifier(StatModifier modifier) {
        statModifiers.Add(modifier.clone());
    }

    public void addModifiers(List<StatModifier> modifiers) {
        foreach(StatModifier modifier in modifiers) {
            addModifier(modifier);
        }
    }

    public void clearModifiersWithId(string id) {
        foreach(StatModifier modifier in statModifiers) {
            if(modifier.id == id) statModifiers.Remove(modifier);
        }
    }

    public void clearModifiers() {
        statModifiers.Clear();
    }

    public void clearDebuffs(){
        List<StatModifier> debuffs = new List<StatModifier>();
        foreach(StatModifier modifier in statModifiers) {
            if(!modifier.isBuff()) debuffs.Add(modifier);
        }
        foreach(StatModifier modifier in debuffs) statModifiers.Remove(modifier);

    }


    //Morale changed (either positive or negative)
    public void adjustMorale(int change){
        Morale += change;
        if(Morale < 0){
            Morale = 0;
        }
        if(Morale > 100){
            Morale = 100;
        }
    }


    //Attack the enemy, possibly with a critical hit
    //Note: Critical hits triple the total damage
    //Type 1 = general attack, nothing changes
    //Type 2 = no defenses, attack while target.DEF is ignored
    public int Attack(CharacterStats target, int type){
        int DEX = getDexterity(), STR = getStrength(), LCK = getLuck();
        HIT = ((((DEX * 3 + LCK) / 2) + (2 * (Morale / 20))) - target.AVO) + accessoryBonus(1);
        CRIT = ((((DEX / 2) - 5) + (Morale / 20)) - target.getLuck()) + accessoryBonus(2);

        if(type == 2 && (weapon != null && weapon.deadlyPierce)){//Deadly Pierce grants +10 CRIT

            CRIT += 10;
        }

        if(weapon != null && weapon.lastStand && HP == 1){//Last Stand gives a gauranteed crit

            ATK = ((STR + (Morale / 20) + weaponBonus() + accessoryBonus(0)) - target.getDefense()) * 3;
            return ATK;
        }

        if(determineCRIT(CRIT)){

            if(type == 2){

                ATK = (STR + (Morale / 20) + weaponBonus() + accessoryBonus(0)) * 3;
            }
            else{

                ATK = ((STR + (Morale / 20) + weaponBonus() + accessoryBonus(0)) - target.getDefense()) * 3; //CRITICAL HIT!


            }
            //target.adjustHP(-ATK);

            if(weapon != null){

                weapon.weaponDamage();

            }

        }
        else if(determineHIT(HIT)){

            if(type == 2){

                ATK = (STR + (Morale / 5) + weaponBonus() + accessoryBonus(0)); //HIT!


            }
            else{

                ATK = (STR + (Morale / 5) + weaponBonus() + accessoryBonus(0)) - target.getDefense(); //HIT!



            }
            //target.adjustHP(-ATK);

            if(weapon != null){

                weapon.weaponDamage();

            }

        }
        else{

            ATK = 0; //Miss...
            
        }


        return ATK;
    }

    public bool determineHIT(int HIT){

       
        if(HIT >= UnityEngine.Random.Range(0, 100)){
            return true;
        }
        else{
            return false;
        }
    }

    public bool determineCRIT(int CRIT){

        
        if(CRIT >= UnityEngine.Random.Range(0,100)){
            return true;
        }
        else{
            return false;
        }
    }

    public int getATK(CharacterStats target, bool noDEF){

        if(noDEF){

            return (getStrength() + (Morale / 20) + weaponBonus() + accessoryBonus(0));
        }
        else{

            return (getStrength() + (Morale / 20) + weaponBonus() + accessoryBonus(0)) - target.getDefense();


        }


    }

    public int getHIT(CharacterStats target, bool noAVO){

        if(noAVO){

            return ((((getDexterity() * 3 + getLuck()) / 2) + (2 * (Morale / 20)))) + accessoryBonus(1);
        }
        else{

            return ((((getDexterity() * 3 + getLuck()) / 2) + (2 * (Morale / 20))) - target.AVO) + accessoryBonus(1);


        }


    }

    public int getCRIT(CharacterStats target, bool noLCK){

        if(noLCK){

            return ((((getDexterity() / 2) - 5) + (Morale / 20))) + accessoryBonus(2);
        }
        else{

            return ((((getDexterity() / 2) - 5) + (Morale / 20)) - target.getLuck()) + accessoryBonus(2);


        }
        
    }

    public int weaponBonus(){

        if(weapon != null && !(weapon.isBroken)){

            return weapon.MGT;

        }
        else{

            return 0;
        }
    }

    

    public int accessoryBonus(int type){

        int totalBonus = 0;


        if(ring != null){

            totalBonus += ring.battleBonus(type);
        }
        
        if(aura != null){

            totalBonus += aura.battleBonus(type);
        }

        return totalBonus;

        
    }


    public void updateAVO(Ring ring, Aura aura){

        int totalMod = 0;
        if(ring != null){

            totalMod += ring.AVOmodifier;

        }

        if(aura != null){

            totalMod += aura.AVOmodifier;
        }

        AVO = ((getSpeed()*3 + getLuck()) / 2) + (2 * (Morale / 20)) + totalMod;

    }

    public int Heal(CharacterStats target){

        int healBonus = 0;
        if(amulet != null){
            healBonus += amulet.HealModifier;
        }

        if(bracelet != null){
            healBonus += bracelet.HealModifier;
        }

        if(aura != null){

            healBonus += aura.HealModifier;
        }

        healBonus += Morale / 10;

        return healBonus;



    }

    public int getMaxHP() {
        return HPMAX.getValue(statModifiers);
    }

    public int getMaxAP() {
        return APMAX.getValue(statModifiers);
    }

    public int getStrength() {
        return STR.getValue(statModifiers);
    }

    public int getDefense() {
        return DEF.getValue(statModifiers);
    }

    public int getSpeed() {
        return SPD.getValue(statModifiers);
    }

    public int getDexterity() {
        return DEX.getValue(statModifiers);
    }

    public int getLuck() {
        return LCK.getValue(statModifiers);
    }

    public int getMovement() {
        return MV.getValue(statModifiers);
    }

    //resist a status effect when an attempt is made. The bool will return to false when the effect is resisted

    public void resistCharm() {

        if(!charmRES){

            charmRES = true;
        }
    }

    public void resistTaunt(){

        if(!tauntRES){

            tauntRES = true;
        }
    }
}
