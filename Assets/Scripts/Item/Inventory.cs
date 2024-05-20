using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class Inventory : MonoBehaviour
{
    public Item equippedItem;
    public List<Item> items;

    [SerializeField]
    private Transform equippedSlotParent, intventorySlotParent;

    [SerializeField]
    private Slot equippedSlot;

    [SerializeField]
    private Slot[] inventorySlots;

    [SerializeField]
    private Drag equippedDrag;

    [SerializeField]
    private Drag[] inventoryDrags;

    public Button closeButton;

    public GameObject explanation;

    GraphicRaycaster raycaster;
    PointerEventData pointerEventData;
    EventSystem eventSystem;

    [SerializeField] private Status status;
    [SerializeField] private SPUM_SpriteList spum_SpriteList;
    [SerializeField] private ItemManager itemManager;

    private PlayerCtrl playerCtrl;
    private Text nowMoney;

#if UNITY_EDITOR
    private void OnValidate()
    {
        equippedSlot = equippedSlotParent.GetComponentInChildren<Slot>();
        equippedDrag = equippedSlotParent.GetComponentInChildren<Drag>();

        inventorySlots = intventorySlotParent.GetComponentsInChildren<Slot>();
        inventoryDrags = intventorySlotParent.GetComponentsInChildren<Drag>();

        // GraphicRaycaster 가져오기
        raycaster = this.transform.root.GetComponent<GraphicRaycaster>();
        // EventSystem 가져오기
        eventSystem = this.transform.root.GetComponent<EventSystem>();

    }
#endif

    void Awake()
    {
        FreshSlot();
        playerCtrl = FindObjectOfType<PlayerCtrl>();

        spum_SpriteList = status.transform.Find("Root").GetComponent<SPUM_SpriteList>();
    }

    public Item GetEquippedItem()
    {
        return equippedSlot.item;
    }

    public void SetStatus(Status status)
    {
        this.status = status;
    }

    public void SetItemManager(ItemManager itemManager)
    {
        this.itemManager = itemManager;
    }

    private void Update()
    {
        EquipItem();
        // 인벤토리가 캔버스의 마지막 자식이 아닌경우 마지막으로 설정
        //if (!this.transform.root.GetChild(this.transform.root.childCount).Equals(this.transform))
        //{
        //    this.transform.SetAsLastSibling();
        //}

        // 마우스 위치에서 PointerEventData 생성
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        if (results.Count == 0)
        {
            explanation.SetActive(false);
            return;
        }

        //상점인 경우
        if(playerCtrl != null && playerCtrl.IsEnableStore())
        {
            if(Input.GetMouseButtonDown(0))
            {
                Item selectedItem = null;
                int delIndex = -1;

                foreach (var result in results)
                {
                    string name = result.gameObject.transform.parent.name;
                    //Debug.Log($"result : {name}");   //Slot ~ Slot (11)

                    if(!name.StartsWith("Slot"))
                    {
                        continue;
                    }

                    if (name.Equals("Slot"))
                    {
                        selectedItem = items[0] == null ? null : items[0];
                        delIndex = 0;
                    }
                    else
                    {
                        string index = name.Split("(")[1];
                        index = index.Substring(0, index.Length - 1);    //1~11
                        delIndex = int.Parse(index);

                        selectedItem = items[delIndex] == null ? null : items[delIndex];
                    }
                }

                if (selectedItem != null)
                {
                    int itemKey = selectedItem.itemID;

                    NpcShop.SellItem(itemKey, 1);
                    DeleteItem_Index(delIndex);

                    return;
                }
            }
        }

        foreach (RaycastResult result in results)
        {
            Drag drag = result.gameObject.GetComponent<Drag>();

            if (drag != null && drag.isDraggable)
            {
                explanation.SetActive(true);

                RectTransform dragRectTransform = drag.GetComponent<RectTransform>();
                float dragWidthHalf = dragRectTransform.rect.width;
                float dragHeightHalf = dragRectTransform.rect.height;

                if (!drag.isEquippedItem)
                {
                    explanation.transform.position = new Vector3(
                        pointerEventData.position.x + dragWidthHalf,
                        pointerEventData.position.y + dragHeightHalf * 1.25f
                    );
                }
                else
                {
                    explanation.transform.position = new Vector3(
                        pointerEventData.position.x - dragWidthHalf * 0.75f,
                        pointerEventData.position.y - dragHeightHalf * 0.75f
                    );
                }


                Slot slot = drag.GetComponent<Slot>();

                explanation.GetComponent<Explanation>().InfoSetting(slot.item.itemImage,
                                                                    slot.item.itemName,
                                                                    slot.item.attackDamage,
                                                                    slot.item.attackSpeed,
                                                                    slot.item.bonusStat,
                                                                    slot.item.addValue,
                                                                    slot.item.itemType,
                                                                    slot.item.charType);
            }
            else
            {
                explanation.SetActive(false);
            }
        }
    }



    public void ShowExplanationStore()
    {
        // 마우스 위치에서 PointerEventData 생성
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerEventData, results);

        Debug.Log($"ShowExplanationStore : {results.Count}");
        if (results.Count == 0)
        {
            explanation.SetActive(false);
            return;
        }

        foreach (RaycastResult result in results)
        {
            //drag 변수 대체 필요
            Drag drag = result.gameObject.GetComponent<Drag>();

            if (drag != null && drag.isDraggable)
            {
                explanation.SetActive(true);

                RectTransform dragRectTransform = drag.GetComponent<RectTransform>();
                float dragWidthHalf = dragRectTransform.rect.width;
                float dragHeightHalf = dragRectTransform.rect.height;

                if (!drag.isEquippedItem)
                {
                    explanation.transform.position = new Vector3(
                        pointerEventData.position.x + dragWidthHalf,
                        pointerEventData.position.y + dragHeightHalf * 1.25f
                    );
                }
                else
                {
                    explanation.transform.position = new Vector3(
                        pointerEventData.position.x - dragWidthHalf * 0.75f,
                        pointerEventData.position.y - dragHeightHalf * 0.75f
                    );
                }


                Slot slot = drag.GetComponent<Slot>();

                explanation.GetComponent<Explanation>().InfoSetting(slot.item.itemImage,
                                                                    slot.item.itemName,
                                                                    slot.item.attackDamage,
                                                                    slot.item.attackSpeed,
                                                                    slot.item.bonusStat,
                                                                    slot.item.addValue,
                                                                    slot.item.itemType,
                                                                    slot.item.charType);
            }
            else
            {
                explanation.SetActive(false);
            }
        }
    }

    public void OnClickCloseButton()
    {
        this.gameObject.SetActive(false);
    }

    public void FreshSlot()
    {
        int i = 0;

        equippedSlot.item = equippedItem;
        equippedDrag.isDraggable = true;
        equippedDrag.defaultItem = equippedItem;

        for (i = 0; i < items.Count && i < inventorySlots.Length; i++)
        {
            inventorySlots[i].item = items[i];
            inventoryDrags[i].isDraggable = true;
            inventoryDrags[i].defaultItem = items[i];
        }
        for (; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].item = null;
            inventoryDrags[i].isDraggable = false;
            inventoryDrags[i].defaultItem = null;
        }

        if (playerCtrl != null)
        {
            Refresh_InvMoney();
        }
    }

    public int GetInventorySlotLength()
    {
        return inventorySlots.Length;
    }

    public void AddItem(Item item)
    {
        if (items.Count < inventorySlots.Length)
        {
            items.Add(item);
            FreshSlot();
        }
    }

    public void DeleteItem_Index(int index)
    {
        if (items.Count != 0)
        {
            items.RemoveAt(index);
            FreshSlot();
        }
    }

    public void EquipItem()
    {
        if (status != null && status.GetComponent<PhotonView>().IsMine)
        {
            if (spum_SpriteList != null && equippedSlot.item != null)
            {
                // 스펌 캐릭터에 존재하는 Weapon항목 스프라이트 중에서 무기를 들고 있는 손에 스프라이트 변경
                foreach (SpriteRenderer sprite in spum_SpriteList._weaponList)
                {
                    if (sprite.sprite != null && sprite != equippedSlot.item.itemImage)
                    {
                        sprite.sprite = equippedSlot.item.itemImage;
                        equippedItem = equippedSlot.item;
                        playerCtrl.SetEquipItem(equippedItem);
                        playerCtrl.TotalStatus(equippedSlot.item);

                        break;
                    }
                }
            }
        }
    }

    public void Refresh_InvMoney()
    {
        //refresh money
        if (playerCtrl == null || nowMoney == null)
        {
            playerCtrl = FindObjectOfType<PlayerCtrl>();
            nowMoney = GameObject.Find("DoubleCurrencyBox")?.GetComponentInChildren<Text>();
        }

        if (nowMoney != null && playerCtrl.IsEnableInventory())
        {
            nowMoney.text = UserInfoManager.GetNowMoney().ToString();
            //Canvas.ForceUpdateCanvases();
            nowMoney.enabled = true;
        }
    }
}