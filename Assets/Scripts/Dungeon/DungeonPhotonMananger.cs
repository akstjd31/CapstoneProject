using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPhotonMananger : MonoBehaviourPunCallbacks
{
    public List<int> lobbyPlayerViewID;

    PhotonView canvasPV;
    PartySystem partySystemScript;

    private string[] charTypeList = new string[] { "Warrior", "Archer" };
    void Start()
    {
        //if (GameObject.Find("PhotonManager").GetComponent<PhotonManager>().GetCharType() != "")
        //{
        //    charType = GameObject.Find("PhotonManager").GetComponent<PhotonManager>().GetCharType();
        //}
    }

    private void Update()
    {
        //AddPlayersWithPhotonViewToList();
    }

    // 하이에라키에 존재하는 모든 플레이어의 ViewID
    void AddPlayersWithPhotonViewToList()
    {
        PhotonView[] allViews = GameObject.FindObjectsOfType<PhotonView>();
        foreach (PhotonView view in allViews)
        {
            if (view.ViewID > 1000 && !lobbyPlayerViewID.Contains(view.ViewID))
            {
                lobbyPlayerViewID.Add(view.ViewID);
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
        }
    }

    // 새 플레이어가 접속했을 때 호출되는 함수
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("새로운 플레이어가 입장했습니다: " + newPlayer.NickName);
        // 새로운 플레이어에게 환영 메시지 표시 등의 처리


    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log("방 생성 실패" + message);
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        Debug.Log("방 생성 성공");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("방 입장 성공");

        string charType = GameObject.FindGameObjectWithTag("PhotonManager").GetComponent<PhotonManager>().GetCharType();
        if (charType != "")
        {
            if (charType.Equals(charTypeList[0]))
            {
                PhotonNetwork.Instantiate(charTypeList[0], Vector2.zero, Quaternion.identity);
            }
            else if (charType.Equals(charTypeList[1]))
            {

                PhotonNetwork.Instantiate(charTypeList[1], Vector2.zero, Quaternion.identity);
            }
        }
        
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 특정 플레이어가 게임을 종료하게 되면 챗에 남김.
        //canvasPV.RPC("GetMessage", RpcTarget.AllBuffered, otherPlayer.NickName + " 님이 떠났습니다!");
    
        // 특정 파티에 속해있는 상태로 탈주 시 해당 파티에서 제거
        canvasPV.RPC("HandlePlayerGameExit", RpcTarget.AllBuffered, otherPlayer.NickName);

        Debug.Log(otherPlayer.NickName + " 님이 떠났습니다!");

        // 로비에 있는 플레이어 목록에 제거
        PlayerCtrl leftPlayer = partySystemScript.GetPlayerCtrlByNickname(otherPlayer.NickName);

        lobbyPlayerViewID.Remove(leftPlayer.GetComponent<PhotonView>().ViewID);
        PhotonNetwork.Destroy(leftPlayer.gameObject);

        // 여기에 다른 플레이어가 방을 나갈 때 실행하고 싶은 로직을 추가
    }
}