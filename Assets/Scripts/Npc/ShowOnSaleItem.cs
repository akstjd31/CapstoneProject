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

    void Start()
    {
        shop = Instantiate(shopView, GameObject.Find("Canvas").transform);
        content = shop.transform.Find("Bag/Items/Viewport/Content");
        //SetOnsaleList();

        Debug.Log("called start");
    }

    public void ShowShopUI()
    {
        shop = Instantiate(shopView, GameObject.Find("Canvas").transform);
        content = shop.transform.Find("Bag/Items/Viewport/Content");

        if(content == null)
        {
            Debug.LogError("Can't Find ScrollView's content object");
            return;
        }

        SetOnsaleList();
    }

    private void SetOnsaleList()
    {
        int numOfItem = 20;
        GameObject temp;

        for (int i = 0; i < numOfItem; i++)
        {
            Debug.Log($"Call SetOnsaleList: {i}");

            temp = Instantiate(item_Container, content);
            temp.GetComponentInChildren<Text>().text = $"{i}번째";
            temp.GetComponent<Button>().onClick.AddListener(() =>
            {
                // 클릭된 버튼을 찾기
                Button clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
                // 클릭된 버튼의 자식 중 "Title" 이름의 오브젝트 찾기
                string titleText = clickedButton.transform.Find("Title")?.GetComponent<Text>().text;

                NpcShop.BuyItem(titleText);
            });

            itemList.Add(temp);
        }
    }
}
