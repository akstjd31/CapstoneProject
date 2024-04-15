using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class PlayerEnterDungeon : MonoBehaviourPunCallbacks
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

    [PunRPC]
    public IEnumerator OnEnterDungeon(bool partyLeader, int[] partyPlayersID, string roomName)
    {
        foreach (int playerID in partyPlayersID)
        {
            //PhotonView PartyPV = PhotonView.Find(partyPlayersID);
            PhotonView PartyPV = PhotonView.Find(playerID);
            if (PartyPV.IsMine)
            {
                SceneManager.LoadScene(sceneName);
                // PhotonNetwork.LeaveRoom();
                // yield return new WaitForSeconds(5.0f);
                // PhotonNetwork.JoinLobby();
                // yield return new WaitForSeconds(5.0f);
                // if (partyLeader)
                // {
                //     CreateRoom(roomName);
                //     yield return new WaitForSeconds(5.0f);
                //     JoinRoom(roomName);
                // }
                // else
                // {
                //     JoinRoom(roomName);
                // }
                yield break;
            }
        }

    }
    // public IEnumerator OnEnterDungeon_(bool partyLeader)
    // {
    //         PhotonNetwork.LeaveRoom();
    //         yield return new WaitForSeconds(5.0f);
    //         if (partyLeader)
    //         {
    //             CreateRoom();
    //         }
    //         else
    //         {
    //             JoinRoom(roomName);
    //         }
    //         yield break;
    // }

    void CreateRoom(string roomName)
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
        Debug.Log("방 생성 성공");
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
        PhotonNetwork.Instantiate("Unit000", Vector2.zero, Quaternion.identity);
        Debug.Log("방 입장 성공");
        PhotonNetwork.LoadLevel(sceneName);
    }
}
