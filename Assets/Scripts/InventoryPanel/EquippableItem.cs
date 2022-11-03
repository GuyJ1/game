using UnityEngine;

public enum EquipmentType //defining equipment types
{
  Weapon,
  Helmet,
  Armor,
  Gloves,
  Accessory1,
  Accessory2,
  Accessory3,
  Accessory4,
}


  [CreateAssetMenu]//allows EquippableItem to be manually created in unity project editor
public class EquippableItem : Item //extends Item class - has scriptableObject
{
  //used for equipment parameter bonus that will be applied to character stats when equipped
  //direct flat bonus to stats
  public int HpBonus;
  public int StrBonus;
  public int DefBonus;
  public int SpdBonus;
  public int DexBonus;
  public int LckBonus;
  public int MvBonus;
  public int ApBonus;
  public int MoraleBonus;
  public int DurBonus;
  [Space] //for visual purpose in unity editor, allows spacing in between
  //percent modifier bonus to stats - will update later
  public int HpMod;
  public int StrMod;
  public int DefMod;
  public int SpdMod;
  public int DexMod;
  public int LckMod;
  public int MvMod;
  public int ApMod;
  public int MoraleMod;
  public int DurMod;
  [Space]
  public EquipmentType EquipmentType;
}
