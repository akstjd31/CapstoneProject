using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowOnSaleItem : MonoBehaviour
{
    [SerializeField]
    private GameObject shopView;
    [SerializeField]
    private GameObject item_Container;

    private GameObject shop;
    private Transform content;
    private List<GameObject> itemList = new();
    private Button btn_reroll;

    private Inventory inv;

    public AudioSource audioSource;
    public AudioClip openShopSound;
    public AudioClip closeShopSound;

    public void ShowShopUI()
    {
        if (shop == null)
        {
            audioSource.PlayOneShot(openShopSound);

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
                int money = UserInfoManager.GetNowMoney();

                if(money >= 5)
                {
                    Reroll();
                }
            });
        }

        shop.SetActive(true);
        //Debug.Log($"content name : {shop.name} {content.name} / {content.transform.parent}");

        SetOnsaleList();
    }

    public void CloseShopUI()
    {
        audioSource.PlayOneShot(closeShopSound);
        itemList = new List<GameObject>();
        //�Ǹ� ���̾��� ������ ����
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        shop.SetActive(false);
    }

    private async void Reroll()
    {
        await UserInfoManager.SetUserMoney_Async(-5);

        itemList = new List<GameObject>();
        //�Ǹ� ���̾��� ������ ����
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        SetOnsaleList();
    }

    private async void SetOnsaleList()
    {
        GameObject temp;
        ItemSell showOnSaleItem = FindObjectOfType<ItemSell>();

        //�� �κ��� Reroll�� ������ ����
        Dictionary<int, List<string>> items = await showOnSaleItem.GetShopItemList();

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
                // Ŭ���� ��ư�� ã��
                Button clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

                //Debug.Log($"call buyItem with button : index {index}, key {itemKeys[index]}");
                    
                NpcShop.BuyItem(itemKeys[index]);
            });

            //setting name & value
            string itemText = ItemSell.GetItemNameByKey(itemKeys[index]);
            temp.transform.Find("ItemName").GetComponent<Text>().text = itemText;
            int itemValue = ItemSell.BuyItemCost(itemKeys[index]);
            temp.transform.Find("ItemValue").GetComponent<TextMeshProUGUI>().text = AddCommasToNumber(itemValue);
            Sprite itemImage = ItemSell.GetItemImageByKey(itemKeys[index]);
            temp.transform.Find("ItemImage").GetComponent<RawImage>().texture = itemImage.texture;

            /*
            //explanation with mouse hover
            EventTrigger trigger = temp.gameObject.AddComponent<EventTrigger>();

            // PointerEnter �̺�Ʈ �߰�
            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener((data) => { OnPointerEnter(); });
            trigger.triggers.Add(pointerEnter);

            // PointerExit �̺�Ʈ �߰�
            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener((data) => { OnPointerExit(); });
            trigger.triggers.Add(pointerExit);
            */

            itemList.Add(temp);
        }
    }

    void OnPointerEnter()
    {
        inv = FindObjectOfType<Inventory>();
        inv.ShowExplanationStore();
    }

    void OnPointerExit()
    {
        inv.ShowExplanationStore();
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