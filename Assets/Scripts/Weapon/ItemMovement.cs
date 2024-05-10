using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMovement : MonoBehaviour
{
    public float startY = 0f;   // 시작 Y 위치
    public float endY = 0.5f;     // 끝 Y 위치
    public float speed = 0.3f;    // 이동 속도

    void Update()
    {
        Movement();
    }

    void Movement()
    {
        // 스무스스텝 계산 후 Lerp 함수를 사용하여 새로운 Y 위치 계산
        float t = Mathf.SmoothStep(0f, 1f, Mathf.PingPong(Time.time * speed, 1f));
        float newY = Mathf.Lerp(startY, endY, t);

        // 현재 위치 갱신
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
