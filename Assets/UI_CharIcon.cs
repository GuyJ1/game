using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CharIcon : MonoBehaviour
{
    public GameObject myChar;
    public CharacterStats charScript = null;
    public HealthBar myHealthBar;
    public bool UpdateChar = false;

    // Late update is called after update
    void LateUpdate()
    {
        // On character update for this icon
        if (UpdateChar)
        {
            charScript = myChar.GetComponent<CharacterStats>();
            transform.GetChild(0).GetChild(0).GetComponent<HealthBar>().gradient = CharacterStats.HealthBarGradient(BattleEngine.isAllyUnit(myChar));
            UpdateChar = false;
        }

        // Update healthbar
        if (charScript != null)
        {
            myHealthBar.SetMaxHealth(charScript.getMaxHP());
            myHealthBar.SetHealth(charScript.HP);
        }
    }
}
