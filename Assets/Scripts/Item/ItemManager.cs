using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 전체적인 아이템 관리 및 전달 스크립트
public class ItemManager : MonoBehaviour
{
    public ItemSO itemSO;

    public List<Item> warriorCommonList;
    public List<Item> warriorRareList;
    public List<Item> warriorLegendaryList;

    public List<Item> archerCommonList;
    public List<Item> archerRareList;
    public List<Item> archerLegendaryList;

    private Inventory inventory;

    // 아이템 등급에 따른 분류
    private void Awake()
    {
        warriorCommonList = new List<Item>();
        warriorRareList = new List<Item>();
        warriorLegendaryList = new List<Item>();

        archerCommonList = new List<Item>();
        archerRareList = new List<Item>();
        archerLegendaryList = new List<Item>();

        foreach (Item item in itemSO.itemList)
        {
            if (item.charType == CharacterType.WARRIOR)
            {
                switch (item.itemType)
                {
                    case ItemType.COMMON:
                        warriorCommonList.Add(item);
                        break;
                    case ItemType.RARE:
                        warriorRareList.Add(item);
                        break;
                    case ItemType.LEGENDARY:
                        warriorLegendaryList.Add(item);
                        break;
                }
            }

            else if (item.charType == CharacterType.ARCHER)
            {
                switch (item.itemType)
                {
                    case ItemType.COMMON:
                        archerCommonList.Add(item);
                        break;
                    case ItemType.RARE:
                        archerRareList.Add(item);
                        break;
                    case ItemType.LEGENDARY:
                        archerLegendaryList.Add(item);
                        break;
                }
            }
        }

        inventory = GameObject.FindGameObjectWithTag("Canvas").transform.Find("Inventory").GetComponent<Inventory>();
        inventory.SetItemManager(this);
    }

    // 아이템 드랍 확률결과로 나온 아이템 타입과 직업에 연관된 아이템 뽑기
    public Item GetRandomItemWithProbability(ItemType itemType, string charType)
    {
        Item resultItem = null;
        int randomIndex;

        if (charType.Equals("Warrior"))
        {
            switch (itemType)
            {
                case ItemType.COMMON:
                    randomIndex = Random.Range(0, warriorCommonList.Count);
                    resultItem = warriorCommonList[randomIndex];
                    break;
                case ItemType.RARE:
                    randomIndex = Random.Range(0, warriorRareList.Count);
                    resultItem = warriorRareList[randomIndex];
                    break;
                case ItemType.LEGENDARY:
                    randomIndex = Random.Range(0, warriorLegendaryList.Count);
                    resultItem = warriorLegendaryList[randomIndex];
                    break;
            }
        }

        else if (charType.Equals("Archer"))
        {
            switch (itemType)
            {
                case ItemType.COMMON:
                    randomIndex = Random.Range(0, archerCommonList.Count);
                    resultItem = archerCommonList[randomIndex];
                    break;
                case ItemType.RARE:
                    randomIndex = Random.Range(0, archerRareList.Count);
                    resultItem = archerRareList[randomIndex];
                    break;
                case ItemType.LEGENDARY:
                    randomIndex = Random.Range(0, archerLegendaryList.Count);
                    resultItem = archerLegendaryList[randomIndex];
                    break;
            }
        }

        return resultItem;
    }
}
