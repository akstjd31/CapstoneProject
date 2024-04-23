using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandleMap : MonoBehaviour, IPointerDownHandler, IDragHandler, IScrollHandler
{
    private RectTransform rectTransform;
    private Vector2 lastPointerPosition;

    private Vector2 minPosition;
    private Vector2 maxPosition;
    private Vector2 canvasSize;
    private Vector2 imageSize;

    private float minScale = 1.0f;
    private float maxScale = 2.0f;
    private float currentScale = 1.0f;

    private void Start()
    {
        canvasSize = new Vector2(1920, 1080);
        rectTransform = GetComponent<RectTransform>();
        CalculateMinMaxPosition();
    }

    private void CalculateMinMaxPosition()
    {
        imageSize = new Vector2(1920, 1080) * currentScale;

        //new Vector2(1920, 1080) * currentScale - new Vector2(1920, 1080);
        //== new Vector2(1920, 1080) * (currentScale - 1)

        minPosition = (imageSize - canvasSize) * -0.5f;
        maxPosition = (imageSize - canvasSize) * 0.5f;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        lastPointerPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        CalculateMinMaxPosition();

        Vector2 pointerDelta = eventData.position - lastPointerPosition;
        Vector2 newPosition = rectTransform.anchoredPosition + pointerDelta;

        Debug.Log($"pointerDelta : {pointerDelta}\tnewPosition : {newPosition}");
        Debug.Log($"minPosition : {minPosition}\nmaxPosition : {maxPosition}");
        // �̹����� ĵ�������� ���� ���, �巡�װ� �̵����� �ʵ��� �մϴ�.
        if (minPosition.x == 0 && minPosition.y == 0 && maxPosition.x == 0 && maxPosition.y == 0)
        {
            return;
        }

        // ���ο� ��ġ�� �巡�� ������ ����� �ʵ��� ����
        newPosition.x = Mathf.Clamp(newPosition.x, minPosition.x, maxPosition.x);
        newPosition.y = Mathf.Clamp(newPosition.y, minPosition.y, maxPosition.y);

        rectTransform.anchoredPosition = newPosition;
        lastPointerPosition = eventData.position;
    }

    public void OnScroll(PointerEventData eventData)
    {
        float scrollDelta = eventData.scrollDelta.y;
        currentScale = Mathf.Clamp(currentScale + scrollDelta * 0.05f, minScale, maxScale);
        rectTransform.localScale = new Vector3(currentScale, currentScale, 1.0f);
    }
}
