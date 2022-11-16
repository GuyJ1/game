using System;
using UnityEngine;
using TMPro;

public class CharacterCardUI : MonoBehaviour
{
    // Public vars
    [SerializeField] public TMP_Text statsPanel;
    [SerializeField] public Vector3 openPos;
    [SerializeField] public Vector3 closePos;
    [SerializeField] float panelSpeed;
    public CharacterStats CurrentCharacter;
    public RectTransform trans;

    // Private vars
    private bool travelingToTarget = false;
    private Vector3 targetPos;

    // Start is called before the first frame update
    void Start()
    {
        close();
    }

    // Update is called once per frame
    void Update()
    {
        // Update Stats Panel
        if (CurrentCharacter != null)
        {
            statsPanel.text = statsList(CurrentCharacter);
        }

        // Target following
        if (travelingToTarget)
        {
            // Set the target position to the position of the tile
            Vector3 targetDirection = (targetPos - trans.transform.position).normalized;
            float distToTarget = Vector3.Distance(trans.transform.position, targetPos);
            float moveDist = Vector3.Distance(new Vector3(0,0,0), targetDirection * panelSpeed * Time.deltaTime);

            // Check whether we reached the target
            if (distToTarget > 0.0f && distToTarget <= moveDist)
            {
                trans.transform.position = targetPos;
                travelingToTarget = false;
            }
            // Else, move this object towards it
            else
            {
                trans.transform.position += targetDirection * panelSpeed * Time.deltaTime;
            }
        }
    }

    public void open(CharacterStats stats)
    {
        // Set character
        CurrentCharacter = stats;

        // Move panel
        targetPos = openPos;
        travelingToTarget = true;
    }

    public void open()
    {
        // Move panel
        targetPos = openPos;
        travelingToTarget = true;
    }

    public void close()
    {
        // Move panel
        targetPos = closePos;
        travelingToTarget = true;
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
