using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CharIcon : MonoBehaviour
{
    public GameObject myChar;
    public CharacterStats charScript = null;
    public HealthBar myHealthBar;

    // Late update is called after update
    void LateUpdate()
    {
        // Get script if null
        if (charScript == null && myChar != null)
        {
            charScript = myChar.GetComponent<CharacterStats>();
        }

        // Update healthbar value
        if (charScript != null)
        {
            myHealthBar.SetMaxHealth(charScript.getMaxHP());
            myHealthBar.SetHealth(charScript.HP);
        }
    }
}
