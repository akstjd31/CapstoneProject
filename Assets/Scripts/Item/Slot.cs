using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IDropHandler
{
    [SerializeField] Image Image;

    private Item Item;


    private void Start()
    {
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
                Image.color = new Color(0.8f, 0.8f, 0.8f, 1);
            }
        }
    }

    // �ش� ���Կ� ���𰡰� ���콺 ��� ���� �� �߻��ϴ� �̺�Ʈ
    public void OnDrop(PointerEventData eventData)
    {
        GameObject targetObj = eventData.pointerDrag;
        Item targetItem = targetObj.GetComponent<Slot>().item;

        Item tmp = targetItem;
        targetItem = item;
        item = tmp;

        Drag targetDrag = this.GetComponent<Drag>();
        Drag drag = targetObj.GetComponent<Drag>();

        targetDrag.isDraggable = true;

        targetDrag.defaultSize = targetDrag.isEquippedItem ? new Vector2(170, 170) : new Vector2(100, 100);

        targetDrag.defaultSprite = targetDrag.image.sprite;

        drag.defaultSprite = null;

        //Drag drag = dropObj.GetComponent<Drag>();
        //Drag targetDrag = this.GetComponent<Drag>();

        //if (drag != null)
        //{ 
        //    targetDrag.isDraggable = true;

        //    targetDrag.defaultSize = targetDrag.isEquippedItem ? new Vector2(170, 170) : new Vector2(100, 100);


        //    targetDrag.defaultSprite = drag.defaultSprite;
        //    targetDrag.image.sprite = targetDrag.defaultSprite;

        //    drag.defaultSprite = null;
        //}
    }
}