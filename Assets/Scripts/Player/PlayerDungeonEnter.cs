using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PlayerDungeonEnter : MonoBehaviourPunCallbacks
{
    PhotonView pv;
    string sceneName = "DungeonScene";

    // Start is called before the first frame update
    void Start()
    {
        pv = this.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // [PunRPC]
    // public IEnumerator OnEnterDungeon(int[] partyPlayersID, string roomName)
    // {
    //     foreach (int playerID in partyPlayersID)
    //     {
    //         // PhotonView PartyPV = PhotonView.Find(partyPlayersID);
    //         PhotonView PartyPV = PhotonView.Find(playerID);
    //         if (PartyPV.IsMine)
    //         {
    //             //SceneManager.LoadScene(sceneName);
    //             if (partyPlayersID[0] == PartyPV.ViewID)
    //             {
    //                 PhotonNetwork.LeaveRoom();
    //                 yield return new WaitForSeconds(3.0f);
    //                 CreateRoom(roomName);
    //                 yield return new WaitForSeconds(3.0f);
    //                 JoinRoom(roomName);
    //             }
    //             else
    //             {
    //                 PhotonNetwork.LeaveRoom();
    //                 yield return new WaitForSeconds(7.0f);
    //                 JoinRoom(roomName);
    //             }
    //             yield break;
    //         }
    //     }
    // }

    //for test
    [PunRPC]
    public IEnumerator OnEnterDungeon(int partyPlayersID, string roomName)
    {
        PhotonView PartyPV = PhotonView.Find(partyPlayersID);
        if (PartyPV.IsMine)
        {
            PhotonNetwork.LeaveRoom();
            yield return new WaitForSeconds(5.0f);
            PhotonNetwork.JoinLobby();
            yield return new WaitForSeconds(5.0f);
            CreateRoom(roomName);
            yield return new WaitForSeconds(5.0f);
            JoinRoom(roomName);
        }
    }

    void CreateRoom(string roomName)
    {
        // 방 생성
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 2 });
        PhotonNetwork.LoadLevel(sceneName);
        //PhotonNetwork.Instantiate("Unit000", Vector2.zero, Quaternion.identity);
        // foreach (int playerID in partyPlayerIDs)
        // {
        //     PhotonView playerView = PhotonView.Find(playerID);
        //     if (playerView != null)
        //     {
        //         playerView.RPC("JoinRoom", RpcTarget.AllBuffered, roomName);
        //     }
        // }

        //for test
        Debug.Log("방 생성 성공");
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        //PhotonNetwork.Instantiate("Unit000", Vector2.zero, Quaternion.identity);
        Debug.Log("방 입장 성공");
    }
}
