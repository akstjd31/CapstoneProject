using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    // 복원할 위치
    public Vector2 defaultPos;

    // 기존 부모 저장
    public Transform defaultParent;
    public Sprite defaultSprite;
    public Color defaultColor;

    public Vector2 defaultSize;

    Inventory inventory;

    public Image image;

    public bool isEquippedItem;

    //private float inventorySlotSize

    public bool isDraggable;

    void Start()
    {
        inventory = this.transform.parent.parent.parent.parent.GetComponent<Inventory>();

        defaultPos = Vector2.zero;
        defaultColor = new Color(0.8f, 0.8f, 0.8f, 1);
        defaultParent = this.transform.parent;
        defaultSprite = this.GetComponent<Image>().sprite;
        defaultSize = this.GetComponent<RectTransform>().sizeDelta;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 특정 아이템을 집는 순간에 백업할 정보 저장
        if (isDraggable)
        {
            // parent.parent.parent = Bag
            this.transform.SetParent(transform.root);
            this.transform.SetAsLastSibling();

            image.raycastTarget = false;
        }
        else
        {
            Debug.Log("해당 자리에 아이템이 존재하지 않습니다!");
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDraggable)
        {
            Vector2 currentPos = eventData.position;
            this.transform.position = currentPos;

            this.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDraggable)
        { 
            this.transform.SetParent(defaultParent);
            this.GetComponent<RectTransform>().anchoredPosition = defaultPos;

            //Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            this.GetComponent<RectTransform>().sizeDelta = defaultSize;
            this.GetComponent<Image>().sprite = defaultSprite;
            this.GetComponent<Image>().color = defaultColor;

            image.raycastTarget = true;

            isDraggable = defaultSprite == null ? false : true;
        }
    }
}