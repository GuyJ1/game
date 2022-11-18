using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : EquippableItem
{
    [SerializeField]
    public int ATKmodifier;
    public int HITmodifier;
    public int CRITmodifier;
    public int AVOmodifier;

    // Start is called before the first frame update
    void Start()
    {

    }


    //return either ATK (0), HIT (1), or CRIT (2)
    public int battleBonus(int type){

        switch(type){
            case 0:
                return ATKmodifier;
            case 1:
                return HITmodifier;
            case 2:
                return CRITmodifier;
            default:
                return 0; //error
        }

    }
}
