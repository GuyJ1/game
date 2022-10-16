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
    public int SPD; //Speed
    public int DEX; //Dexterity
    public int LCK; //Luck
    public int MV; //Movement
    public int AP; //Ability Points (possible currency for abilities)
    public int APMAX; //Maximum ability points
    public int Morale; //Morale (from 0 - 100), 
                        //depending on this value, ATK/CRIT are boosted from +1 to +5 and HIT/AVO is boosted by +2 to +10
    public int MoraleMAX; //Maximum Morale
    public int ATK; //Attack power (= STR - Enemy's DEF)
    public int HIT; //Hit  (= (DEX*3 + LCK) / 2)
    public int CRIT; //Critical  (= (DEX / 2) - 5)
    public int AVO; //Avoid  (= (SPD*3 + LCK) / 2)

    public int HitRate; // = HIT - Enemy's AVO
    public int CritRate; // = CRIT - Enemy's LCK

    public string Name;

    // Actions
    public GameObject basicAttack, comboAttack; //Generic actions
    public List<GameObject> abilities; //Unique abilities

    // Healthbar
    public HealthBar healthBar;
    public float healthBarYOffset;

    // Canvas reference
    public GameObject canvas;

    // Character's logical position on the grid
    public Vector2Int gridPosition;

    // Crew that this character belongs to (should be set by CrewSystem)
    public GameObject crew;

    

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

        // Set health, ability points, and morale
        HP = HPMAX;
        healthBar.SetMaxHealth(HPMAX);

        AP = APMAX;

        Morale = MoraleMAX;
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
    }

    // Update healthbar position
    void LateUpdate()
    {
        // Offset
        Vector3 posOffset = new Vector3(0, healthBarYOffset, 0);

        // Update the healthbar position
        healthBar.transform.position = Camera.main.WorldToScreenPoint(transform.position + posOffset);
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
        if(Morale < 0){
            Morale = 0;
        }
        if(Morale > MoraleMAX){
            Morale = MoraleMAX;
        }
    }

    public int Attack(EnemyStats target){

        ATK = (STR + (Morale / 5)) - target.DEF;
        target.adjustHP(-ATK);

        return ATK;
    }

    




}
