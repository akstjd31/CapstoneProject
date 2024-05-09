using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 아이템 자체에 달려있는 스크립트
public class Items : MonoBehaviour
{
    public Item item;

    private Vector3 startPos;
    private Vector3 endPos;
    public float lerpTime = 1.0f;

    private Vector3 targetVelocity = Vector3.zero;

    private void Start()
    {
        startPos = transform.position;
        endPos = new Vector2(startPos.x, startPos.y + 0.5f);
    }

    private void Update()
    {
        // 부드럽게 이동
        transform.position = Vector3.SmoothDamp(transform.position, endPos, ref targetVelocity, lerpTime);

        // 목표 지점에 도달하면 방향 변경
        if (Vector3.Distance(transform.position, endPos) < 0.05f)
        {
            Vector3 tmp = startPos;
            startPos = endPos;
            endPos = tmp;
        }
    }
}
