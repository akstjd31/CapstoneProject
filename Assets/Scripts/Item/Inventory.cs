using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

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

        this.transform.SetAsLastSibling();
    }
#endif

    void Awake()
    {
        FreshSlot();

        spum_SpriteList = status.transform.Find("Root").GetComponent<SPUM_SpriteList>();
        playerCtrl = FindObjectOfType<PlayerCtrl>();
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
        if(playerCtrl.IsEnableStore())
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
                        selectedItem = items[0];
                        delIndex = 0;
                    }
                    else
                    {
                        string index = name.Split("(")[1];
                        index = index.Substring(0, index.Length - 1);    //1~11
                        //Debug.Log($"index : {index}");
                        delIndex = int.Parse(index);

                        selectedItem = items[delIndex];
                    }
                }

                Debug.Log($"delIndex : {delIndex}");

                if (selectedItem != null)
                {
                    int itemKey = selectedItem.itemID;
                    Debug.Log($"itemKey : {itemKey}");

                    NpcShop.SellItem(101, itemKey, 1);
                    DeleteItem_Index(delIndex);
                    Debug.Log("sell item complete");

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

        //refresh money
        if(playerCtrl == null || nowMoney == null)
        {
            playerCtrl = FindObjectOfType<PlayerCtrl>();
            nowMoney = GameObject.Find("DoubleCurrencyBox")?.GetComponentInChildren<Text>();
        }
        else if(nowMoney != null && playerCtrl.IsEnableInventory())
        {
            nowMoney.text = UserInfoManager.GetNowMoney().ToString();
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

    private void EquipItem()
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
                    TotalStatus(equippedSlot.item);

                    break;
                }
            }
        }
    }
    
    public void TotalStatus(Item equippedItem)
    {
        status.attackDamage = status.GetDefaultAttackDamage() + equippedItem.attackDamage;
        status.attackSpeed = equippedItem.attackSpeed;

        PlayerCtrl playerCtrl = status.GetComponent<PlayerCtrl>();
        playerCtrl.SetAnimSpeed(playerCtrl.GetAnimSpeed(status.attackSpeed));

        if (equippedSlot.item.bonusStat != BonusStat.NONE)
        {
            switch (equippedItem.bonusStat)
            {
                case BonusStat.HP:
                    status.MAXHP = status.GetDefaultHP() + equippedItem.addValue;
                    break;
                case BonusStat.MOVESPEED:
                    status.moveSpeed = status.GetDefaultMoveSpeed() + (int)equippedItem.addValue;
                    break;
                case BonusStat.EVASIONRATE:
                    status.evasionRate = status.GetDefaultEvasionRate() + equippedItem.addValue;
                    break;
            }
        }
    }
}