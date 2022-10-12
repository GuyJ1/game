using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    //visual cue object
    [Header("visual cue")] 
    [SerializeField] private GameObject visualCue;

    [Header("ink json")]
    [SerializeField] private TextAsset inkJSON;  

    private bool playerInRange;

    private void Awake()
    {
        playerInRange = false;
        visualCue.SetActive(false);     // initalize visual cue to inactive
    }

    // enables/disables the visual cue based on whether the player is in range or not
    private void Update()
    {
        if(playerInRange) {
            visualCue.SetActive(true);

            // triggers the dialogue if interact button is pressed
            // if(InputManager.GetInstance().GetInteractPressed()) {
            //     Debug.Log(inkJSON.text);
            // }

        } 
            

        else {
            visualCue.SetActive(false);
        } 
        

    }
    // checks if the player is in range within the visual cue
    private void OnTriggerEnter2D(Collider2D collier)
    {
        if(GetComponent<Collider>().gameObject.tag == "Player") {
            playerInRange = true;
        }
    }

    // checks if the player exits the range within the visual cue
    private void OnTriggerExit2D(Collider2D collier)
    {
        if(GetComponent<Collider>().gameObject.tag == "Player") {
            playerInRange = false;
        }
    }
}
