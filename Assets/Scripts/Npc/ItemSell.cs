using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//reference only class - no direct use (NpcShop.cs etc.)
public class ItemSell : MonoBehaviour
{
    private static bool executeInit = false;

    private static List<List<Item>> item_warrior = new List<List<Item>>(3);     //100~104 : common | 130~134 : rare | 160~164 : legendary
    private static List<List<Item>> item_archer = new List<List<Item>>(3);      //200~204 : common | 230~234 : rare | 260~264 : legendary

    //itemKey : buyValue(-1 : can not buy), sellValue(-1 : can not sell)
    private static Dictionary<int, List<int>> _items = new();
    private static Dictionary<int, string> _itemName = new();
    private static List<int> _saleItemKeys = new();

    private static readonly int numOfItem = 5; 

    private static void InitItemList()
    {
        for (int i = 0; i < 3; i++)
        {
            item_warrior.Add(new List<Item>());
            item_archer.Add(new List<Item>());
        }

        ItemManager itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
        item_warrior[0] = itemManager.warriorCommonList;
        item_warrior[1] = itemManager.warriorRareList;
        item_warrior[2] = itemManager.warriorLegendaryList;

        item_archer[0] = itemManager.archerCommonList;
        item_archer[1] = itemManager.archerRareList;
        item_archer[2] = itemManager.archerLegendaryList;

        int buyValue = -1, sellValue = -1;

        //set warrior weapon data
        for(int i = 0; i < 3; i++)
        {
            for(int j = 0; j < item_warrior[i].Count; j++)
            {
                switch(i)
                {
                    case 0:
                        buyValue = 10;
                        sellValue = 5;
                        break;
                    case 1:
                        buyValue = 25;
                        sellValue = 15;
                        break;
                    case 2:
                        buyValue = 50;
                        sellValue = 30;
                        break;
                }

                _itemName.Add(item_warrior[i][j].itemID, item_warrior[i][j].itemName);
                _items.Add(item_warrior[i][j].itemID, new() { buyValue, sellValue });
            }
        }
        //set warrior archer data
        for (int i = 0; i < 3; i++)
        {
            for(int j = 0; j < item_archer[i].Count; j++)
            {
                switch (i)
                {
                    case 0:
                        buyValue = 10;
                        sellValue = 5;
                        break;
                    case 1:
                        buyValue = 25;
                        sellValue = 15;
                        break;
                    case 2:
                        buyValue = 50;
                        sellValue = 30;
                        break;
                }

                _itemName.Add(item_archer[i][j].itemID, item_archer[i][j].itemName);
                _items.Add(item_archer[i][j].itemID, new() { buyValue, sellValue });
            }
        }

        executeInit = true;
        /*
        foreach (var kvp in _itemName)
        {
            Debug.Log("key: " + kvp.Key + ", name: " + kvp.Value);
        }
        Debug.Log($"_items : {_items.Count}/_itemName : {_itemName.Count}");
        */
    }

    public async Task<Dictionary<int, List<string>>> GetShopItemList()
    {
        Dictionary<int, List<string>> rtValue = new();
        List<int> itemKeys = new();
        DropChanceCalculator drop = FindObjectOfType<DropChanceCalculator>();

        if(!executeInit)
        {
            InitItemList();
        }

        //throw player's level
        drop.SetLevel(await UserInfoManager.GetLevel());
        string charType = await UserInfoManager.GetCharClass();
        //selected item list
        List<Item> showItem = new();

        for (int i = 0; i < numOfItem; i++)
        {
            ItemType type = drop.RandomDropItem();
            int type_int = type == ItemType.COMMON ? 0
                : type == ItemType.RARE ? 1 : 2;
            int index = -1;

            if (charType.Equals("Warrior"))
            {
                index = Random.Range(0, item_warrior[type_int].Count);

                //prevent duplication
                if (showItem.Contains(item_warrior[type_int][index]))
                {
                    i--;
                    continue;
                }
                showItem.Add(item_warrior[type_int][index]);
            }
            if (charType.Equals("Archer"))
            {
                index = Random.Range(0, item_archer[type_int].Count);

                //prevent duplication
                if (showItem.Contains(item_archer[type_int][index]))
                {
                    i--;
                    continue;
                }
                showItem.Add(item_archer[type_int][index]);
            }
        }


        //get showItem's key-list
        _saleItemKeys = new();
        for(int i = 0; i < showItem.Count; i++)
        {
            _saleItemKeys.Add(showItem[i].itemID);
        }

        rtValue = Inner_GetShopItemList(_saleItemKeys);

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

                List<string> itemData = new()
                {
                    itemName, itemPrice.ToString()
                };
                rtValue.Add(key, itemData);
            }
        }

        return rtValue;
    }

    public static Sprite GetItemImageByKey(int itemKey)
    {
        Sprite rtImage = null;
        List<Item> filterItems;
        int type = -1;

        //warrior
        if(itemKey < 200)
        {
            switch(itemKey / 10 % 10)
            {
                case 0:
                    type = 0;
                    break;
                case 3:
                    type = 1;
                    break;
                case 6:
                    type = 2;
                    break;
            }
            filterItems = item_warrior[type];
        }
        //archer
        else
        {
            switch (itemKey / 10 % 10)
            {
                case 0:
                    type = 0;
                    break;
                case 3:
                    type = 1;
                    break;
                case 6:
                    type = 2;
                    break;
            }
            filterItems = item_archer[type];
        }

       for(int i = 0; i < filterItems.Count; i++)
        {
            if (filterItems[i].itemID == itemKey)
            {
                rtImage = filterItems[i].itemImage;
                break;
            }
        }

        return rtImage;
    }

    public static string GetItemNameByKey(int itemKey)
    {
        return _itemName[itemKey];
    }
    
    public static Item GetItemByKey(int itemKey)
    {
        Item rtItem = null;
        List<Item> filterItems;
        int type = -1;

        //warrior
        if (itemKey < 200)
        {
            switch (itemKey / 10 % 10)
            {
                case 0:
                    type = 0;
                    break;
                case 3:
                    type = 1;
                    break;
                case 6:
                    type = 2;
                    break;
            }
            filterItems = item_warrior[type];
        }
        //archer
        else
        {
            switch (itemKey / 10 % 10)
            {
                case 0:
                    type = 0;
                    break;
                case 3:
                    type = 1;
                    break;
                case 6:
                    type = 2;
                    break;
            }
            filterItems = item_archer[type];
        }

        for (int i = 0; i < filterItems.Count; i++)
        {
            if (filterItems[i].itemID == itemKey)
            {
                rtItem = filterItems[i];
                break;
            }
        }

        return rtItem;
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
