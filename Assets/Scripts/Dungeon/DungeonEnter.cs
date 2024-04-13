using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class DungeonEnter : MonoBehaviourPunCallbacks
{
    // 전환할 씬의 이름
    public string nextSceneName = "DungeonScene";
    int[] partyUserID = new int[2];
    
    // Start is called before the first frame update
    void Start()
    {
        //메모용
        //던전 입장
        //파티 참여 상태일시 파티생성 버튼 준비 버튼으로 교체
        //준비 완료 버튼 누를시 준비 상태 동기화 및 준비 취소 버튼으로 교체
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
        PlayerCtrl playerCtrl = other.GetComponent<PlayerCtrl>();
        if(playerCtrl.isPartyMember)
        {
            this.transform.localScale = new Vector3(this.transform.localScale.x - 0.1f, this.transform.localScale.y - 0.1f, this.transform.localScale.z - 0.1f);
            if(playerCtrl.GetComponent<PhotonView>().ViewID == playerCtrl.party.GetPartyLeaderID() && playerCtrl.party.GetPartyHeadCount() == 2 
            && playerCtrl.party.partyMembers[0].GetComponent<PhotonView>().ViewID == playerCtrl.GetComponent<PhotonView>().ViewID)
            {
                partyUserID[0] = playerCtrl.party.GetPartyLeaderID();
                partyUserID[1] = playerCtrl.party.GetPartyMemberID();
                //SceneManager.LoadScene("DungeonScene");
                RequestSceneSwitch(partyUserID);
            }
        }
    }

    // 특정 플레이어들에게만 씬 전환을 요청하는 RPC 메서드
    [PunRPC]
    void RequestSceneSwitch(int[] playerIDs)
    {
        foreach (int playerID in playerIDs)
        {
            // 각 플레이어에게 씬 전환 요청을 보냄
            PhotonView playerView = PhotonView.Find(playerID);
            playerView.RPC("SwitchScene", RpcTarget.AllBuffered, nextSceneName);
        }
    }

    // 씬 전환 메서드
    [PunRPC]
    void SwitchScene(string sceneName)
    {
        // 포톤 네트워크를 통해 씬 전환을 수행
        PhotonNetwork.LoadLevel(sceneName);
    }
}