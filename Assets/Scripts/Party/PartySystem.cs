using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;


public class PartySystem : MonoBehaviourPunCallbacks
{
    public GameObject createPartyButton;
    public GameObject[] readyButton = new GameObject[2];
    public GameObject partyView;
    public GameObject partyCreator;
    public InputField inputField;
    public GameObject partyRoom;
    public GameObject content;

    public GameObject[] partyMemberHUD; // 파티 멤버 HUD
    public Button leavingParty; // 파티

    [SerializeField] private List<Party> parties = new List<Party>();   // 파티 리스트 

    LobbyManager lobbyManager;

    PhotonView canvasPV;


    private const string PartiesKey = "Parties";

    private void Start()
    {
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        canvasPV = GetComponent<PhotonView>();

        //StartCoroutine(WaitForSecond());
    }

    public void FindPartyRoom()
    {
        if (GameObject.FindGameObjectWithTag("PartyRoom") && GameObject.FindGameObjectWithTag("PartyRoom").transform.parent != content.transform)
        {
            GameObject[] party = GameObject.FindGameObjectsWithTag("PartyRoom");
            for (int i = 0; i < party.Length; i++)
            {
                parties.Add(party[i].GetComponent<Party>());
                party[i].transform.parent = content.transform;
                party[i].GetComponent<Button>().onClick.AddListener(OnClickJoinPartyButton);
            }
        }
    }

    IEnumerator WaitForSecond()
    {
        yield return new WaitForSeconds(2f);
        FindPartyRoom();
    }

    private void Update()
    {
        FindPartyRoom();
    }


    public void OpenPartyViewer()
    {
        if (!partyCreator.activeSelf && !partyView.activeSelf)
        {
            partyView.SetActive(true);
        }
    }

    // 파티 생성 UI
    public void OpenPartyCreator()
    {
        if (!partyView.activeSelf && !partyCreator.activeSelf)
        {
            partyCreator.SetActive(true);
        }
    }

    // 파티 생성 X 버튼
    public void CreatorOnExitButtonClick()
    {
        inputField.text = "";
        partyCreator.SetActive(false);
    }

    // 파티 목록 UI X 버튼
    public void ViewerOnExitButtonClick()
    {
        partyView.SetActive(false);
    }

    // 파티 ViewID로 Party.cs 찾기
    private int FindRoomByPartyID(int partyID)
    {
        for (var i = 0; i < parties.Count; i++)
        {
            if (partyID == parties[i].GetComponent<PhotonView>().ViewID)
            {
                return i;
            }
        }
        return -1;
    }

    // 닉네임으로 PlayerCtrl.cs 찾기
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

    // 파티 생성 완료 버튼을 클릭했을때
    public void OnClickCompleteButton()
    {
        // 로비에 있는 플레이어 ViewID
        foreach (int playerViewID in lobbyManager.lobbyPlayerViewID)
        {
            PhotonView targetPhotonView = PhotonView.Find(playerViewID);

            // 본인 PhotonView를 발견하면
            if (targetPhotonView.Owner.NickName.Equals(PhotonNetwork.NickName))
            {
                PlayerCtrl playerCtrl = targetPhotonView.GetComponent<PlayerCtrl>();
                
                // 본인이 파티에 속해있지 않으면
                if (!playerCtrl.isPartyMember)
                {
                    // 방을 생성한다.
                    GameObject room = PhotonNetwork.InstantiateRoomObject(partyRoom.name, Vector3.zero, Quaternion.identity);

                    Debug.Log("Room View ID: " + room.GetComponent<PhotonView>().ViewID);
                    Debug.Log("view ID: " + targetPhotonView.ViewID);
                    canvasPV.RPC("PartyRoomSetting", RpcTarget.AllBuffered, inputField.text, targetPhotonView.ViewID, room.GetComponent<PhotonView>().ViewID);
                    Debug.Log(PhotonNetwork.NickName + " 가 방을 생성하였습니다.");
                    partyCreator.SetActive(false);
                    playerCtrl.party.SetPartyLeaderID(playerCtrl.GetComponent<PhotonView>().ViewID);
                    readyButton[0].SetActive(true);
                    createPartyButton.SetActive(false);
                    partyMemberHUD[0].GetComponentInChildren<Text>().text = PhotonNetwork.NickName;
                    break;
                }
                else
                {
                    Debug.Log(PhotonNetwork.NickName + " 해당 플레이어는 이미 파티에 속해 있습니다!");
                }
            }
        }
    }

    public void Ready()
    {
        PlayerCtrl playerCtrl = GetPlayerCtrlByNickname(PhotonNetwork.NickName);
        canvasPV.RPC("ReadyRPC", RpcTarget.AllBuffered, playerCtrl.GetComponent<PhotonView>().ViewID);

        readyButton[0].SetActive(!playerCtrl.isReady);
        readyButton[1].SetActive(playerCtrl.isReady);
    }

    [PunRPC]
    public void ReadyRPC(int playerViewID)
    {
        PlayerCtrl playerCtrl = PhotonView.Find(playerViewID).GetComponent<PlayerCtrl>();
        if (playerCtrl.isReady)
        {
            playerCtrl.isReady = false;
            if (playerViewID == playerCtrl.party.GetPartyLeaderID())
            {
                partyMemberHUD[0].transform.Find("Ready").gameObject.SetActive(false);
            }
            else if (playerViewID == playerCtrl.party.GetPartyMemberID())
            {
                partyMemberHUD[1].transform.Find("Ready").gameObject.SetActive(false);
            }
        }
        else
        {
            playerCtrl.isReady = true;
            if (playerViewID == playerCtrl.party.GetPartyLeaderID())
            {
                partyMemberHUD[0].transform.Find("Ready").gameObject.SetActive(true);
            }
            else if (playerViewID == playerCtrl.party.GetPartyMemberID())
            {
                partyMemberHUD[1].transform.Find("Ready").gameObject.SetActive(true);
            }
        }
    }


    // 파티 가입 버튼
    public void OnClickJoinPartyButton()
    {
        // 클릭한 파티 방의 정보를 가져옴.
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        Party party = clickObject.transform.parent.GetComponent<Party>();

        // 파티에 인원이 꽉차면
        if (party.GetPartyHeadCount() == 2)
        {
            Debug.Log("해당 파티에 인원이 꽉 차있습니다!");
        }
        else
        {
            PhotonView targetPhotonView = PhotonView.Find(party.GetPartyLeaderID());

            // 현재 플레이어가 이미 해당 파티에 속해 있는 경우
            if (PhotonNetwork.NickName.Equals(targetPhotonView.Controller.NickName))
            {
                Debug.Log(PhotonNetwork.NickName + "님은 이미 해당 파티에 속해 있습니다!");
            }
            else
            {
                PlayerCtrl secondPlayerCtrl = GetPlayerCtrlByNickname(PhotonNetwork.NickName);
                int partyMemberViewID = secondPlayerCtrl.GetComponent<PhotonView>().ViewID;
                canvasPV.RPC("JoinPartyRPC", RpcTarget.AllBuffered, party.GetComponent<PhotonView>().ViewID, partyMemberViewID);
                secondPlayerCtrl.party.SetPartyMemberID(partyMemberViewID);
                createPartyButton.SetActive(false);
                readyButton[0].SetActive(true);
            }
        }
    }

    // 파티 가입 RPC
    [PunRPC]
    public void JoinPartyRPC(int partyID, int playerViewID)
    {
        PhotonView targetPartyID = PhotonView.Find(partyID);
        Party party = targetPartyID.GetComponent<Party>();

        if (targetPartyID != null)
        {
            PlayerCtrl playerCtrl = PhotonView.Find(playerViewID).GetComponent<PlayerCtrl>();

            // 해당 플레이어가 파티에 가입되어 있지 않은 상태에서
            if (!playerCtrl.isPartyMember)
            {
                playerCtrl.isPartyMember = true;
                playerCtrl.party = party;
                playerCtrl.party.SetPartyMemberID(playerViewID);
            }
        }
        else
        {
            Debug.Log(PhotonNetwork.NickName + "님은 이미 파티에 가입되어 있습니다!");
        }
    }

    // 플레이어가 게임을 종료했을 때 (LobbyManager.cs)
    [PunRPC]
    public void HandlePlayerGameExit(string playerName)
    {
        PlayerCtrl playerCtrl = GetPlayerCtrlByNickname(playerName);

        if (playerCtrl)
        {
            PhotonView playerPV = playerCtrl.GetComponent<PhotonView>();

            // 나간 플레이어가 파티원이었다면
            if (playerCtrl.isPartyMember)
            {
                PhotonView partyRoomPV = PhotonView.Find(playerCtrl.party.GetComponent<PhotonView>().ViewID);
                Party party = partyRoomPV.GetComponent<Party>();

                // 만약 리더였을 때
                if (party.GetPartyLeaderID() == playerPV.ViewID)
                {
                    // 꽉 차있는 파티였다면
                    if (party.GetPartyHeadCount() == 2)
                    {
                        int tmp = party.GetPartyMemberID();
                        party.SetPartyLeaderID(tmp);
                        party.SetPartyMemberID(-1);
                        partyRoomPV.RequestOwnership();
                    }
                    else
                    {
                        PhotonNetwork.Destroy(party.gameObject);
                    }
                }
                else if (party.GetPartyMemberID() == playerPV.ViewID)
                {
                    party.SetPartyMemberID(-1);
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

        createPartyButton.SetActive(true);
        readyButton[0].SetActive(false);
        readyButton[1].SetActive(false);

        if (playerCtrl.isPartyMember)
        {
            canvasPV.RPC("OnLeavePartyRPC", RpcTarget.AllBuffered, playerCtrl.GetComponent<PhotonView>().ViewID);
        }
        else
        {
            Debug.Log(PhotonNetwork.NickName + "님은 파티에 속해있지 않습니다!");
        }
    }

    // 파티 탈퇴 RPC
    [PunRPC]
    public void OnLeavePartyRPC(int viewID)
    {
        PhotonView targetViewID = PhotonView.Find(viewID);
        PlayerCtrl playerCtrl = targetViewID.GetComponent<PlayerCtrl>();
        Party party = playerCtrl.party;

        int partyListIdx = FindRoomByPartyID(party.GetComponent<PhotonView>().ViewID);

        if (partyListIdx != -1)
        {
            // 1인 파티였다면
            if (party.GetPartyHeadCount() == 1)
            {
                PhotonNetwork.Destroy(party.gameObject);
                parties.Remove(parties[partyListIdx]);
            }
            else
            {
                // 파티 리더였다면 현 멤버를 리더로 교체
                if (parties[partyListIdx].GetPartyLeaderID() == viewID)
                {
                    parties[partyListIdx].SetPartyLeaderID(parties[partyListIdx].GetPartyMemberID());
                    parties[partyListIdx].SetPartyMemberID(-1);

                    PhotonView.Find(parties[partyListIdx].GetPartyLeaderID()).GetComponent<PlayerCtrl>().party.GetComponent<PhotonView>().RequestOwnership();

                }

                // 멤버였다면 멤버를 -1로
                else if (parties[partyListIdx].GetPartyMemberID() == viewID)
                {
                    parties[partyListIdx].SetPartyMemberID(-1);
                }
            }

            playerCtrl.isReady = false;
            playerCtrl.isPartyMember = false;
            playerCtrl.party = null;
        }
    }

        //[PunRPC]
        //private void AddPartyRoomList(int partyRoomViewID)
        //{
        //    parties.Add(PhotonView.Find(partyRoomViewID).GetComponent<Party>());
        //}

    // 파티 방 세팅 RPC
    [PunRPC]
    public void PartyRoomSetting(string receiveMessage, int playerViewID, int partyRoomViewID)
    {
        
        //GameObject room = Instantiate(partyRoom, Vector2.zero, Quaternion.identity, content.transform);

        PhotonView partyPV = PhotonView.Find(partyRoomViewID);
        partyPV.transform.parent = content.transform;

        Party newParty = partyPV.GetComponent<Party>();
        newParty.SetContext(receiveMessage);
        newParty.SetPartyLeaderID(playerViewID);
        newParty.SetLeaderName(PhotonNetwork.NickName);

        newParty.GetComponentInChildren<Button>().onClick.AddListener(OnClickJoinPartyButton);
        parties.Add(newParty);
        //parties[parties.Count - 1].GetComponent<Button>().onClick.AddListener(OnClickJoinPartyButton);

        PhotonView.Find(playerViewID).GetComponent<PlayerCtrl>().party = newParty;
        PhotonView.Find(playerViewID).GetComponent<PlayerCtrl>().isPartyMember = true;
        //canvasPV.RPC("AddPartyRoomList", RpcTarget.AllBuffered, partyRoomViewID);
    }
}