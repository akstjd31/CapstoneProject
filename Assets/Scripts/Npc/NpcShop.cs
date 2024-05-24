using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NpcShop : MonoBehaviour
{
    //semaphore for getting or setting money
    private static SemaphoreSlim semaphore = new SemaphoreSlim(1);

    public static void ReleaseSemaphore()
    {
        semaphore.Release();
    } 

    public static void BuyItem(int itemKey, int count = 1)
    {
        //Debug.Log($"Buy item : npc({npcKey}), item({itemKey}), count({count})");
        BuyItem_Async(itemKey, count);
    }

    private static async void BuyItem_Async(int itemKey, int count)
    {
        await semaphore.WaitAsync();

        try
        {
            int itemCost = ItemSell.BuyItemCost(itemKey);

            if (itemCost == -1)
            {
                throw new NpcStoreException("you can't buy this item");
            }

            int money = itemCost * count;

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
        finally
        {
            //Semaphore Release in UserInfoManager
        }
    }
    public static void SellItem(int itemKey, int count = 1)
    {
        //Debug.Log($"Sell item : npc({npcKey}), item({itemKey}), count({count})");
        SellItem_Async(itemKey, count);
    }

    private static async void SellItem_Async(int itemKey, int count)
    {
        await semaphore.WaitAsync();

        try
        {
            int itemCost = ItemSell.SellItemCost(itemKey);

            if (itemCost == -1)
            {
                throw new NpcStoreException("you can't sell this item");
            }

            int money = itemCost * count;
            AdjustMoney(money);
        }
        finally
        {
            //Semaphore Release in UserInfoManager
        }
    }
    
    private static void AdjustMoney(int value)
    {
#pragma warning disable CS4014
        UserInfoManager.SetUserMoney_Async(value);
#pragma warning restore CS4014
    }
}

class NpcStoreException : Exception
{
    public NpcStoreException(string message) : base(message)
    {
    }
}
