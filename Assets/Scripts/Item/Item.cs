using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item", menuName = "ScriptableObject/Item")]
public class Item : ScriptableObject
{
    public int itemID;  // 아이템 ID
    public string itemName;
    public Sprite itemImage;
}
