using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public GameObject character;
    // Start is called before the first frame update
    void Start()
    {
        var gameobj = Instantiate(character, this.transform);
        gameobj.GetComponent<Animator>().Rebind();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
