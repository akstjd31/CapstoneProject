using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShowOnSaleItem : MonoBehaviour
{
    [SerializeField]
    private GameObject shopView;
    [SerializeField]
    private GameObject item_Container;

    private GameObject shop;
    private Transform content;
    private List<GameObject> itemList = new List<GameObject>();
    private Button btn_reroll;
    private bool canReroll = true;  //condition of reroll

    public void ShowShopUI(int shopType)
    {
        if (shop == null)
        {
            shop = Instantiate(shopView, GameObject.Find("Canvas").transform);
            content = shop.transform.Find("Bag/Items/Viewport/Content");
            btn_reroll = GameObject.Find("ButtonReroll").GetComponent<Button>();

            if (content == null)
            {
                Debug.LogError("Can't Find ScrollView's content object");
                return;
            }

            GameObject.Find("CloseButton").GetComponent<Button>().onClick.AddListener(() =>
            {
                CloseShopUI();
            });
            btn_reroll.onClick.AddListener(() =>
            {
                Debug.Log("clicked reroll");

                if(canReroll)
                {
                    Reroll();
                }
            });
        }

        shop.SetActive(true);
        //Debug.Log($"content name : {shop.name} {content.name} / {content.transform.parent}");

        SetOnsaleList(shopType);
    }

    public void CloseShopUI()
    {
        itemList = new List<GameObject>();
        //판매 중이었던 아이템 삭제
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        shop.SetActive(false);
    }

    private void Reroll()
    {
        itemList = new List<GameObject>();
        //판매 중이었던 아이템 삭제
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        SetOnsaleList();
    }

    private async void SetOnsaleList(int shopKind = 1001)
    {
        GameObject temp;
        ItemSell showOnSaleItem = FindObjectOfType<ItemSell>();

        //이 부분이 Reroll의 영향을 받음
        Dictionary<int, List<string>> items = await showOnSaleItem.GetShopItemList(shopKind);

        int numOfItem = items.Count;
        //Debug.Log($"items : {numOfItem}");
        List<int> itemKeys = new(items.Keys);
        //List<string> itemName = items.Values.SelectMany(list => list).ToList();

        for (int i = 0; i < numOfItem; i++)
        {
            //Debug.Log($"Call SetOnsaleList: {i}");
            int index = i;

            temp = Instantiate(item_Container, content);
            temp.GetComponentInChildren<Text>().text = ItemSell.GetItemNameByKey(itemKeys[index]);
            temp.GetComponent<Button>().onClick.AddListener(() =>
            {
                // 클릭된 버튼을 찾기
                Button clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

                string itemText = ItemSell.GetItemNameByKey(itemKeys[index]);
                clickedButton.transform.Find("ItemName").GetComponent<Text>().text = itemText;
                Debug.Log($"call buyItem with button : index {index}, key {itemKeys[index]}, name {itemText}");

                NpcShop.BuyItem(itemKeys[index]);
            });

            itemList.Add(temp);
        }
    }
}