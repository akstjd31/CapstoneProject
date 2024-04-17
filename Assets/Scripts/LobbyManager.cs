using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public List<int> lobbyPlayerViewID;

    PhotonView canvasPV;
    PartySystem partySystemScript;
    void Start()
    {
        canvasPV = GameObject.FindGameObjectWithTag("Canvas").GetComponent<PhotonView>();
        partySystemScript = canvasPV.GetComponent<PartySystem>();
        if (PhotonNetwork.IsConnected)
        {
            // 이미 생성된 방 목록 가져오기
            int roomCount = PhotonNetwork.CountOfRooms;

            // 이미 생성된 방이 하나 이상 존재하는지 확인
            if (roomCount > 0)
            {
                // 첫 번째로 찾은 방에 들어가기
                PhotonNetwork.JoinRoom("Lobby");
            }
            else
            {
                CreateRoom();
            }
        }
    }

    private void Update()
    {
        AddPlayersWithPhotonViewToList();
    }

    // 하이에라키에 존재하는 모든 플레이어의 ViewID
    void AddPlayersWithPhotonViewToList()
    {
        PhotonView[] allViews = GameObject.FindObjectsOfType<PhotonView>();
        foreach (PhotonView view in allViews)
        {
            if (view.ViewID > 1000 && IsLastCharacterOne(view.ViewID) && !lobbyPlayerViewID.Contains(view.ViewID))
            {
                lobbyPlayerViewID.Add(view.ViewID);
            }
        }
    }

    private bool IsLastCharacterOne(int viewID)
    {
        string viewIDString = viewID.ToString();
        char lastCharacter = viewIDString[viewIDString.Length - 1];
        return lastCharacter == '1';
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
        // 새로운 플레이어
        canvasPV.RPC("GetMessage", RpcTarget.All, "Anonymous", newPlayer.NickName + " 님이 입장했습니다!");
    }

    [PunRPC]
    private void RemovePlayerViewID(int leftPlayerViewID)
    {
        lobbyPlayerViewID.Remove(leftPlayerViewID);
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
        //PhotonNetwork.Instantiate("Player", Vector2.zero, Quaternion.identity);
        PhotonNetwork.Instantiate("Unit000", Vector2.zero, Quaternion.identity);
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();
    }
    public void TransferMasterClient()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // 현재 방에 있는 모든 플레이어의 PhotonViewID를 가져옴
            List<int> viewIDs = new List<int>();
            foreach (Player player in PhotonNetwork.PlayerList)
            {
                viewIDs.Add(player.ActorNumber);
            }

            // 현재 마스터 클라이언트의 PhotonViewID를 가져옴
            int currentMasterClientViewID = PhotonNetwork.MasterClient.ActorNumber;

            // 다음으로 높은 ViewID를 가진 플레이어를 찾음
            int nextMasterClientViewID = GetNextHighestViewID(currentMasterClientViewID, viewIDs);

            // 해당 플레이어를 새로운 마스터 클라이언트로 설정
            Player newMasterClient = PhotonNetwork.CurrentRoom.GetPlayer(nextMasterClientViewID);
            PhotonNetwork.SetMasterClient(newMasterClient);
        }
    }

    // 다음으로 높은 ViewID를 가진 플레이어를 찾는 함수
    private int GetNextHighestViewID(int currentViewID, List<int> viewIDs)
    {
        int nextHighestViewID = int.MaxValue;
        foreach (int viewID in viewIDs)
        {
            if (viewID > currentViewID && viewID < nextHighestViewID)
            {
                nextHighestViewID = viewID;
            }
        }
        return nextHighestViewID;
    }

    // 마스터 클라이언트가 변경되었을 때 호출됩니다.
    //public override void OnMasterClientSwitched(Player newMasterClient)
    //{
    //    Debug.Log("New master client: " + newMasterClient.NickName);

    //    // 마스터 클라이언트가 변경되었을 때 오브젝트의 소유권을 변경합니다.
    //    TransferMasterClient();
    //}


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // 특정 플레이어가 게임을 종료하게 되면 챗에 남김.
        canvasPV.RPC("GetMessage", RpcTarget.All, "Anonymous", otherPlayer.NickName + " 님이 떠났습니다!");

        // 특정 파티에 속해있는 상태로 탈주 시 해당 파티에서 제거
        canvasPV.RPC("HandlePlayerGameExit", RpcTarget.AllBuffered, otherPlayer.NickName);

        // 로비에 있는 플레이어 목록에 제거
        //PlayerCtrl leftPlayer = partySystemScript.GetPlayerCtrlByNickname(otherPlayer.NickName);

        PhotonView leftPlayerPV = null;
        foreach (int playerViewID in lobbyPlayerViewID)
        {
            PhotonView targetPhotonView = PhotonView.Find(playerViewID);
            if (targetPhotonView.Controller.NickName.Equals(otherPlayer.NickName))
            {
                leftPlayerPV = targetPhotonView;
                break;
            }
        }

        this.GetComponent<PhotonView>().RPC("RemovePlayerViewID", RpcTarget.AllBuffered, leftPlayerPV.ViewID);
        //lobbyPlayerViewID.Remove(leftPlayerPV.ViewID);


        if (leftPlayerPV.IsMine)
        {
            PhotonNetwork.Destroy(leftPlayerPV.transform.parent.gameObject);
        }

        //TransferMasterClient();

        // 여기에 다른 플레이어가 방을 나갈 때 실행하고 싶은 로직을 추가
    }
}