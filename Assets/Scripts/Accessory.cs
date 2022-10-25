using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Accessory : MonoBehaviour
{

    [SerializeField]
    public string Name;
    public string Description;
    public int Type; //different types of accessories give different bonuses to the unit
    //1: Hat, grants +STR/DEF/ATK/HIT
    //2: Ring, grants +SPD/DEX/CRIT/AVO
    //3: Necklace, grants +HP/HP Regen
    //4: Bracelet, grants +AP/AP Regen
    //5: Shoes, grants +MV
    //6: Aura, grants +Morale, can also grant any of the above stats
    public int bonusCount; //depending on the type, 1 or multiple bonuses can be granted


    // Start is called before the first frame update
    void Start()
    {


        
    }

    //multiple child classes should be created for each of the above types, each with their own function that grants the bonueses to the unit.
}
