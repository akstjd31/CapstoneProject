using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
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
    }

    private void Update()
    {
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

        foreach (RaycastResult result in results)
        {
            Drag drag = result.gameObject.GetComponent<Drag>();

            if (drag != null && drag.isDraggable)
            {
                explanation.SetActive(true);

                RectTransform dragRectTransform = drag.GetComponent<RectTransform>();
                float dragWidthHalf = dragRectTransform.rect.width;
                float dragHeightHalf = dragRectTransform.rect.height;

                explanation.transform.position = new Vector3(
                    pointerEventData.position.x + dragWidthHalf,
                    pointerEventData.position.y + dragHeightHalf
                );

                Slot slot = drag.GetComponent<Slot>();

                explanation.GetComponent<Explanation>().InfoSetting(slot.item.itemImage,
                                                                    slot.item.itemName,
                                                                    slot.item.attackDamage,
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
        Item item = null;

        if (equippedDrag.isDraggable)
        {
            item = equippedSlot.item;
            items.Remove(item);
        }

        for (i = 0; i < items.Count && i < inventorySlots.Length; i++)
        {
            inventorySlots[i].item = items[i];
            inventoryDrags[i].isDraggable = true;
        }
        for (; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].item = null;
            inventoryDrags[i].isDraggable = false;
        }

        if (item != null)
        {
            items.Add(item);
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

}