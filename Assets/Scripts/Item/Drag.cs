using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // 옮길 정보들
    public Vector2 defaultPos;
    public Transform defaultParent;
    public Sprite defaultSprite;
    public Color defaultColor;
    public Item defaultItem;

    public Vector2 defaultSize;

    public Image image;

    public bool isEquippedItem;

    private PlayerCtrl playerCtrl;

    //private float inventorySlotSize

    public bool isDraggable;

    void Start()
    {
        image = this.GetComponent<Image>();

        defaultPos = Vector2.zero;
        defaultColor = new Color(0, 0, 0, 0);
        defaultParent = this.transform.parent;
        defaultSprite = this.GetComponent<Image>().sprite;
        defaultSize = this.GetComponent<RectTransform>().sizeDelta;
        defaultItem = this.GetComponent<Slot>().item;
    }

    void Update()
    {
        if (isEquippedItem && defaultSprite != null)
        {
            this.GetComponent<Image>().sprite = defaultSprite;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 해당 슬롯에 아이템이 존재한다면
        if (isDraggable && !isEquippedItem)
        {
            // parent.parent.parent = Bag
            this.transform.SetParent(transform.root);
            this.transform.SetAsLastSibling();

            image.raycastTarget = false;
        }
        else
        {
            Debug.Log("슬롯에 아이템이 없거나 뺄 수 없습니다!");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDraggable && !isEquippedItem)
        {
            Vector2 currentPos = eventData.position;
            this.transform.position = currentPos;

            this.GetComponent<RectTransform>().sizeDelta = new Vector2(250, 250);
        }
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDraggable)
        {
            this.transform.SetParent(defaultParent);
            this.GetComponent<RectTransform>().anchoredPosition = defaultPos;

            //Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            defaultColor = new Color(1, 1, 1, 1);
        }
        else
        {
            this.transform.SetParent(defaultParent);
            this.GetComponent<RectTransform>().anchoredPosition = defaultPos;

            defaultColor = new Color(0, 0, 0, 0);

            defaultSprite = null;
            defaultItem = null;
        }

        this.GetComponent<RectTransform>().sizeDelta = defaultSize;
        this.GetComponent<Image>().sprite = defaultSprite;
        this.GetComponent<Image>().color = defaultColor;
        this.GetComponent<Slot>().item = defaultItem;
        image.raycastTarget = true;
    }
}