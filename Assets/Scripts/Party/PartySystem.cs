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

    public GameObject[] partyMemberHUD;

    [SerializeField] private List<Party> parties = new List<Party>();

    LobbyManager lobbyManager;

    PhotonView canvasPV;

    private int partyRoomID = 100;

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

    public void OpenPartyCreator()
    {
        if (!partyView.activeSelf && !partyCreator.activeSelf)
        {
            partyCreator.SetActive(true);
        }
    }

    public void CreatorOnExitButtonClick()
    {
        inputField.text = "";
        partyCreator.SetActive(false);
    }

    public void ViewerOnExitButtonClick()
    {
        partyView.SetActive(false);
    }

    // ????? ???? ID?? ??? ???
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

    // ??? ???? ????? ?????? ??
    public void OnClickCompleteButton()
    {
        // ?? ?????? ???????? ?????? ???? foreach
        foreach (int playerViewID in lobbyManager.lobbyPlayerViewID)
        {
            PhotonView targetPhotonView = PhotonView.Find(playerViewID);

            // ???? ???????? ?????? ????? ???
            if (targetPhotonView.Owner.NickName.Equals(PhotonNetwork.NickName))
            {
                PlayerCtrl playerCtrl = targetPhotonView.GetComponent<PlayerCtrl>();

                // ???? ??? ?????? ??? ????? ?????? ???? ??
                if (!playerCtrl.isPartyMember)
                {

                    canvasPV.RPC("PartyRoomSetting", RpcTarget.AllBuffered, inputField.text, targetPhotonView.ViewID);
                    Debug.Log(PhotonNetwork.NickName + " ????? ????????????.");
                    partyCreator.SetActive(false); // ??? ????? ? ??????
                    playerCtrl.party.partyMembers[0] = playerCtrl;
                    createPartyButton.SetActive(false);
                    readyButton[0].SetActive(true);

                    partyMemberHUD[0].GetComponentInChildren<Text>().text = PhotonNetwork.NickName;

                    break;
                }
                else
                {
                    Debug.Log(PhotonNetwork.NickName + " ???? ??? ????? ???? ??????!");
                }
            }
        }
    }

    public void Ready()//???? ??? ?
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


    // ??? ???? ???
    public void OnClickJoinPartyButton()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        Party party = clickObject.GetComponent<Party>();

        // ??? ????? ?? ?? ???
        if (party.GetPartyHeadCount() == 2)
        {
            Debug.Log("??? ????? ?? ?? ??????!");
        }
        else
        {
            PhotonView targetPhotonView = PhotonView.Find(party.GetPartyLeaderID());

            // ????? ??? 1???? ????? ?????? ???
            if (PhotonNetwork.NickName.Equals(targetPhotonView.Controller.NickName))
            {
                Debug.Log(PhotonNetwork.NickName + " ??????? ??? ??? ????? ?????? ??????!");
            }
            else
            {
                PlayerCtrl secondPlayerCtrl = GetPlayerCtrlByNickname(PhotonNetwork.NickName);
                //PhotonView partyPV = party.GetComponent<PhotonView>();

                if (secondPlayerCtrl != null)
                {
                    // ?? ??? ???????? ??? ?????? ?????????? ???? RPC ???
                    canvasPV.RPC("JoinPartyRPC", RpcTarget.AllBuffered, party.partyID, secondPlayerCtrl.GetComponent<PhotonView>().ViewID);
                    secondPlayerCtrl.party.partyMembers[1] = secondPlayerCtrl;
                    createPartyButton.SetActive(false);
                    readyButton[0].SetActive(true);
                }
            }
        }
    }
    // ??? ???? RPC
    [PunRPC]
    public void JoinPartyRPC(int partyID, int playerViewID)
    {
        int partyIdx = -1;

        for (int i = 0; i < parties.Count; i++)
        {
            if (parties[i].partyID == partyID)
            {
                partyIdx = i;
            }
        }

        PlayerCtrl playerCtrl = PhotonView.Find(playerViewID).GetComponent<PlayerCtrl>();

        // ??? ????? ??? ???
        if (!playerCtrl.isPartyMember)
        {
            if (partyIdx != -1)
            {
                playerCtrl.isPartyMember = true;
                playerCtrl.party = parties[partyIdx];
                playerCtrl.party.SetPartyMemberID(playerViewID);
            }
            else
            {
                Debug.Log("????? ??? ?? ???????!");
            }
        }
        else
        {
            Debug.Log(PhotonNetwork.NickName + " ???? ??? ????? ???? ??????!");
        }
    }

    // ??? ????? ???? ??? ?????? ???? ?? ??? (LobbyManager.cs)
    [PunRPC]
    public void HandlePlayerGameExit(string playerName)
    {
        PlayerCtrl playerCtrl = GetPlayerCtrlByNickname(playerName);

        if (playerCtrl)
        {
            PhotonView playerPV = playerCtrl.GetComponent<PhotonView>();

            // ????? ?????? ??? ???
            if (playerCtrl.isPartyMember)
            {
                for (var i = 0; i < parties.Count; i++)
                {
                    if (playerCtrl.party.partyID == parties[i].partyID)
                    {
                        // ???? ??????? ???????
                        if (parties[i].GetPartyLeaderID() == playerPV.ViewID)
                        {
                            // ??????? ??????? ????? ??? ??????? ????????? ???????.
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

    // ??? ??? ???
    public void OnLeavePartyButtonClick()
    {
        PlayerCtrl playerCtrl = GetPlayerCtrlByNickname(PhotonNetwork.NickName);

        createPartyButton.SetActive(true);
        readyButton[0].SetActive(false);
        readyButton[1].SetActive(false);
        canvasPV.RPC("OnLeavePartyRPC", RpcTarget.AllBuffered, playerCtrl.GetComponent<PhotonView>().ViewID);
    }

    // ????? ???? ?? ????? RPC
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
                // ????? ????? ????? 1????? ????? ?????.
                if (parties[partyIdx].GetPartyHeadCount() == 1)
                {
                    playerCtrl.party.partyMembers[0] = null;
                    parties[partyIdx].GetComponent<Button>().onClick.RemoveListener(OnClickJoinPartyButton);
                    Destroy(parties[partyIdx].gameObject);
                    parties.Remove(parties[partyIdx]);
                }
                else
                {
                    // 2????? ??? ?????? ????????? ????????? ??????.
                    if (parties[partyIdx].GetPartyLeaderID() == viewID)
                    {
                        parties[partyIdx].SetPartyLeaderID(parties[partyIdx].GetPartyMemberID());
                        parties[partyIdx].SetPartyMemberID(-1);
                        playerCtrl.party.partyMembers[0] = playerCtrl.party.partyMembers[1];
                        playerCtrl.party.partyMembers[0] = null;
                    }
                    else if (parties[partyIdx].GetPartyMemberID() == viewID)
                    {
                        parties[partyIdx].SetPartyMemberID(-1);
                        playerCtrl.party.partyMembers[1] = null;
                    }
                }
                playerCtrl.isReady = false;
                playerCtrl.isPartyMember = false;
                playerCtrl.party = null;
            }
        }
        else
        {
            Debug.Log("??? ??????? ????? ?????????? ??????!");
        }
    }

    //[PunRPC]
    //private void AddPartyRoomList(int partyRoomViewID)
    //{
    //    parties.Add(PhotonView.Find(partyRoomViewID).GetComponent<Party>());
    //}

    // ??? ?? ????
    [PunRPC]
    public void PartyRoomSetting(string receiveMessage, int playerViewID)
    {
        // ?? ????
        GameObject room = Instantiate(partyRoom, Vector3.zero, Quaternion.identity, content.transform);
        room.transform.parent = content.transform;

        //PhotonView partyPV = PhotonView.Find(partyRoomViewID);
        Party newParty = room.GetComponent<Party>();
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