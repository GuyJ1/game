using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;

public class Dialogue_Manager_Final : MonoBehaviour
{
    public TextAsset inkFile;
    public GameObject textBox;
    public GameObject customButton;
    public GameObject optionPanel;

    static Story story;
    Text nametag;
    Text message;
    static Choice choiceSelected;

    // Start is called before the first frame update
    void Start()
    {
        story = new Story(inkFile.text);
        nametag = textBox.transform.GetChild(0).GetComponent<Text>();
        message = textBox.transform.GetChild(1).GetComponent<Text>();
        choiceSelected = null;
    }

    /*private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Is there more to the story?
            if(story.canContinue)
            {
                nametag.text = "Main Character";
                AdvanceDialogue();

                //Are there any choices?
                if (story.currentChoices.Count != 0)
                {
                    StartCoroutine(ShowChoices());
                }
            }
            else
            {
                FinishDialogue();
            }
        }
    }
    */

    // Finished the Story (Dialogue)
    private void FinishDialogue()
    {
        Debug.Log("End of Dialogue!");
    }
}
