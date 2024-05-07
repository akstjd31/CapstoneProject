using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//내부 동작에 필요한 스크립트로 버튼 등에 적용하지 말 것 (NpcShop.cs 이용)
public class ItemSell : MonoBehaviour
{
    //itemKey : buyValue(-1 : can not buy), sellValue(-1 : can not sell)
    private static Dictionary<int, List<int>> _items = new()
    {
        { 1001, new List<int> { 100, 50 } },
        { 1002, new List<int> { 100, 50 } },
        { 1003, new List<int> { 100, -1 } },
        { 1004, new List<int> { 200, 100 } },
        { 1005, new List<int> { 200, 100 } },
        { 2001, new List<int> { -1, 50 } }
    };

    public Dictionary<string, int> GetShopItemList(int shopKind = 1001)
    {
        Dictionary<string, int> rtValue = new();

        switch (shopKind)
        {
            case 1001:
                rtValue = new()
                {
                    { "A", 100 },
                    { "B", 100 },
                    { "C", 200 },
                    { "D", 200 },
                    { "E", 150 },
                    { "F", 150 },
                    { "G", 100 },
                    { "H", 100 },
                    { "I", 100 },
                };
                break;
        }


        return rtValue;
    }

    public static int BuyItemCost(int itemKey)
    {
        if (_items.ContainsKey(itemKey) && _items[itemKey][0] != -1)
        {
            return _items[itemKey][0];
        }
        return -1;
    }
    
    public static int SellItemCost(int itemKey)
    {
        if (_items.ContainsKey(itemKey) && _items[itemKey][1] != -1)
        {
            return _items[itemKey][1];
        }
        return -1;
    }

    public static Dictionary<int, List<int>> GetItemList()
    {
        return _items;
    }

    public static Dictionary<int,int> GetItemList(int type)
    {
        Dictionary<int, int> item = new();
        var keys = _items.Keys;

        //for buying process
        if(type == 1)
        {
            foreach (var key in keys)
            {
                if (_items[key][0] != -1)
                {
                    item.Add(key, _items[key][0]);
                }
            }

            return item;
        }
        //for selling process
        else if(type == 2)
        {
            foreach (var key in keys)
            {
                if (_items[key][1] != -1)
                {
                    item.Add(key, _items[key][1]);
                }
            }

            return item;
        }

        throw new KeyNotFoundException($"type : {type} should be 1(buy cost) or 2(sell cost)");
    }
}
