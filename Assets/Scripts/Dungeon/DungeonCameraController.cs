using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCameraController : MonoBehaviour
{
    private GameObject player;
    float cameraHalfWidth, cameraHalfHeight;
    // Start is called before the first frame update
    void Start()
    {
        cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
        cameraHalfHeight = Camera.main.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(cameraHalfWidth);
        Debug.Log(cameraHalfHeight);
        Debug.Log(DungeonManager.playerRoomPos);
        if (player == null)
        {
            if (GameObject.FindGameObjectWithTag("Player"))
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
        }
        else
        {
            if(player.transform.position.x > DungeonManager.playerRoomPos.x - cameraHalfWidth
            && player.transform.position.x < DungeonManager.playerRoomPos.x + cameraHalfWidth)
            {
                Vector3 desiredPosition = new Vector3(player.transform.position.x, this.transform.position.y, -10);
                transform.position = desiredPosition;
            }
            if(player.transform.position.y > DungeonManager.playerRoomPos.y - cameraHalfHeight
            && player.transform.position.y < DungeonManager.playerRoomPos.y + cameraHalfHeight)
            {
                Vector3 desiredPosition = new Vector3(this.transform.position.x, player.transform.position.y, -10);
                transform.position = desiredPosition;
            }
            // 카메라가 플레이어를 따라다님
        }
    }
}
