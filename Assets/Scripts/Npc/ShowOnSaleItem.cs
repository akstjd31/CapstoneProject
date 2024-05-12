using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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

                Debug.Log($"call buyItem with button : index {index}, key {itemKeys[index]}");
                    
                NpcShop.BuyItem(itemKeys[index]);
            });

            //setting name & value
            string itemText = ItemSell.GetItemNameByKey(itemKeys[index]);
            temp.transform.Find("ItemName").GetComponent<Text>().text = itemText;
            int itemValue = ItemSell.BuyItemCost(itemKeys[index]);
            temp.transform.Find("ItemValue").GetComponent<TextMeshProUGUI>().text = AddCommasToNumber(itemValue);

            itemList.Add(temp);
        }
    }

    private static string AddCommasToNumber(int number)
    {
        string numberStr = number.ToString();

        if(numberStr.Length <= 3)
        {
            return numberStr;
        }

        int commaPosition = numberStr.Length % 3;
        if (commaPosition == 0)
        {
            commaPosition = 3;
        }
        
        string result = "";
        for (int i = 0; i < numberStr.Length; i++)
        {
            result += numberStr[i];
            if (i + 1 == commaPosition && i < numberStr.Length - 1)
            {
                result += ",";
                commaPosition += 3;
            }
        }

        return result;
    }
}