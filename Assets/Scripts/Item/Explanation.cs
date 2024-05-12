using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Explanation : MonoBehaviour
{
    public Image itemImage;
    public Text itemName;
    public Text itemAttackDamage;
    public Text itemAttackSpeed;
    public Text itemBonusStat;
    public Text itemBonusStatValue;
    public Text itemType;   // 아이템 등급
    public Text itemCharType;   // 직업 전용 아이템

    public void InfoSetting(Sprite sprite, string name, int attackDamage, int attackSpeed, BonusStat bonusStat, float value, ItemType type, CharacterType charType)
    {
        // Sprite
        itemImage.color = new Color(1, 1, 1, 1);
        itemImage.sprite = sprite;

        // Name
        itemName.text = "Name: " + name;

        // AttackDamage
        itemAttackDamage.text = "AttackDamage +" + attackDamage.ToString();

        // AttackSpeed
        itemAttackSpeed.text = "AttackSpeed: " + attackSpeed.ToString();

        // BonusStat
        if (bonusStat == BonusStat.HP)
        {
            itemBonusStat.text = "Bonus Stat: Hp +" + value.ToString();
        }
        else if (bonusStat == BonusStat.MOVESPEED)
        {
            itemBonusStat.text = "Bonus Stat: Speed +" + value.ToString();
        }
        else if (bonusStat == BonusStat.EVASIONRATE)
        {
            itemBonusStat.text = "Bonus Stat: EvasionRate +" + value.ToString();
        }
        else
        {
            itemBonusStat.text = "Bonus Stat: None";
        }

        // Type
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

        // Character Type
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
