
using UnityEngine;


  [CreateAssetMenu] //allows creationg of item assets directly unity by right clicking project
public class Item : ScriptableObject
{
  public string ItemName; //item name
  public Sprite Icon; //item image
  public string Type; //item type
  public string Stats; //item parameters

}
