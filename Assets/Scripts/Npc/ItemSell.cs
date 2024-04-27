using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSell : MonoBehaviour
{
    //itemKey : buyValue(-1 : can not buy), sellValue(-1 : can not sell)
    private static Dictionary<int, Dictionary<int, int>> _items = new()
    {
        { 1001, new Dictionary<int, int> { { 100, 50 } } },
        { 1002, new Dictionary<int, int> { { 100, 50 } } },
        { 1003, new Dictionary<int, int> { { 100, -1 } } },
        { 1004, new Dictionary<int, int> { { 200, 100 } } },
        { 1005, new Dictionary<int, int> { { 200, 100 } } },
        { 2001, new Dictionary<int, int> { { -1, 50 } } }
    };

    public static int BuyItem(int itemKey)
    {
        if (_items.ContainsKey(itemKey) && _items[itemKey][0] != -1)
        {
            return _items[itemKey][0];
        }
        return -1;
    }
    
    public static int SellItem(int itemKey)
    {
        if (_items.ContainsKey(itemKey) && _items[itemKey][1] != -1)
        {
            return _items[itemKey][1];
        }
        return -1;
    }

    public static Dictionary<int, Dictionary<int, int>> GetItemList()
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
