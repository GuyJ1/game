using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public bool friendly; //Whether this ability targets allies or enemies
    public int totalDMG;
    public int totalHP;
    public int cost;
    public int costType; //Depending on this value, the a different resource will be used fot this ability
                         //AP = 0, HP = 1, Morale = 2, Gold = 3, Food = 4, etc.
    public int range;

    public Ability(bool friendly, int totalDMG, int totalHP, int cost, int costType, int range) {
        this.friendly = friendly;
        this.totalDMG = totalDMG;
        this.totalHP = totalHP;
        this.cost = cost;
        this.cost = costType;
        this.range = range;
    }

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
