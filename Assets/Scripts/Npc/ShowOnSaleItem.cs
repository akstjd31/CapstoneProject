using System.Collections;
using System.Collections.Generic;
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

    /*
    void Start()
    {
        shop = Instantiate(shopView, GameObject.Find("Canvas").transform);
        content = shop.transform.Find("Bag/Items/Viewport/Content");
        //SetOnsaleList();

        Debug.Log("called start");
    }
    */

    public void ShowShopUI()
    {
        shop = Instantiate(shopView, GameObject.Find("Canvas").transform);
        content = shop.transform.Find("Bag/Items/Viewport/Content");

        if(content == null)
        {
            Debug.LogError("Can't Find ScrollView's content object");
            return;
        }
        Debug.Log($"content name : {shop.name} {content.name} / {content.transform.parent}");

        SetOnsaleList();
    }

    public void CloseShopUI()
    {
        itemList = null;
        Destroy(shop);

        Debug.Log($"Destroy : {shop == null} {content == null}");
    }

    private void SetOnsaleList(int shopKind = 1001)
    {
        GameObject temp;
        ItemSell showOnSaleItem = FindObjectOfType<ItemSell>();

        Dictionary<string, int> items = showOnSaleItem.GetShopItemList();
        int numOfItem = items.Count;
        Debug.Log($"items : {numOfItem}");
        List<string> itemName = new(items.Keys);

        for (int i = 0; i < numOfItem; i++)
        {
            //Debug.Log($"Call SetOnsaleList: {i}");
            int index = i;

            temp = Instantiate(item_Container, content);
            temp.GetComponentInChildren<Text>().text = itemName[index];
            temp.GetComponent<Button>().onClick.AddListener(() =>
            {
                // 클릭된 버튼을 찾기
                Button clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                string itemText = itemName[index];
                clickedButton.transform.Find("ItemName").GetComponent<Text>().text = itemText;
                Debug.Log($"call buyItem with button : index {index}, title {itemText}");

                NpcShop.BuyItem(itemText);
            });

            itemList.Add(temp);
        }
    }
}
