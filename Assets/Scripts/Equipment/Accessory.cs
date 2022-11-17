using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accessory : MonoBehaviour
{
    [SerializeField]
    public string Name;
    public string Description;
    public int Type; //different types of accessories give different bonuses to the unit
    //1: Hat, grants +STR/DEF/SPD/DEX
    //2: Ring, grants +ATK/HIT/CRIT/AVO
    //3: Amulet, grants +HP / +LCK
    //4: Bracelet, grants +AP / +LCK
    //5: Shoes, grants +MV / +SPD
    //6: Aura, can grant any of the above stats


    // Start is called before the first frame update
    void Start()
    {


        
    }

    //multiple child classes are created for each of the above types, each with their own function that grants the bonuses to the unit.
}
