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
        Drag drag = this.GetComponent<Drag>();

        //// 만약 쓰레기통에 넣는다면 해당 아이템 삭제
        //if (targetObj.CompareTag("Trash"))
        //{
        //    targetObj.GetComponent<Slot>().item = null;
        //    targetDrag.isDraggable = false;
        //    return;
        //}

        if (!drag.isDraggable)
        {
            Item targetItem = targetObj.GetComponent<Slot>().item;
            Drag targetDrag = targetObj.GetComponent<Drag>();

            if (!targetDrag.isEquippedItem)
            {
                Item tmp = targetItem;
                targetItem = item;
                item = tmp;

                drag.isDraggable = true;
                drag.defaultSize = drag.isEquippedItem ? new Vector2(350, 350) : new Vector2(200, 200);
                drag.defaultItem = targetDrag.defaultItem;
                drag.defaultSprite = drag.image.sprite;

                targetDrag.isDraggable = false;
            }
        }
        else
        {
            // 장착된 아이템 교체
            if (drag.isEquippedItem)
            {
                Drag targetDrag = targetObj.GetComponent<Drag>();

                Sprite tmpSprite = drag.defaultSprite;
                drag.defaultSprite = targetDrag.defaultSprite;
                targetDrag.defaultSprite = tmpSprite;

                this.transform.root.Find("Inventory").GetComponent<Inventory>().equippedItem = targetDrag.defaultItem;
                this.transform.root.Find("Inventory").GetComponent<Inventory>().items.Remove(targetDrag.defaultItem);

                Item tmpItem = targetDrag.defaultItem;
                targetDrag.defaultItem = drag.defaultItem;
                drag.defaultItem = tmpItem;

                this.transform.root.Find("Inventory").GetComponent<Inventory>().items.Add(targetDrag.defaultItem);

                item = drag.defaultItem;
            }
        }
    }
}