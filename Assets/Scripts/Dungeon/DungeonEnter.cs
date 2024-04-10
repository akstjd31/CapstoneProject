using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class DungeonEnter : MonoBehaviourPunCallbacks
{
    // 전환할 씬의 이름
    public string nextSceneName = "DungeonScene";
    int[] partyUserID = new int[2];
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D()
    {
        partyUserID[0] = GetComponent<Party>().GetPartyLeaderID();
        partyUserID[1] = GetComponent<Party>().GetPartyMemberID();
        //SceneManager.LoadScene("DungeonScene");
        RequestSceneSwitch(partyUserID);
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
