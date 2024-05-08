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
        if (player == null)
        {
            if (GameObject.FindGameObjectWithTag("Player"))
            {
                player = GameObject.FindGameObjectWithTag("Player");
            }
        }
        else
        {
            Vector3 desiredPosition;
            if(player.GetComponent<PlayerCtrl>().isMoveRoom)
            {
                desiredPosition = new Vector3(player.transform.position.x, player.transform.position.y, -10);
                transform.position = desiredPosition;
                player.GetComponent<PlayerCtrl>().isMoveRoom = false;
            }
            // if((player.transform.position.x > DungeonManager.playerRoomPos.x - cameraHalfWidth && player.transform.position.x < DungeonManager.playerRoomPos.x + cameraHalfWidth) &&
            // (player.transform.position.y > DungeonManager.playerRoomPos.y - cameraHalfHeight && player.transform.position.y < DungeonManager.playerRoomPos.y + cameraHalfHeight))
            // {
            //     desiredPosition = new Vector3(player.transform.position.x, player.transform.position.y, -10);
            //     transform.position = desiredPosition;
            // }
            // else
            // {
            //     if(player.transform.position.x < DungeonManager.playerRoomPos.x - cameraHalfWidth)
            //     {
            //         desiredPosition = new Vector3(DungeonManager.playerRoomPos.x - cameraHalfWidth, this.transform.position.y, -10);
            //         transform.position = desiredPosition;
            //     }
            //     else if(player.transform.position.x > DungeonManager.playerRoomPos.x + cameraHalfWidth)
            //     {
            //         desiredPosition = new Vector3(DungeonManager.playerRoomPos.x + cameraHalfWidth, this.transform.position.y, -10);
            //         transform.position = desiredPosition;
            //     }
            //     if(player.transform.position.y < DungeonManager.playerRoomPos.y - cameraHalfHeight)
            //     {
            //         desiredPosition = new Vector3(this.transform.position.x, DungeonManager.playerRoomPos.y - cameraHalfHeight, -10);
            //         transform.position = desiredPosition;
            //     }
            //     else if(player.transform.position.y > DungeonManager.playerRoomPos.y + cameraHalfHeight)
            //     {
            //         desiredPosition = new Vector3(this.transform.position.x, DungeonManager.playerRoomPos.y + cameraHalfHeight, -10);
            //         transform.position = desiredPosition;
            //     }
            // }
            if(player.transform.position.x > DungeonManager.playerRoomPos.x - cameraHalfWidth
            && player.transform.position.x < DungeonManager.playerRoomPos.x + cameraHalfWidth)
            {
                desiredPosition = new Vector3(player.transform.position.x, this.transform.position.y, -10);
                transform.position = desiredPosition;
            }
            else if(player.transform.position.x < DungeonManager.playerRoomPos.x - cameraHalfWidth
            || player.transform.position.x > DungeonManager.playerRoomPos.x + cameraHalfWidth)
            {
                if(player.transform.position.x < DungeonManager.playerRoomPos.x - cameraHalfWidth)
                {
                    desiredPosition = new Vector3(DungeonManager.playerRoomPos.x - cameraHalfWidth, this.transform.position.y, -10);
                    transform.position = desiredPosition;
                }
                else if(player.transform.position.x > DungeonManager.playerRoomPos.x + cameraHalfWidth)
                {
                    desiredPosition = new Vector3(DungeonManager.playerRoomPos.x + cameraHalfWidth, this.transform.position.y, -10);
                    transform.position = desiredPosition;
                }
            }

            if(player.transform.position.y > DungeonManager.playerRoomPos.y - cameraHalfHeight
            && player.transform.position.y < DungeonManager.playerRoomPos.y + cameraHalfHeight)
            {
                desiredPosition = new Vector3(this.transform.position.x, player.transform.position.y, -10);
                transform.position = desiredPosition;
            }
            else if(player.transform.position.y < DungeonManager.playerRoomPos.y - cameraHalfHeight
            || player.transform.position.y > DungeonManager.playerRoomPos.y + cameraHalfHeight)
            {
                if(player.transform.position.y < DungeonManager.playerRoomPos.y - cameraHalfHeight)
                {
                    desiredPosition = new Vector3(this.transform.position.x, DungeonManager.playerRoomPos.y - cameraHalfHeight, -10);
                    transform.position = desiredPosition;
                }
                else if(player.transform.position.y > DungeonManager.playerRoomPos.y + cameraHalfHeight)
                {
                    desiredPosition = new Vector3(this.transform.position.x, DungeonManager.playerRoomPos.y + cameraHalfHeight, -10);
                    transform.position = desiredPosition;
                }
            }
            // 카메라가 플레이어를 따라다님
        }
    }
}
