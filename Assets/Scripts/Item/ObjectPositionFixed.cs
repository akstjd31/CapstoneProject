using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPositionFixed : MonoBehaviour
{
    // 원하는 위치를 선언합니다.
    public Vector3 fixedPosition;

    void Update()
    {
        // 오브젝트의 위치를 고정하려는 위치로 설정합니다.
        transform.position = fixedPosition;
    }
}
