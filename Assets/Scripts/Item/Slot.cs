using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;

public class Slot : MonoBehaviour, IDropHandler
{
    [SerializeField] Image Image;

    private Item Item;


    private void Start()
    {
    }

    private void Update()
    {
        if (item == null && this.GetComponent<Drag>().isDraggable)
        {
            this.GetComponent<Drag>().isDraggable = false;
        }
    }

    public Item item
    {
        get { return Item; }
        set
        {
            Item = value;
            if (Item != null)
            {
                Image.sprite = item.itemImage;
                Image.color = new Color(1, 1, 1, 1);
            }
            else
            {
                Image.sprite = null;
                Image.color = new Color(0, 0, 0, 0);
            }
        }
    }

    // 아이템 드랍
    public void OnDrop(PointerEventData eventData)
    {
        GameObject targetObj = eventData.pointerDrag;
        Drag targetDrag = this.GetComponent<Drag>();

        //// 만약 쓰레기통에 넣는다면 해당 아이템 삭제
        //if (targetObj.CompareTag("Trash"))
        //{
        //    targetObj.GetComponent<Slot>().item = null;
        //    targetDrag.isDraggable = false;
        //    return;
        //}

        if (!targetDrag.isDraggable)
        {
            Item targetItem = targetObj.GetComponent<Slot>().item;
            Drag drag = targetObj.GetComponent<Drag>();

            if (!drag.isEquippedItem)
            {
                Item tmp = targetItem;
                targetItem = item;
                item = tmp;

                targetDrag.isDraggable = true;

                targetDrag.defaultSize = targetDrag.isEquippedItem ? new Vector2(350, 350) : new Vector2(200, 200);

                targetDrag.defaultSprite = targetDrag.image.sprite;

                drag.isDraggable = false;
            }
        }
        else
        {
            // 장착된 아이템이 존재해도 덮어씌우기
            if (targetDrag.isEquippedItem)
            {
                Item targetItem = targetObj.GetComponent<Slot>().item;
                Drag drag = targetObj.GetComponent<Drag>();

                Inventory inventory = this.transform.root.Find("Inventory").GetComponent<Inventory>();
                inventory.items.Remove(this.GetComponent<Slot>().item);

                item = targetItem;
                targetItem = null;

                targetDrag.defaultSize = new Vector2(350, 350);

                targetDrag.defaultSprite = targetDrag.image.sprite;

                drag.isDraggable = false;

            }
        }
    }
}