using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class NpcShop : MonoBehaviour
{
    //npcKey : 101~
    //sellType : 0,1,2 (buy&sell, only buy, only sell)
    //shopKind : 1001~, 2001~

    private static void BuyItem(int npcKey, int itemKey, int count)
    {
        BuyItem_Async(npcKey, itemKey, count);
    }

    private static async void BuyItem_Async(int npcKey, int itemKey, int count)
    {
        int npcType = await GetNpcType(npcKey);
        int money = 0;

        //���� ������ npc
        if (npcType == 0 || npcType == 1)
        {
            int itemCost = ItemSell.BuyItem(itemKey);

            if(itemCost == -1)
            {
                throw new NpcStoreException("you can't buy this item");
            }

            //���Ÿ� �Ϸ��ϰ� ������ ���� ������Ŵ
            money = itemCost * count;
            AdjustMoney(-money);
        }
        else
        {
            throw new NpcStoreException("you can't buy item with this npc");
        }
    }
    private static void SellItem(int npcKey, int itemKey, int count)
    {
        SellItem_Async(npcKey, itemKey, count);
    }

    private static async void SellItem_Async(int npcKey, int itemKey, int count)
    {
        int npcType = await GetNpcType(npcKey);
        int money = 0;

        //�Ǹ� ������ npc
        if (npcType == 0 || npcType == 2)
        {
            int itemCost = ItemSell.BuyItem(itemKey);

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
    
    private static async void AdjustMoney(int value)
    {

    }

    private static async Task<int> GetNpcType(int npcKey)
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        CollectionReference coll_npc = db.Collection("Npc");

        DocumentReference doc_info = coll_npc.Document("Info");

        // npc�� Ű�� �ش��ϴ� ���� ��������
        DocumentSnapshot npcSnapshot = await doc_info.GetSnapshotAsync();

        if (npcSnapshot.Exists)
        {
            // npc�� Ű�� �ش��ϴ� �����Ͱ� �����ϴ� ���
            Dictionary<string, object> npcData = npcSnapshot.ToDictionary();

            // npc�� Ű�� �ش��ϴ� �����Ϳ��� sellType ��������
            if (npcData.ContainsKey(npcKey.ToString()))
            {
                Dictionary<string, object> npcInfo = (Dictionary<string, object>)npcData[npcKey.ToString()];
                if (npcInfo.ContainsKey("sellType"))
                {
                    // sellType ���� ��ȯ
                    return (int)npcInfo["sellType"];
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
