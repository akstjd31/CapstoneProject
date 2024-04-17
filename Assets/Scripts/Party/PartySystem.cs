using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;


public class PartySystem : MonoBehaviourPunCallbacks
{
    public GameObject createPartyButton;    // 파티 생성 버튼
    public GameObject[] readyButton = new GameObject[2];    
    public GameObject partyView;    // 파티목록 보기
    public GameObject partyCreator; // 파티생성
    public InputField inputField;   // 파티내용
    public GameObject partyRoom;    // 파티방
    public GameObject content;  // 파티방이 생성되는 위치

    public GameObject[] partyMemberHUD; // 파티가입 시 적용되는 HP UI

    [SerializeField] private List<Party> parties = new List<Party>();   // 현재 생성된 파티 리스트

    LobbyManager lobbyManager;

    PhotonView canvasPV;

    private int partyRoomID = 100;  // 파티 방 ID를 부여하기 위한 시작 ID

    private const string PartiesKey = "Parties";

    private void Start()
    {
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        canvasPV = GetComponent<PhotonView>();

        //StartCoroutine(WaitForSecond());
    }

    // 하이에라키에 생성된 파티 방 찾기 (임시로 만들어 놂.)
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


    // 파티 목록 보기(버튼 이벤트 함수)
    public void OpenPartyViewer()
    {
        if (!partyCreator.activeSelf && !partyView.activeSelf)
        {
            partyView.SetActive(true);
        }
    }

    // 파티 생성 하기(버튼 이벤트 함수)
    public void OpenPartyCreator()
    {
        if (!partyView.activeSelf && !partyCreator.activeSelf)
        {
            partyCreator.SetActive(true);
        }
    }

    // 파티 생성 창 끄기
    public void CreatorOnExitButtonClick()
    {
        inputField.text = "";
        partyCreator.SetActive(false);
    }

    // 파티 목록 창 끄기
    public void ViewerOnExitButtonClick()
    {
        partyView.SetActive(false);
    }

    // 파티에 부여된 ID로 파티리스트에서 찾기.
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

    // 플레이어 닉네임으로 해당 플레이어 찾기.
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

    // 파티 생성 완료 버튼을 눌렀을 때
    public void OnClickCompleteButton()
    {
        // 현재 로비에 존재하는 플레이어들중에
        foreach (int playerViewID in lobbyManager.lobbyPlayerViewID)
        {
            PhotonView targetPhotonView = PhotonView.Find(playerViewID);

            // 로컬 닉네임과 비교하여
            if (targetPhotonView.Owner.NickName.Equals(PhotonNetwork.NickName))
            {
                PlayerCtrl playerCtrl = targetPhotonView.GetComponent<PlayerCtrl>();

                // 해당 플레이어가 파티에 가입이 되어있는지
                if (!playerCtrl.isPartyMember)
                {
                    GameObject room = PhotonNetwork.InstantiateRoomObject(partyRoom.name, Vector3.zero, Quaternion.identity);
                    this.GetComponent<PhotonView>().RPC("PartyRoomSetting", RpcTarget.AllBuffered, inputField.text, targetPhotonView.ViewID, room.GetComponent<PhotonView>().ViewID);
                    Debug.Log(PhotonNetwork.NickName + " 님이 파티를 생성하였습니다.");
                    partyCreator.SetActive(false);
                    playerCtrl.party.SetPartyLeaderID(playerCtrl.GetComponent<PhotonView>().ViewID);
                    createPartyButton.SetActive(false);
                    readyButton[0].SetActive(true);

                    partyMemberHUD[0].GetComponentInChildren<Text>().text = PhotonNetwork.NickName;

                    break;
                }
                else
                {
                    Debug.Log(PhotonNetwork.NickName + " 플레이어는 파티를 생성할 수 없습니다!");
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
            if (playerViewID == playerCtrl.party.GetPartyLeaderID())//
            {
                partyMemberHUD[0].transform.GetChild(2).gameObject.SetActive(false);
            }
            else if (playerViewID == playerCtrl.party.GetPartyMemberID())
            {
                partyMemberHUD[1].transform.GetChild(2).gameObject.SetActive(false);
            }
        }
        else
        {
            playerCtrl.isReady = true;
            if (playerViewID == playerCtrl.party.GetPartyLeaderID())
            {
                partyMemberHUD[0].transform.GetChild(2).gameObject.SetActive(true);
            }
            else if (playerViewID == playerCtrl.party.GetPartyMemberID())
            {
                partyMemberHUD[1].transform.GetChild(2).gameObject.SetActive(true);
            }
        }
    }


    // 파티 가입하기(버튼 이벤트 함수)
    public void OnClickJoinPartyButton()
    {
        // 현재 클릭한 UI에 대한 정보를 가져옴.
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        Party party = clickObject.GetComponent<Party>();

        // 만약 들어가려는 파티가 꽉 찼다면
        if (party.GetPartyHeadCount() == 2)
        {
            Debug.Log("해당 파티는 꽉 찼습니다!");
        }
        else
        {
            PhotonView targetPhotonView = PhotonView.Find(party.GetPartyLeaderID());

            // 해당 파티의 리더가 본인인지 확인
            if (PhotonNetwork.NickName.Equals(targetPhotonView.Controller.NickName))
            {
                Debug.Log(PhotonNetwork.NickName + "플레이어는 이미 파티에 가입되어 있습니다!");
            }
            else
            {
                PlayerCtrl secondPlayerCtrl = GetPlayerCtrlByNickname(PhotonNetwork.NickName);
                //PhotonView partyPV = party.GetComponent<PhotonView>();

                // 파티원으로 제대로 들어갔다면 
                if (secondPlayerCtrl != null)
                {
                    int partyMemberViewID = secondPlayerCtrl.GetComponent<PhotonView>().ViewID;
                    canvasPV.RPC("JoinPartyRPC", RpcTarget.AllBuffered, party.GetComponent<PhotonView>().ViewID, partyMemberViewID);
                    secondPlayerCtrl.party.SetPartyMemberID(partyMemberViewID);
                    createPartyButton.SetActive(false);
                    readyButton[0].SetActive(true);
                }
            }
        }
    }
    // 파티 가입 RPC
    [PunRPC]
    public void JoinPartyRPC(int partyID, int playerViewID)
    {
        PhotonView target = PhotonView.Find(partyID);
        Party party = target.GetComponent<Party>();

        
        //int partyIdx = -1;

        //// 가입하려는 파티의 ID로 파티리스트에서 찾아 인덱스를 얻어온다.
        //for (int i = 0; i < parties.Count; i++)
        //{
        //    if (parties[i].partyID == partyID)
        //    {
        //        partyIdx = i;
        //    }
        //}

        if (target != null)
        {
            PlayerCtrl playerCtrl = PhotonView.Find(playerViewID).GetComponent<PlayerCtrl>();

            // 가입을 원하는 플레이어가 특정 파티에 가입되어 있는지 확인
            if (!playerCtrl.isPartyMember)
            {

                playerCtrl.isPartyMember = true;
                playerCtrl.party = party;
                playerCtrl.party.SetPartyMemberID(playerViewID);
            }
        }
        else
        {
            Debug.Log("해당 파티를 찾을 수 없습니다!");
        }
    }

    // 파티에 가입된 상태로 게임을 나가버렸을 때 실행되는 RPC (LobbyManager.cs)
    [PunRPC]
    public void HandlePlayerGameExit(string playerName)
    {
        PlayerCtrl playerCtrl = GetPlayerCtrlByNickname(playerName);

        if (playerCtrl)
        {
            PhotonView playerPV = playerCtrl.GetComponent<PhotonView>();

            // 나간 플레이어가 파티에 속해 있었다면
            if (playerCtrl.isPartyMember)
            {
                PhotonView partyRoomPV = PhotonView.Find(playerCtrl.party.GetComponent<PhotonView>().ViewID);
                Party party = partyRoomPV.GetComponent<Party>();

                // 나간 플레이어가 리더였다면?
                if (party.GetPartyLeaderID() == playerPV.ViewID)
                {
                    // 그 파티가 인원이 꽉 찬 파티였다면?
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

    // 파티 탈퇴(버튼 이벤트 함수)
    public void OnLeavePartyButtonClick()
    {
        PlayerCtrl playerCtrl = GetPlayerCtrlByNickname(PhotonNetwork.NickName);

        createPartyButton.SetActive(true);
        readyButton[0].SetActive(false);
        readyButton[1].SetActive(false);
        canvasPV.RPC("OnLeavePartyRPC", RpcTarget.AllBuffered, playerCtrl.GetComponent<PhotonView>().ViewID);
    }

    // 파티 탈퇴 시 RPC
    [PunRPC]
    public void OnLeavePartyRPC(int viewID)
    {
        PhotonView targetViewID = PhotonView.Find(viewID);
        PlayerCtrl playerCtrl = targetViewID.GetComponent<PlayerCtrl>();

        // 탈퇴하려는 플레이어의 파티와 파티리스트 비교로 인덱스 얻기
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
                // 탈퇴하려는 플레이어가 있는 파티가 인원이 1명일때
                if (parties[partyIdx].GetPartyHeadCount() == 1)
                {
                    parties[partyIdx].GetComponent<Button>().onClick.RemoveListener(OnClickJoinPartyButton);
                    Destroy(parties[partyIdx].gameObject);
                    parties.Remove(parties[partyIdx]);
                }
                else
                {
                    // 꽉 찬 파티였다면
                    if (parties[partyIdx].GetPartyLeaderID() == viewID)
                    {
                        parties[partyIdx].SetPartyLeaderID(parties[partyIdx].GetPartyMemberID());
                        parties[partyIdx].SetPartyMemberID(-1);

                        PhotonView.Find(parties[partyIdx].GetPartyLeaderID()).GetComponent<PlayerCtrl>().party.GetComponent<PhotonView>().RequestOwnership();

                    }
                    else if (parties[partyIdx].GetPartyMemberID() == viewID)
                    {
                        parties[partyIdx].SetPartyMemberID(-1);
                    }
                }
                playerCtrl.isReady = false;
                playerCtrl.isPartyMember = false;
                playerCtrl.party = null;
            }
        }
        else
        {
            Debug.Log("플레이어는 파티에 가입되어 있지 않습니다!");
        }
    }

    //[PunRPC]
    //private void AddPartyRoomList(int partyRoomViewID)
    //{
    //    parties.Add(PhotonView.Find(partyRoomViewID).GetComponent<Party>());
    //}

    // 파티 생성 RPC
    [PunRPC]
    public void PartyRoomSetting(string receiveMessage, int playerViewID, int partyRoomViewID)
    {
        
        //GameObject room = Instantiate(partyRoom, Vector2.zero, Quaternion.identity, content.transform);

        PhotonView partyPV = PhotonView.Find(partyRoomViewID);
        partyPV.transform.parent = content.transform;

        Party newParty = partyPV.GetComponent<Party>();
        newParty.SetContext(receiveMessage);
        newParty.SetPartyLeaderID(playerViewID);
        newParty.partyID = partyRoomID;
        partyRoomID++;

        newParty.GetComponent<Button>().onClick.AddListener(OnClickJoinPartyButton);
        parties.Add(newParty);
        //parties[parties.Count - 1].GetComponent<Button>().onClick.AddListener(OnClickJoinPartyButton);

        PhotonView.Find(playerViewID).GetComponent<PlayerCtrl>().party = newParty;
        PhotonView.Find(playerViewID).GetComponent<PlayerCtrl>().isPartyMember = true;
        //canvasPV.RPC("AddPartyRoomList", RpcTarget.AllBuffered, partyRoomViewID);
    }
}