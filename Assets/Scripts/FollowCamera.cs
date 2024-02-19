using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    //public float smoothSpeed = 5f;  // 이동 속도 조절
    private GameObject player;
    float cameraHalfWidth, cameraHalfHeight;
    void Start()
    {
        player = null;

        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        cameraHalfHeight = Camera.main.orthographicSize;
    }

    void Update()
    {
        if (player == null)
        {
            if (GameObject.FindGameObjectWithTag("Player"))
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
        }
        else
        {
            // 카메라가 플레이어를 따라다님
            Vector3 desiredPosition = new Vector3(
            player.transform.position.x,
            player.transform.position.y,
            -10);                                                                                               
            transform.position = desiredPosition; 
        }
    }
}
