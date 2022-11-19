using System;
using UnityEngine;
using TMPro;

public class CharacterCardUI : MonoBehaviour
{
    // Public vars
    [SerializeField] public TMP_Text statsPanel;
    [SerializeField] public Animator panelAnim;
    public CharacterStats CurrentCharacter;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Update Stats Panel
        if (CurrentCharacter != null)
        {
            statsPanel.text = statsList(CurrentCharacter);
        }
    }

    public void open(CharacterStats stats)
    {
        // Set character
        CurrentCharacter = stats;

        // Open panel
        panelAnim.Play("PanelOpen");
    }

    public void open()
    {
        // Open panel
        panelAnim.Play("PanelOpen");
    }

    public void close()
    {
        // Close panel
        panelAnim.Play("PanelClose");
    }

    public string statsList(CharacterStats stats)
    {
        string text = "";

        text += "Max HP: " + stats.getMaxHP() + "\n";
        text += "ATK: " + stats.ATK + "\n";
        text += "STR: " + stats.getStrength() + "\n";
        text += "DEF: " + stats.getDefense() + "\n";
        text += "SPD: " + stats.getSpeed() + "\n";
        text += "DEX: " + stats.getDexterity() + "\n";
        text += "LCK: " + stats.getLuck() + "\n";
        text += "AVO: " + stats.AVO + "\n";
        text += "HIT: " + stats.HIT + "\n";
        text += "MV: " + stats.getMovement();

        return text;
    }
}
