using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhotonLobbyManager : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            // 이미 생성된 방 목록 가져오기
            int roomCount = PhotonNetwork.CountOfRooms;

            // 이미 생성된 방이 하나 이상 존재하는지 확인
            if (roomCount > 0)
            {
                PhotonNetwork.JoinRoom("Lobby");
            }
            else
            {
                CreateRoom();
            }
        }
    }

    public void CreateRoom() 
    {
        //방 옵션
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 10;

        //방 목록에 보이게 할것인가?
        options.IsVisible = true;

        //방에 참여 가능 여부
        options.IsOpen = true;

        if (PhotonNetwork.IsConnectedAndReady)
        {
            //방 생성
            PhotonNetwork.CreateRoom("Lobby", options);
            PhotonNetwork.LoadLevel("LobbyScene");
        }
    }
}
