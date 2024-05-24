using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 무기 등급
public enum ItemType
{ 
    COMMON, RARE, LEGENDARY
}

public enum CharacterType
{ 
    WARRIOR, ARCHER
}

public enum BonusStat
{
    NONE, HP, MOVESPEED, EVASIONRATE
}

// attackSpeed : 1 ~ 10 == Animation Speed : 0.5 ~ 1.5
//

[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Item")]
public class Item : ScriptableObject
{
    public int itemID;  // 아이템 ID
    public string itemName;
    public int attackDamage;
    public int attackSpeed; 
    public Sprite itemImage;
    public ItemType itemType;
    public CharacterType charType;
    public BonusStat bonusStat;
    public float addValue;
    public GameObject prefab;
}
