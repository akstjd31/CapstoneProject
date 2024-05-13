using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// 전체적인 아이템 관리 및 전달 스크립트
public class ItemManager : MonoBehaviour
{
    public ItemSO itemSO;

    public List<Item> warriorCommonList;
    public List<Item> warriorRareList;
    public List<Item> warriorLegendaryList;

    public List<Item> archerCommonList;
    public List<Item> archerRareList;
    public List<Item> archerLegendaryList;

    private Inventory inventory;

    [SerializeField] private List<int> randIdxList;
    // 아이템 등급에 따른 분류
    private void Awake()
    {
        warriorCommonList = new List<Item>();
        warriorRareList = new List<Item>();
        warriorLegendaryList = new List<Item>();

        archerCommonList = new List<Item>();
        archerRareList = new List<Item>();
        archerLegendaryList = new List<Item>();

        foreach (Item item in itemSO.itemList)
        {
            if (item.charType == CharacterType.WARRIOR)
            {
                switch (item.itemType)
                {
                    case ItemType.COMMON:
                        warriorCommonList.Add(item);
                        break;
                    case ItemType.RARE:
                        warriorRareList.Add(item);
                        break;
                    case ItemType.LEGENDARY:
                        warriorLegendaryList.Add(item);
                        break;
                }
            }

            else if (item.charType == CharacterType.ARCHER)
            {
                switch (item.itemType)
                {
                    case ItemType.COMMON:
                        archerCommonList.Add(item);
                        break;
                    case ItemType.RARE:
                        archerRareList.Add(item);
                        break;
                    case ItemType.LEGENDARY:
                        archerLegendaryList.Add(item);
                        break;
                }
            }
        }

        inventory = GameObject.FindGameObjectWithTag("Canvas").transform.Find("Inventory").GetComponent<Inventory>();
        inventory.SetItemManager(this);

        
    }

    private void Start()
    {
        if (PhotonNetwork.IsConnected)
        {
            // 포톤 네트워크가 연결된 상태에서만 작동하도록 함
            GetComponent<PhotonView>().RPC("SyncPlayerData", RpcTarget.AllBuffered, Random.Range(0, warriorCommonList.Count)); // 초기 데이터 동기화
        }
    }

    // 플레이어 데이터를 동기화하는 메서드
    [PunRPC]
    void SyncPlayerData(int value)
    {
        // 값 추가
        randIdxList.Add(value);
    }

    // 포톤 동기화를 위한 메서드
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 데이터를 보냄
            stream.SendNext(randIdxList.Count); // 리스트의 길이 전송
            foreach (int value in randIdxList)
            {
                stream.SendNext(value); // 리스트의 각 항목 전송
            }
        }
        else
        {
            // 데이터를 받음
            int count = (int)stream.ReceiveNext(); // 리스트의 길이 받음
            randIdxList.Clear(); // 기존 리스트 초기화
            for (int i = 0; i < count; i++)
            {
                int value = (int)stream.ReceiveNext(); // 리스트의 각 항목 받음
                randIdxList.Add(value); // 받은 값으로 리스트 갱신
            }
        }

    //    [PunRPC]
    //public void RandomCommonItemIndex(int viewID)
    //{
    //    PhotonView playerPV = PhotonView.Find(viewID);
    //    Status status = playerPV.GetComponent<Status>();
    //    PlayerCtrl playerCtrl = playerPV.GetComponent<PlayerCtrl>();

    //    if (status.charType.Equals("Warrior"))
    //    {
    //        playerCtrl.SetRandIndex(randIdx);
    //    }
    //    else
    //    {
    //        playerCtrl.SetRandIndex(randIdx);
    //    }
    //}

    // 아이템 드랍 확률결과로 나온 아이템 타입과 직업에 연관된 아이템 뽑기
    public Item GetRandomItemWithProbability(ItemType itemType, string charType)
    {
        Item resultItem = null;
        int randomIndex;

        if (charType.Equals("Warrior"))
        {
            switch (itemType)
            {
                case ItemType.COMMON:
                    randomIndex = Random.Range(0, warriorCommonList.Count);
                    resultItem = warriorCommonList[randomIndex];
                    break;
                case ItemType.RARE:
                    randomIndex = Random.Range(0, warriorRareList.Count);
                    resultItem = warriorRareList[randomIndex];
                    break;
                case ItemType.LEGENDARY:
                    randomIndex = Random.Range(0, warriorLegendaryList.Count);
                    resultItem = warriorLegendaryList[randomIndex];
                    break;
            }
        }

        else if (charType.Equals("Archer"))
        {
            switch (itemType)
            {
                case ItemType.COMMON:
                    randomIndex = Random.Range(0, archerCommonList.Count);
                    resultItem = archerCommonList[randomIndex];
                    break;
                case ItemType.RARE:
                    randomIndex = Random.Range(0, archerRareList.Count);
                    resultItem = archerRareList[randomIndex];
                    break;
                case ItemType.LEGENDARY:
                    randomIndex = Random.Range(0, archerLegendaryList.Count);
                    resultItem = archerLegendaryList[randomIndex];
                    break;
            }
        }

        return resultItem;
    }
}
