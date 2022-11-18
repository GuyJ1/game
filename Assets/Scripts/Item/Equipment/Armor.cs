using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : EquippableItem
{
    public int RES; //Resistance. Total defense is increased/decreased with this stat.
    public int CON; //Constitution. Depending on this value compared to unit's STR, total movement allowed will be decreased, from 1 to 5.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public new void applyModifiers(CharacterStats character)
    {
        base.applyModifiers(character);
        //TODO: Armor should implement its custom modifiers here
    }
}
