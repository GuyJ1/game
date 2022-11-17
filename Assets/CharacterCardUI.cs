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

        text += "Max HP: " + stats.HPMAX + "\n";
        text += "ATK: " + stats.ATK + "\n";
        text += "DEF: " + stats.DEF + "\n";
        text += "SPD: " + stats.SPD + "\n";
        text += "DEX: " + stats.DEX + "\n";
        text += "LCK: " + stats.CRIT + "\n";
        text += "AVO: " + stats.AVO + "\n";
        text += "HIT: " + stats.HIT + "\n";
        text += "MV: " + stats.MV;

        return text;
    }
}
