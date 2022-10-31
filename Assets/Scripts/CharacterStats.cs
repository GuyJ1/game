using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{

    [SerializeField]
    public int HP; //Current health
    public int HPMAX; //Maximum health
    public int STR; //Strength
    public int DEF; //Defense
    public int SPD; //Agility
    public int DEX; //Dexterity
    public int LCK; //Luck
    public int MV; //Movement
    public int AP; //Ability Points (possible currency for abilities)
    public int APMAX; //Maximum ability points
    public int Morale; //Morale (from 0 - 100), 
                        //depending on this value, ATK/CRIT are boosted from +1 to +5 and HIT/AVO is boosted by +2 to +10
    public int MoraleMAX; //Maximum Morale
    public int MoraleMIN; //Minimum Morale (used for Shoes)
    public int ATK; //Attack power (= STR - Enemy's DEF)
    public int HIT; //Hit Rate (= (((DEX*3 + LCK) / 2) - Enemy's AVO)
    public int CRIT; //Critical Rate (= ((DEX / 2) - 5) - Enemy's LCK)
    public int AVO; //Avoid  (= (SPD*3 + LCK) / 2)

    public string Name;

    // Actions
    public GameObject basicAttack, comboAttack; //Generic actions
    public List<GameObject> abilities; //Unique abilities

    // Healthbar
    [SerializeField] public float healthBarScale;
    public HealthBar healthBar;
    public float healthBarYOffset;

    // Canvas reference
    public GameObject canvas;

    // Character's logical position on the grid
    public Vector2Int gridPosition;

    // Crew that this character belongs to (should be set by CrewSystem)
    public GameObject crew;

    //Equipment
    public Weapon weapon; //increases ATK
    public Armor armor; //increases DEF
    public Hat hat; //can increase STR, DEF, SPD, and/or DEX
    public Ring ring; //can increase ATK, HIT, CRIT, and/or AVO
    public Amulet amulet; //can increase max HP and/or LCK
    public Bracelet bracelet; //can increase max AP and/or LCK
    public Shoes shoes; //can increase MV and/or Morale
    public Aura aura; //can increase any stat


    

    // Start is called before the first frame update
    void Start()
    {
        // Attach healthbar to canvas
        canvas = GameObject.Find("UI Menu");
        healthBar = Instantiate(healthBar, canvas.transform);

        // Set gradient color
        bool isPlayer = crew.GetComponent<CrewSystem>().isPlayer;
        Gradient grad = new Gradient();

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        GradientColorKey[] colorKey = new GradientColorKey[2];
        colorKey[0].color = isPlayer ? Color.yellow : new Color(255f/255f, 165f/255f, 0f, 1f);
        colorKey[0].time = 0.0f;
        colorKey[1].color = isPlayer ? Color.green : Color.red;
        colorKey[1].time = 1.0f;

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[1];
        alphaKey[0].alpha = 0.5f;

        grad.SetKeys(colorKey, alphaKey);

        healthBar.gradient = grad;

        MoraleMIN = 0;

        //Temporary name for player-controlled characters for logging: Will need to have functionality added later
        Name = "Pupperton";

        // Set health, ability points, morale, etc.

        if(hat != null){

            hat.statBonus(this);
        }

        if(amulet != null){
            amulet.statBonus(this);
        }

        if(bracelet != null){

            bracelet.statBonus(this);
        }

        if(shoes != null){

            shoes.statBonus(this);

        }

        if(aura != null){

            aura.statBonus(this);
        }




        if(MoraleMIN > 80){//minimum morale can't go higher than 80%

            MoraleMIN = 80;

        }

        if(armor != null){
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
            
        }

        HP = HPMAX;

        healthBar.SetMaxHealth(HPMAX);

        Morale = MoraleMAX;

        AP = APMAX;

        updateAVO(ring, aura);




    }

    // Update is called once per frame
    void Update()
    {
        // Health Testing
        if (Input.GetKeyDown(KeyCode.Space))
        {

            //damage
            adjustHP(-20);
        }

        updateAVO(ring, aura);

    }

    // Update healthbar position
    void LateUpdate()
    {
        // Offset
        Vector3 posOffset = new Vector3(0, healthBarYOffset, 0);

        // --------- Update healthbar position ---------
        healthBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + posOffset);

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
        if(AP > APMAX){
            AP = APMAX;
        }

        return 0;
    }

    //Morale changed (either positive or negative)
    public void adjustMorale(int change){
        Morale += change;
        if(Morale < MoraleMIN){
            Morale = MoraleMIN;
        }
        if(Morale > MoraleMAX){
            Morale = MoraleMAX;
        }
    }

    //Attack the enemy, possibly with a critical hit
    //Note: Critical hits triple the total damage
    public int Attack(CharacterStats target){

        HIT = ((((DEX * 3 + LCK) / 2) + (2 * (Morale / 5))) - target.AVO) + accessoryBonus(1);
        CRIT = ((((DEX / 2) - 5) + (Morale / 5)) - target.LCK) + accessoryBonus(2);

        if(determineCRIT(CRIT)){

            ATK = ((STR + (Morale / 5) + weaponBonus() + accessoryBonus(0)) - target.DEF) * 3; //CRITICAL HIT!
            target.adjustHP(-ATK);

            if(weapon != null){

                weapon.weaponDamage();

            }

        }
        else if(determineHIT(HIT)){
            ATK = (STR + (Morale / 5) + weaponBonus() + accessoryBonus(0)) - target.DEF; //HIT!
            target.adjustHP(-ATK);

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

        AVO = ((SPD*3 + LCK) / 2) + (2 * (Morale / 5)) + totalMod;

    }

    

    




}
