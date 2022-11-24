using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconBar : MonoBehaviour
{
    public int value, maxValue, width, height;
    public GameObject emptyIcon, fullIcon;
    //private List<Sprite> sprites;

    // Update is called once per frame
    void LateUpdate()
    {
        
    }

    public void SetMaxValue(int ap)
    {
        GameObject obj = Instantiate(emptyIcon, this.transform);
    }

    public void SetValue(int ap)
    {
        
    }
}
