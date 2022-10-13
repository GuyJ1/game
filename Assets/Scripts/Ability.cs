using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public bool friendly; //Whether this ability targets allies or enemies
    public int totalDMG;
    public int totalHP;
    public int cost;
    public int range;

    public Ability(bool friendly, int totalDMG, int totalHP, int cost, int range) {
        this.friendly = friendly;
        this.totalDMG = totalDMG;
        this.totalHP = totalHP;
        this.cost = cost;
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
