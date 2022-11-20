using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CrewTurnOrder : MonoBehaviour
{
    // Public vars
    public BattleEngine battleScript; // Battle engine ref
    public GameObject icon; // Icon game object to attach to canvas
    public List<GameObject> icons = new List<GameObject>(); // Char icons list
    public float leftMargin; // Spacing away from center of the canvas
    public float iconSpacing; // Spacing for individual icons
    public float iconHeight; // Distance away from the bottom of the canvas
    
    // Private vars
    private int numOfCharacters = 0;

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

                // Check both lists to see whether we have this icon already
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

                    // Add icon to list
                    icons.Add(newIcon);

                    // Modify rect transform
                    RectTransform rect = icon.transform.GetComponent<RectTransform>();
                    rect.anchorMin = new Vector2(0.0f, 0.0f);
                    rect.anchorMax = new Vector2(0.0f, 0.0f);
                    rect.anchoredPosition = new Vector2(shiftAmount(numOfCharacters), iconHeight);

                    // Modify healthbar
                    icon.transform.GetChild(0).GetChild(0).GetComponent<HealthBar>().gradient = CharacterStats.HealthBarGradient(BattleEngine.isAllyUnit(unit));

                    // Increment counter
                    numOfCharacters++;        
                }
            }

            // Update number of characters
            numOfCharacters = currNumOfChars;
        }
    }

    private float shiftAmount(int n)
    {
        // Calculate shift
        float shift = leftMargin + (n * iconSpacing);
        float canvasWidth = transform.parent.transform.GetComponent<RectTransform>().rect.width - 50f;

        // If the calculated shift position is off the screen, then we'll have to adjust
        // the position of every character icon on that side of the screen
        if (shift > canvasWidth)
        {
            float iconSpaceNew = (canvasWidth - leftMargin) / n;
            float newShift = shift;

            for (int i = 0; i < icons.Count; i++)
            {
                GameObject currIcon = icons[i];
                newShift = leftMargin + (i * iconSpaceNew);
                currIcon.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(-newShift, iconHeight);
            }

            shift = newShift; // for return
        }

        // Return calculated shift
        return shift;
    }
}
