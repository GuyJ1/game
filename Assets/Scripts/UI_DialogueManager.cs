// this script should manage the current dialogue interaction
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;    //text mesh pro
using Ink.Runtime;  // ink file

public class UI_DialogueManager : MonoBehaviour
{
    // ------------------------ variables: ------------------------
    [Header("Dialogue UI")] 
    [SerializeField] private GameObject dialoguePanel;  // dialogue panel object
    [SerializeField] private TextMeshProUGUI dialogueText;  // dialogue text object
    private Story currentStory; // keeps track of current ink file to display
    private bool dialogueIsPlaying;
    private static UI_DialogueManager instance; // static instance of the dialogue manager


    // ------------------------ functions: ------------------------

    // sets the current instance
    private void Awake()
    {
        instance = this;

        if(instance != null) {
            Debug.LogWarning("there's more than one dialogue manager");
        }
        instance = this;
    }


    public static UI_DialogueManager GetInstance()
    {
        return instance;
    }


    // initalizing everything
    private void Start() 
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
    }


    private void Update() 
    {
        // return right away if the dialogue isnt playing
        if(!dialogueIsPlaying) return;

        // continue to next line in dialogue when the button is pressed
        if(InputManager.GetInstance().GetSubmitPressed())
        {
            ContinueStory();
        }
    }

    
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        ContinueStory();
    }


    private void ContinueStory()
    {
        // gets the next line of dialogue text from the json file
        if(currentStory.canContinue) {
            dialogueText.text = currentStory.Continue();    
        }

        // exits dialogue if no more text exists
        else ExitDialogueMode();
    }


    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = " ";
    }
    
}
// ------------------------ end of file ------------------------