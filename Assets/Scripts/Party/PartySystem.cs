using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;


public class PartySystem : MonoBehaviourPunCallbacks
{
    public GameObject partyView;
    public GameObject partyCreator;
    public InputField inputField;
    public GameObject partyRoom;
    public GameObject content;

    public GameObject[] partyMemberHUD;

    [SerializeField] private List<Party> parties = new List<Party>();

    LobbyManager lobbyManager;

    PhotonView canvasPV;

    private int partyRoomID = 100;

    private void Start()
    {
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        canvasPV = GetComponent<PhotonView>();
    }

    public void OpenPartyViewer()
    {
        if (!partyCreator.activeSelf && !partyView.activeSelf)
        {
            partyView.SetActive(true);
        }
    }

    public void OpenPartyCreator()
    {
        if (!partyView.activeSelf && !partyCreator.activeSelf)
        {
            partyCreator.SetActive(true);
        }
    }

    public void CreatorOnExitButtonClick()
    {
        partyCreator.SetActive(false);
        inputField.text = "";
    }

    public void ViewerOnExitButtonClick()
    {
        partyView.SetActive(false);
    }

    // 파티에 적힌 ID로 찾는 함수
    private Party FindRoomByPartyID(int partyID)
    {
        for (var i = 0; i < parties.Count; i++)
        {
            if (partyID == parties[i].partyID)
            {
                return parties[i];
            }
        }
        return null;
    }
    public PlayerCtrl GetPlayerCtrlByNickname(string nickName)
    {
        foreach (int playerViewID in lobbyManager.lobbyPlayerViewID)
        {
            PhotonView targetPhotonView = PhotonView.Find(playerViewID);
            if (targetPhotonView.Controller.NickName.Equals(nickName))
            {
                return targetPhotonView.GetComponent<PlayerCtrl>();
            }
        }
        return null;
    }

    // 파티 생성 버튼을 눌렀을 때
    public void OnClickCompleteButton()
    {
        // 어떤 플레이어가 누른건지 확인하기 위한 foreach
        foreach (int playerViewID in lobbyManager.lobbyPlayerViewID)
        {
            PhotonView targetPhotonView = PhotonView.Find(playerViewID);

            // 현재 플레이어의 닉네임과 비교하여 찾음
            if (targetPhotonView.Owner.NickName.Equals(PhotonNetwork.NickName))
            {
                PlayerCtrl playerCtrl = targetPhotonView.GetComponent<PlayerCtrl>();
                
                // 만약 해당 플레이어가 어느 파티에 속하지 않을 때
                if (!playerCtrl.isPartyMember)
                {
                    // 방 생성
                    canvasPV.RPC("GetPartyRoom", RpcTarget.AllBuffered, inputField.text, targetPhotonView.ViewID);
                    Debug.Log(PhotonNetwork.NickName + " 파티가 생성되었습니다.");
                    partyCreator.SetActive(false); // 파티 만들기 창 비활성화

                   //partyMemberHUD[0].GetComponentInChildren<Text>().text = PhotonNetwork.NickName;

                    break;
                }
                else
                {
                    Debug.Log(PhotonNetwork.NickName + " 님은 이미 파티에 속해 있습니다!");
                }
            }
        }
    }

    // 파티 참가 버튼
    public void OnClickJoinPartyButton()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        Party party = clickObject.GetComponent<Party>();

        // 파티 인원이 꽉 찬 경우
        if (party.GetPartyHeadCount() == 2)
        {
            Debug.Log("파티 인원이 꽉 차 있습니다!");
        }
        else
        {
            PhotonView targetPhotonView = PhotonView.Find(party.GetPartyLeaderID());

            // 파티에 있는 1명의 인원이 본인일 경우
            if (PhotonNetwork.NickName.Equals(targetPhotonView.Controller.NickName))
            {
                Debug.Log(PhotonNetwork.NickName + " 님께서는 이미 해당 파티에 가입되어 있습니다!");
            }
            else
            {
                PlayerCtrl secondPlayerCtrl = GetPlayerCtrlByNickname(PhotonNetwork.NickName);

                if (secondPlayerCtrl != null)
                {
                    // 두 번째 플레이어의 파티 정보를 업데이트하기 위해 RPC 호출
                    canvasPV.RPC("JoinPartyRPC", RpcTarget.AllBuffered, party.partyID, secondPlayerCtrl.GetComponent<PhotonView>().ViewID);
                }
            }
        }
    }

    // 파티 참가 RPC
    [PunRPC]
    public void JoinPartyRPC(int partyID, int playerViewID)
    {
        int partyIdx = -1;

        for (var i = 0; i < parties.Count; i++)
        {
            if (parties[i].partyID == partyID)
            {
                partyIdx = i;
                break;
            }
        }

        PlayerCtrl playerCtrl = PhotonView.Find(playerViewID).GetComponent<PlayerCtrl>();

        // 파티 멤버가 아닌 경우
        if (!playerCtrl.isPartyMember)
        {
            playerCtrl.isPartyMember = true;
            playerCtrl.party = parties[partyIdx];
            playerCtrl.party.SetPartyMemberID(playerViewID);
        }
        else
        {
            Debug.Log(PhotonNetwork.NickName + " 님은 이미 파티에 속해 있습니다!");
        }
    }

    // 특정 파티에 속해 있는 플레이어가 종료 시 호출 (LobbyManager.cs)
    [PunRPC]
    public void HandlePlayerGameExit(string playerName)
    {
        PlayerCtrl playerCtrl = GetPlayerCtrlByNickname(playerName);

        if (playerCtrl)
        {
            PhotonView playerPV = playerCtrl.GetComponent<PhotonView>();

            // 파티에 가입되어 있는 경우
            if (playerCtrl.isPartyMember)
            {
                for (var i = 0; i < parties.Count; i++)
                {
                    if (playerCtrl.party.partyID == parties[i].partyID)
                    {
                        // 만약 파티장이 나간거면
                        if (parties[i].GetPartyLeaderID() == playerPV.ViewID)
                        {
                            // 파티원이 존재했던 파티면 해당 파티원을 파티장으로 변경한다.
                            if (parties[i].GetPartyHeadCount() == 2)
                            {
                                parties[i].SetPartyLeaderID(parties[i].GetPartyMemberID());
                                parties[i].SetPartyMemberID(-1);
                            }
                            else
                            {
                                parties[i].GetComponent<Button>().onClick.RemoveListener(OnClickJoinPartyButton);
                                Destroy(parties[i].gameObject);
                                parties.Remove(parties[i]);
                            }
                        }
                        else
                        {
                            parties[i].SetPartyMemberID(-1);
                        }

                        break;
                    }
                }
            }
            else
            {
                Debug.Log(playerPV.ViewID);
            }
        }
    }

    // 파티 탈퇴 버튼
    public void OnLeavePartyButtonClick()
    {
        PlayerCtrl playerCtrl = GetPlayerCtrlByNickname(PhotonNetwork.NickName);

        canvasPV.RPC("OnLeavePartyRPC", RpcTarget.AllBuffered, playerCtrl.GetComponent<PhotonView>().ViewID);
    }

    // 파티를 떠날 때 쓰이는 RPC
    [PunRPC]
    public void OnLeavePartyRPC(int viewID)
    {
        PhotonView targetViewID = PhotonView.Find(viewID);
        PlayerCtrl playerCtrl = targetViewID.GetComponent<PlayerCtrl>();

        if (playerCtrl.isPartyMember)
        {
            int partyIdx = -1;

            for (var i = 0; i < parties.Count; i++)
            {
                if (parties[i].partyID == playerCtrl.party.partyID)
                {
                    partyIdx = i;
                    break;
                }
            }

            if (partyIdx != -1)
            {
                // 파티에 해당되는 인원이 1명이면 파티를 없얜다.
                if (parties[partyIdx].GetPartyHeadCount() == 1)
                {
                    parties[partyIdx].GetComponent<Button>().onClick.RemoveListener(OnClickJoinPartyButton);
                    Destroy(parties[partyIdx].gameObject);
                    parties.Remove(parties[partyIdx]);
                }
                else
                {
                    // 2명이면 해당 플레이어가 파티원인지 파티장인지 확인한다.
                    if (parties[partyIdx].GetPartyLeaderID() == viewID)
                    {
                        parties[partyIdx].SetPartyLeaderID(parties[partyIdx].GetPartyMemberID());
                        parties[partyIdx].SetPartyMemberID(-1);
                    }
                }

                playerCtrl.isPartyMember = false;
                playerCtrl.party = null;
            }
        }
        else
        {
            Debug.Log("해당 플레이어는 파티에 가입되어있지 않습니다!");
        }
    }

    // 파티 방 생성
    [PunRPC]
    public void GetPartyRoom(string receiveMessage, int ViewID)
    {
        GameObject room = PhotonNetwork.Instantiate(partyRoom.name, Vector3.zero, Quaternion.identity);
        //GameObject room = Instantiate(partyRoom, Vector3.zero, Quaternion.identity, content.transform);
        room.transform.parent = content.transform;

        Party newParty = room.GetComponent<Party>();
        newParty.SetContext(receiveMessage);
        newParty.SetPartyLeaderID(ViewID);
        newParty.partyID = partyRoomID;
        partyRoomID++;
        parties.Add(room.GetComponent<Party>());
        parties[parties.Count - 1].GetComponent<Button>().onClick.AddListener(OnClickJoinPartyButton);

        PhotonView.Find(ViewID).GetComponent<PlayerCtrl>().party = parties[parties.Count - 1];
        PhotonView.Find(ViewID).GetComponent<PlayerCtrl>().isPartyMember = true;
    }
}
