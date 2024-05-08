using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        equippedSlot = equippedSlotParent.GetComponentInChildren<Slot>();
        equippedDrag = equippedSlotParent.GetComponentInChildren<Drag>();

        inventorySlots = intventorySlotParent.GetComponentsInChildren<Slot>();
        inventoryDrags = intventorySlotParent.GetComponentsInChildren<Drag>();

        this.transform.SetAsLastSibling();
    }
#endif

    void Awake()
    {
        FreshSlot();
    }

    public void OnClickCloseButton()
    {
        this.gameObject.SetActive(false);
    }

    public void FreshSlot()
    {
        int i = 0;

        // 착용 슬롯
        //if (items.Count > 0)
        //{
        //    equippedSlot.item = items[0];
        //    equippedDrag.isDraggable = true;
        //    i++;
        //}
        //else
        //{
        //    equippedSlot.item = null;
        //    equippedDrag.isDraggable = false;
        //}

        for (; i < items.Count && i < inventorySlots.Length; i++)
        {
            inventorySlots[i].item = items[i];
            inventoryDrags[i].isDraggable = true;
        }
        for (; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].item = null;
            inventoryDrags[i].isDraggable = false;
        }
    }

    public void AddItem(Item item)
    {
        if (items.Count < inventorySlots.Length)
        {
            items.Add(item);
            FreshSlot();
        }
        else
        {
            Debug.Log("아이템 공간이 없습니다.");
        }
    }

    //public void ChangeSlot(Slot slot1, Slot slot2)
    //{

    //}
}