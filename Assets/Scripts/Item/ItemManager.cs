using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 전체적인 아이템 관리 및 전달 스크립트
public class ItemManager : MonoBehaviour
{
    public ItemSO itemSO;

    [SerializeField] private List<Item> commonList;
    [SerializeField] private List<Item> rareList;
    [SerializeField] private List<Item> legendaryList;

    // 아이템 등급에 따른 분류
    private void Start()
    {
        commonList = new List<Item>();
        rareList = new List<Item>();
        legendaryList = new List<Item>();

        foreach (Item item in itemSO.itemList)
        {
            switch (item.itemType)
            {
                case ItemType.COMMON:
                    commonList.Add(item);
                    break;
                case ItemType.RARE:
                    rareList.Add(item);
                    break;
                case ItemType.LEGENDARY:
                    legendaryList.Add(item);
                    break;
            }
        }
    }

    // 아이템 드랍 확률결과로 나온 아이템 타입에 따른 아이템 뽑기
    public Item GetRandomItemWithProbability(ItemType itemType)
    {
        Item resultItem = null;
        int randomIndex;
        switch (itemType)
        {
            case ItemType.COMMON:
                randomIndex = Random.Range(0, commonList.Count);
                resultItem = commonList[randomIndex];
                break;
            case ItemType.RARE:
                randomIndex = Random.Range(0, rareList.Count);
                resultItem = rareList[randomIndex];
                break;
            case ItemType.LEGENDARY:
                randomIndex = Random.Range(0, legendaryList.Count);
                resultItem = legendaryList[randomIndex];
                break;
        }

        return resultItem;
    }
}
