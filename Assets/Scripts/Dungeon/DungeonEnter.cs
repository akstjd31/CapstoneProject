using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class DungeonEnter : MonoBehaviourPunCallbacks
{
    // 전환할 씬의 이름
    public string sceneName = "DungeonScene";
    int[] partyPlayersID = new int[2];

    PlayerCtrl playerCtrl;
    string roomName;
    PhotonView dungeonEntrancePV;
    
    // Start is called before the first frame update
    void Start()
    {
        dungeonEntrancePV = GetComponent<PhotonView>();
        // 포톤 서버에 연결
        //PhotonNetwork.ConnectUsingSettings();
        //메모용
        //던전 입장
        //파티 참여 상태일시 파티생성 버튼 준비 버튼으로 교체 o
        //준비 완료 버튼 누를시 준비 상태 동기화 및 준비 취소 버튼으로 교체 o
        //두 파티원 모두 준비 완료일 경우에 파티장이 던전 입장 가능 
        //파티원 아이디를 가져와 두명만 씬 전환 
        //두 플레이어 캐릭터 던전에 생성

        //던전
        //던전 방 밖으로 카메라 시야가 비추지 않도록 설정
        //맵 다시만들기
        //플레이하는 맵의 문, 벽 배치

        //몬스터
        //공격, 어그로 구현
        //원거리 근거리 몬스터 구현

        //무기
        //
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Enter Trigger");
        this.transform.localScale = new Vector3(this.transform.localScale.x + 0.1f, this.transform.localScale.y + 0.1f, this.transform.localScale.z + 0.1f);
        playerCtrl = other.GetComponent<PlayerCtrl>();
        if(playerCtrl.isPartyMember)
        {
            if(playerCtrl.GetComponent<PhotonView>().ViewID == playerCtrl.party.GetPartyLeaderID() && playerCtrl.party.GetPartyHeadCount() == 2 
            && playerCtrl.party.partyMembers[0].GetComponent<PhotonView>().ViewID == playerCtrl.GetComponent<PhotonView>().ViewID)
            {
                partyPlayersID[0] = playerCtrl.party.GetPartyLeaderID();
                partyPlayersID[1] = playerCtrl.party.GetPartyMemberID();
                roomName = PhotonView.Find(partyPlayersID[0]).Controller.NickName;
                this.transform.localScale = new Vector3(this.transform.localScale.x - 0.2f, this.transform.localScale.y - 0.2f, this.transform.localScale.z - 0.2f);
                foreach (int playerID in partyPlayersID)
                {
                    PhotonView playerView = PhotonView.Find(playerID);
                    playerView.RPC("OnEnterDungeon", RpcTarget.AllBuffered, partyPlayersID, roomName);
                }
            }

            //for test
            // partyPlayersID[0] = playerCtrl.party.GetPartyLeaderID();
            // PhotonView playerView = PhotonView.Find(partyPlayersID[0]);
            // playerView.RPC("OnEnterDungeon", RpcTarget.AllBuffered, partyPlayersID[0], roomName);
        }
    }
    void CreateRoom()
    {
        // 방 생성
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 2 });
        // foreach (int playerID in partyPlayerIDs)
        // {
        //     PhotonView playerView = PhotonView.Find(playerID);
        //     if (playerView != null)
        //     {
        //         playerView.RPC("JoinRoom", RpcTarget.AllBuffered, roomName);
        //     }
        // }

        //for test
        PhotonView playerView = PhotonView.Find(partyPlayersID[0]);
        playerView.RPC("JoinRoom_", RpcTarget.AllBuffered, roomName);
        PhotonNetwork.LoadLevel(sceneName);
        Debug.Log("방 생성 성공");
    }

    // public override void OnCreatedRoom()
    // {
    //     base.OnCreatedRoom();

    // }

    [PunRPC]
    public void JoinRoom_(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        Debug.Log("방 입장 성공");
        PhotonNetwork.Instantiate("Unit000", Vector2.zero, Quaternion.identity);
    }

    // public override void OnJoinedRoom()
    // {
    //     base.OnJoinedRoom();

    //     Debug.Log("방 입장 성공");
    //     PhotonNetwork.Instantiate("Unit000", Vector2.zero, Quaternion.identity);
    // }
}