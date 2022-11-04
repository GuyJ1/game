using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeTypeWeapon : Weapon
{

    [SerializeField]
    int type; //1 = Sword, 2 = Scimitar


    // Start is called before the first frame update
    void Start()
    {
        if(type == 2){//scimitar users lose strength for mobility

            MGT -= 5;
        }

        Range = 1;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //unique ability for scimitar, attack twice if user's SPD  - target's SPD  >= 5




}
