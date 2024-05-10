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
    //������ ������ �������� ������ ��, �� ��ư�� ���
    public static void BuyItem(int itemKey)
    {
        //read item name
        Debug.Log($"called BuyItem method : {itemKey}");
        //������ 1���� ���� ����
        //���� UI�� ���� ���� �� ���� ���� ����
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

            //Debug.Log($"npcType : {npcType}");
            //���� ������ npc
            if (npcType == 0 || npcType == 1)
            {
                int itemCost = ItemSell.BuyItemCost(itemKey);

                if (itemCost == -1)
                {
                    throw new NpcStoreException("you can't buy this item");
                }

                money = itemCost * count;

                //������ ���� ������� �˻�
                int userMoney = await UserInfoManager.GetUserMoney_Async();
                //Debug.Log($"userMoney : {userMoney}");
                if (userMoney < money)
                {
                    throw new NpcStoreException("No money, you can't buy this item");
                }

                //���Ÿ� �Ϸ��ϰ� ������ ���� ������Ŵ
                AdjustMoney(-money);
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
            //�Ǹ� ������ npc
            if (npcType == 0 || npcType == 2)
            {
                int itemCost = ItemSell.SellItemCost(itemKey);

                if (itemCost == -1)
                {
                    throw new NpcStoreException("you can't sell this item");
                }

                //�ǸŸ� �Ϸ��ϰ� ������ ���� ������Ŵ
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

        // npc�� Ű�� �ش��ϴ� ���� ��������
        DocumentSnapshot npcSnapshot = await doc_npc.GetSnapshotAsync();

        if (npcSnapshot.Exists)
        {
            // npcKey�� �ش��ϴ� �����Ͱ� �����ϴ� ���
            Dictionary<string, object> npcData = npcSnapshot.ToDictionary();

            // npcKey�� �ش��ϴ� �����Ϳ��� sellType ��������
            if (npcData.ContainsKey("sellType"))
            {
                // sellType ���� ��ȯ
                object sellTypeObj = npcData["sellType"];
                if (sellTypeObj is long sellTypeLong)
                {
                    // long���� ĳ������ �� int�� ��ȯ
                    return (int)sellTypeLong;
                }
                else
                {
                    // ĳ���� �Ұ����� ��� null ��ȯ
                    throw new NpcStoreException("Failed GetNpcType : npcData[\"sellType\"] can't be cast to int");
                }
            }
        }

        // npc�� Ű�� �ش��ϴ� �����Ͱ� ���ų� sellType�� ���� ��� �⺻���� -1 ��ȯ
        return -1;
    }

    public static void MakeNpcShopDB()
    {
        MakeNpcShopDB_Async();
    }

    private static async void MakeNpcShopDB_Async()
    {
        // Firestore �ν��Ͻ� ����
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // �����͸� ������ �÷��ǰ� ���� ����
        CollectionReference coll_npc = db.Collection("NpcInfo");

        string npcName = "";
        int npcKey = 101;       //101~
        int sellType = -1, shopKind = -1;
        bool isLastNpc = false;
        Dictionary<string, object> npcContainer;

        //npc ���� ������(�ʿ� �� �߰�)

        //���� npc ������
        while (true)
        {
            switch (npcKey)
            {
                case 101:
                    npcName = "A";
                    sellType = 0;               //0 : �Ǹ�/���� ����, 1 : ������ ���Ÿ� ����, 2 : ������ �ǸŸ� ����
                    shopKind = 1001;            //shopKind�� �����ϸ� ������ �������� �Ǹ� (+ 1000~ : ���, 2000~ : �Ҹ�ǰ)
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
