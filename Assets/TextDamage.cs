using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextDamage : MonoBehaviour
{
    // Public vars
    [SerializeField] public TMP_Text damageText;
    [SerializeField] public int damageToDisplay;
    [SerializeField] public Color32 damageColor;


    // Start is called before the first frame update
    void Start()
    {
        damageText.text = damageToDisplay.ToString();
        damageText.color = damageColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
