using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class NodeInfo : MonoBehaviour
{
    
    public int goldReward;
    public int BoozeReward;
    public int MoraleReward;
    public int Enemies;
    public string description;
    public static TextMeshProUGUI textBox;
    public static GameObject textnode;
    public GameObject node;
    
    private struct Information{
        int Gold;
        int Booze;
        int Morale;
        int Enemies;
        string NodeDescription;
    };

    public void Awake()
    {
        textnode = GameObject.Find("Text (TMP)");
        
        node = this.gameObject;
        textBox=textnode.GetComponent<TextMeshProUGUI>();
        
    }
    public void showTextBox()
    {
        textBox.color = Color.white;
        textBox.text = "Gold Reward: "+ goldReward + "\n";
        textBox.text += "Booze Reward: " + BoozeReward + "\n";
        textBox.text += "Morale Reward: " + MoraleReward + "\n";
        textBox.text += "Enemies: " + Enemies + "\n";
        textBox.text += description;
        if(!NodeLines.isNeighbor(PlayerMove.getCurrNode(),PlayerMove.getDestination()))
        {
            //string no = "\nCannot Travel to \nthis Destination yet.";
            
            textBox.text +=  "<color=red>\nCannot Travel to \nthis Destination yet, \n<i> Matey</i>.</color>" ;
           // textBox.color = Color.red;
        }
        textnode.SetActive(true);
    }

    public static void hideTextBox()
    {
        textBox.text = "";
        textnode.SetActive(false);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (NodeClick.clickobj == this.gameObject)
        {
           this.showTextBox();
        }

    }
}
