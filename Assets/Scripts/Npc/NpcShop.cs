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

        //구매 가능한 npc
        if (npcType == 0 || npcType == 1)
        {
            int itemCost = ItemSell.BuyItem(itemKey);

            if(itemCost == -1)
            {
                throw new NpcStoreException("you can't buy this item");
            }

            //구매를 완료하고 유저의 돈을 증감시킴
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

        //판매 가능한 npc
        if (npcType == 0 || npcType == 2)
        {
            int itemCost = ItemSell.BuyItem(itemKey);

            if (itemCost == -1)
            {
                throw new NpcStoreException("you can't sell this item");
            }

            //판매를 완료하고 유저의 돈을 증감시킴
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

        // npc의 키에 해당하는 문서 가져오기
        DocumentSnapshot npcSnapshot = await doc_info.GetSnapshotAsync();

        if (npcSnapshot.Exists)
        {
            // npc의 키에 해당하는 데이터가 존재하는 경우
            Dictionary<string, object> npcData = npcSnapshot.ToDictionary();

            // npc의 키에 해당하는 데이터에서 sellType 가져오기
            if (npcData.ContainsKey(npcKey.ToString()))
            {
                Dictionary<string, object> npcInfo = (Dictionary<string, object>)npcData[npcKey.ToString()];
                if (npcInfo.ContainsKey("sellType"))
                {
                    // sellType 값을 반환
                    return (int)npcInfo["sellType"];
                }
            }
        }

        // npc의 키에 해당하는 데이터가 없거나 sellType이 없는 경우 기본값인 -1 반환
        return -1;
    }

    public static void MakeNpcShopDB()
    {
        MakeNpcShopDB_Async();
    }

    private static async void MakeNpcShopDB_Async()
    {
        // Firestore 인스턴스 생성
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // 데이터를 저장할 컬렉션과 문서 참조
        CollectionReference coll_npc = db.Collection("NpcInfo");

        string npcName = "";
        int npcKey = 101;       //101~
        int sellType = -1, shopKind = -1;
        bool isLastNpc = false;
        Dictionary<string, object> npcContainer;

        //npc 공용 데이터(필요 시 추가)

        //개별 npc 데이터
        while (true)
        {
            switch (npcKey)
            {
                case 101:
                    npcName = "A";
                    sellType = 0;               //0 : 판매/구매 가능, 1 : 유저가 구매만 가능, 2 : 유저가 판매만 가능
                    shopKind = 1001;            //shopKind가 동일하면 동일한 아이템을 판매 (+ 1000~ : 장비, 2000~ : 소모품)
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
