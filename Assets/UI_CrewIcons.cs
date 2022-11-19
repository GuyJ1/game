using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CrewIcons : MonoBehaviour
{
    // Public vars
    public BattleEngine battleScript; // Battle engine ref
    public GameObject icon; // Icon game object to attach to canvas
    public List<GameObject> icons = new List<GameObject>(); // Char icons list
    public float centerSpacing; // Spacing away from center of the canvas
    public float iconSpacing; // Spacing for individual icons
    public float iconHeight; // Spacing for individual icons
    
    // Private vars
    private int numOfCharacters = 0;
    private int goodChars = 0;
    private int badChars = 0;

    // Late update is called after Update()
    void LateUpdate()
    {
        // Get char count from battle engine
        int currNumOfChars = battleScript.units.Count;

        // Update when the # of chars in the battle system change
        if (numOfCharacters != currNumOfChars)
        {
            // For every character
            foreach (GameObject unit in battleScript.units)
            {
                bool isNewIcon = true;

                // See whether we have this icon already
                foreach (GameObject icon in icons)
                {
                    if (unit == icon.GetComponent<UI_CharIcon>().myChar)
                    {
                        isNewIcon = false;
                    }
                }

                // If this is a new icon-
                if (isNewIcon == true)
                {
                    // Spawn Icon
                    GameObject newIcon = Instantiate(icon, this.transform);
                    //Vector3 pos = newIcon.transform.position; // store pos

                    // Set Icon Data
                    newIcon.GetComponent<UI_CharIcon>().myChar = unit;

                    // Position based on whether ally or enemy
                    if (BattleEngine.isAllyUnit(unit) == true)
                    {
                        newIcon.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-centerSpacing - (goodChars * iconSpacing), iconHeight);
                        newIcon.transform.GetChild(0).GetChild(0).GetComponent<HealthBar>().gradient = CharacterStats.HealthBarGradient(true);
                        goodChars++;
                    }
                    else
                    {
                        newIcon.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(centerSpacing + (badChars * iconSpacing), iconHeight);
                        newIcon.transform.GetChild(0).GetChild(0).GetComponent<HealthBar>().gradient = CharacterStats.HealthBarGradient(false);
                        badChars++;
                    }          

                    // Add new Icon to List
                    icons.Add(newIcon);
                }
            }

            // Update number of characters
            numOfCharacters = currNumOfChars;
        }
    }
}
