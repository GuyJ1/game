using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarteringTrigger : MonoBehaviour
{
    [Header("Ink JSON")]
    [SerializeField] private TextAsset inkJSON;

    private void Update()
    {
        if (InputManager.GetInstance().GetSubmitPressed())
        {
            Debug.Log(inkJSON.text);
        }
    }
}