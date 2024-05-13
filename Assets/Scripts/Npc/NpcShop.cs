using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NpcShop : MonoBehaviour
{
    //npcKey : 101~
    //sellType : 0,1,2 (buy&sell, only buy, only sell)
    //shopKind : 1001~, 2001~
    //itemKey : 1001~, 2001~

    public static void Test_Buy()
    {
        BuyItem(101, 1001, 1);
    }
    public static void Test_Sell()
    {
        SellItem(102, 1004, 1);
    }

    //release method
    public static void BuyItem(int itemKey)
    {
        //read item name
        Debug.Log($"called BuyItem method : {itemKey}");
        BuyItem(101, itemKey, 1);
    }

    //semaphore for getting or setting money
    private static SemaphoreSlim semaphore = new SemaphoreSlim(1);

    public static void ReleaseSemaphore()
    {
        semaphore.Release();
    } 

    public static void BuyItem(int npcKey, int itemKey, int count)
    {
        Debug.Log($"Buy item : npc({npcKey}), item({itemKey}), count({count})");
        BuyItem_Async(npcKey, itemKey, count);
    }

    private static async void BuyItem_Async(int npcKey, int itemKey, int count)
    {
        await semaphore.WaitAsync();

        try
        {
            int npcType = await GetNpcType(npcKey);
            int money = 0;

            if (npcType == 0 || npcType == 1)
            {
                int itemCost = ItemSell.BuyItemCost(itemKey);

                if (itemCost == -1)
                {
                    throw new NpcStoreException("you can't buy this item");
                }

                money = itemCost * count;

                int userMoney = await UserInfoManager.GetUserMoney_Async();
                //Debug.Log($"userMoney : {userMoney}");
                if (userMoney < money)
                {
                    throw new NpcStoreException("No money, you can't buy this item");
                }

                AdjustMoney(-money);
                Inventory inv = FindObjectOfType<Inventory>();
                inv.AddItem(ItemSell.GetItemByKey(itemKey));
            }
            else
            {
                throw new NpcStoreException("you can't buy item with this npc");
            }
        }
        finally
        {
            //Release in UserInfoManager
            //semaphore.Release();
        }
    }
    public static void SellItem(int npcKey, int itemKey, int count)
    {
        Debug.Log($"Sell item : npc({npcKey}), item({itemKey}), count({count})");
        SellItem_Async(npcKey, itemKey, count);
    }

    private static async void SellItem_Async(int npcKey, int itemKey, int count)
    {
        await semaphore.WaitAsync();

        try
        {
            int npcType = await GetNpcType(npcKey);
            int money = 0;

            //Debug.Log($"npcType : {npcType}");
            if (npcType == 0 || npcType == 2)
            {
                int itemCost = ItemSell.SellItemCost(itemKey);

                if (itemCost == -1)
                {
                    throw new NpcStoreException("you can't sell this item");
                }

                money = itemCost * count;
                AdjustMoney(money);
            }
            else
            {
                throw new NpcStoreException("you can't sell item with this npc");
            }
        }
        finally
        {
            //Release in UserInfoManager
            //semaphore.Release();
        }
    }
    
    private static void AdjustMoney(int value)
    {
#pragma warning disable CS4014
        UserInfoManager.SetUserMoney_Async(value);
#pragma warning restore CS4014
    }

    private static async Task<int> GetNpcType(int npcKey)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        CollectionReference coll_npc = db.Collection("NpcInfo");

        DocumentReference doc_npc = coll_npc.Document(npcKey.ToString());

        DocumentSnapshot npcSnapshot = await doc_npc.GetSnapshotAsync();

        if (npcSnapshot.Exists)
        {
            Dictionary<string, object> npcData = npcSnapshot.ToDictionary();

            if (npcData.ContainsKey("sellType"))
            {
                object sellTypeObj = npcData["sellType"];
                if (sellTypeObj is long sellTypeLong)
                {
                    return (int)sellTypeLong;
                }
                else
                {
                    throw new NpcStoreException("Failed GetNpcType : npcData[\"sellType\"] can't be cast to int");
                }
            }
        }

        return -1;
    }

    public static void MakeNpcShopDB()
    {
        MakeNpcShopDB_Async();
    }

    private static async void MakeNpcShopDB_Async()
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        CollectionReference coll_npc = db.Collection("NpcInfo");

        string npcName = "";
        int npcKey = 101;       //101~
        int sellType = -1, shopKind = -1;
        bool isLastNpc = false;
        Dictionary<string, object> npcContainer;

        while (true)
        {
            switch (npcKey)
            {
                case 101:
                    npcName = "A";
                    sellType = 0;
                    shopKind = 1001;
                    break;
                case 102:
                    npcName = "B";
                    sellType = 0;
                    shopKind = 2001;

                    isLastNpc = true;
                    break;
            }

            DocumentReference doc_npc = coll_npc.Document(npcKey.ToString());

            npcContainer = new()
            {
                { "key", npcKey },
                { "npcName", npcName },
                { "sellType", sellType },
                { "shopKind", shopKind }
            };

            await doc_npc.SetAsync(npcContainer);

            if(isLastNpc)
            {
                return;
            }

            npcKey++;
        }
    }
}

class NpcStoreException : Exception
{
    public NpcStoreException(string message) : base(message)
    {
    }
}
