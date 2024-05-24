using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemSO", menuName = "ScriptableObject/ItemSO")]
public class ItemSO : ScriptableObject
{
    public List<Item> itemList;
}
