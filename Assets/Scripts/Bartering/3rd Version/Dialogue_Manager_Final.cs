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
}
