using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Explanation : MonoBehaviour
{
    public Image itemImage;
    public Text itemName;
    public Text itemAttackDamage;
    public Text itemType;   // 아이템 등급
    public Text itemCharType;   // 직업 전용 아이템

    public void InfoSetting(Sprite sprite, string name, int attackDamage, ItemType type, CharacterType charType)
    {
        itemImage.color = new Color(1, 1, 1, 1);
        itemImage.sprite = sprite;
        itemName.text = "Name: " + name;
        itemAttackDamage.text = "AttackDamage: " + attackDamage.ToString();

        if (type == ItemType.COMMON)
        {
            itemType.color = Color.white;
            itemType.text = "Common";
        }
        else if (type == ItemType.RARE)
        {
            itemType.color = Color.blue;
            itemType.text = "Rare";
        }
        else if (type == ItemType.LEGENDARY)
        {
            itemType.color = Color.yellow;
            itemType.text = "Legendary";
        }

        if (charType == CharacterType.WARRIOR)
        {
            itemCharType.text = "- Warrior -";
        }
        else
        {
            itemCharType.text = "- Archer -";
        }
    }
}
