using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//reference only class - no direct use (NpcShop.cs etc.)
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
        { 1006, new List<int> { 100, 50 } },
        { 1007, new List<int> { 100, 50 } },
        { 1008, new List<int> { 100, -1 } },
        { 1009, new List<int> { 200, 100 } },
        { 1010, new List<int> { 200, 100 } },

        { 2001, new List<int> { -1, 50 } }
    };
    
    private static Dictionary<int, string> _itemName = new()
    {
        { 1001, "A" },
        { 1002, "B" },
        { 1003, "C" },
        { 1004, "D" },
        { 1005, "E" },
        { 1006, "F" },
        { 1007, "G" },
        { 1008, "H" },
        { 1009, "I" },
        { 1010, "J" },

        { 2001, "a" }
    };

    public async Task<Dictionary<int, List<string>>> GetShopItemList(int shopKind = 1001)
    {
        Dictionary<int, List<string>> rtValue = new();
        List<int> itemKeys = new();
        DropChanceCalculator drop = FindObjectOfType<DropChanceCalculator>();

        //throw player's level
        drop.SetLevel(await UserInfoManager.GetLevel());
        ItemType type = drop.RandomDropItem();
        Debug.Log($"item grade : {type}");

        string charType = await UserInfoManager.GetCharClass();
        Debug.Log($"charType : {charType}");

        switch (shopKind)
        {
            case 1001:
                itemKeys = new()
                {
                    1001, 1002, 1003, 1004, 1005
                };
                break;
            case 2001:
                itemKeys = new()
                {
                    1006, 1007, 1008, 1009, 1010
                };
                break;
        }

        rtValue = Inner_GetShopItemList(itemKeys);

        return rtValue;
    }

    private Dictionary<int, List<string>> Inner_GetShopItemList(List<int> itemKeys)
    {
        Dictionary<int, List<string>> rtValue = new();

        foreach (int key in itemKeys)
        {
            if (_itemName.ContainsKey(key))
            {
                string itemName = _itemName[key];
                int itemPrice = _items[key][0];

                //filter items
                if(itemPrice <= 0)
                {
                    continue;
                }

                List<string> itemData = new List<string>
                {
                    itemName, itemPrice.ToString()
                };
                rtValue.Add(key, itemData);
            }
        }

        return rtValue;
    }

    public static string GetItemNameByKey(int itemKey)
    {
        return _itemName[itemKey];
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
