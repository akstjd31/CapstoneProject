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

    public GameObject[] partyMemberHUD; // ï¿½ï¿½Æ¼ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½Ç´ï¿?HP UI

    [SerializeField] private List<Party> parties = new List<Party>();   // ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½Æ¼ ï¿½ï¿½ï¿½ï¿½Æ®

    LobbyManager lobbyManager;

    PhotonView canvasPV;

    private int partyRoomID = 100;  // ï¿½ï¿½Æ¼ ï¿½ï¿½ IDï¿½ï¿½ ï¿½Î¿ï¿½ï¿½Ï±ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ID

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

    // ï¿½ï¿½Æ¼ ï¿½ï¿½ï¿½ï¿½ ï¿½Ï±ï¿½(ï¿½ï¿½Æ° ï¿½Ìºï¿½Æ® ï¿½Ô¼ï¿½)
    public void OpenPartyCreator()
    {
        if (!partyView.activeSelf && !partyCreator.activeSelf)
        {
            partyCreator.SetActive(true);
        }
    }

    // ï¿½ï¿½Æ¼ ï¿½ï¿½ï¿½ï¿½ Ã¢ ï¿½ï¿½ï¿½ï¿½
    public void CreatorOnExitButtonClick()
    {
        inputField.text = "";
        partyCreator.SetActive(false);
    }

    // ï¿½ï¿½Æ¼ ï¿½ï¿½ï¿?Ã¢ ï¿½ï¿½ï¿½ï¿½
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

    // ï¿½Ã·ï¿½ï¿½Ì¾ï¿½ ï¿½Ð³ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½Ø´ï¿½ ï¿½Ã·ï¿½ï¿½Ì¾ï¿½ Ã£ï¿½ï¿½.
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

    // ï¿½ï¿½Æ¼ ï¿½ï¿½ï¿½ï¿½ ï¿½Ï·ï¿½ ï¿½ï¿½Æ°ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½
    public void OnClickCompleteButton()
    {
        // ï¿½ï¿½ï¿½ï¿½ ï¿½Îºï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½Ï´ï¿½ ï¿½Ã·ï¿½ï¿½Ì¾ï¿½ï¿½ï¿½ß¿ï¿?
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
                    // ¸¶½ºÅÍ Å¬¶óÀÌ¾ðÆ®¿¡ ¿µÇâÀ» ¹ÞÁö ¾Ê´Â ¿ÀºêÁ§Æ® »ý¼º¹ý
                    GameObject room = PhotonNetwork.InstantiateRoomObject(partyRoom.name, Vector3.zero, Quaternion.identity);

                    this.GetComponent<PhotonView>().RPC("PartyRoomSetting", RpcTarget.AllBuffered, inputField.text, targetPhotonView.ViewID, room.GetComponent<PhotonView>().ViewID);
                    Debug.Log(PhotonNetwork.NickName + " ´ÔÀÌ ÆÄÆ¼¸¦ »ý¼ºÇÏ¿´½À´Ï´Ù.");
                    partyCreator.SetActive(false);
                    playerCtrl.party.SetPartyLeaderID(playerCtrl.GetComponent<PhotonView>().ViewID);
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


    // ï¿½ï¿½Æ¼ ï¿½ï¿½ï¿½ï¿½ï¿½Ï±ï¿½(ï¿½ï¿½Æ° ï¿½Ìºï¿½Æ® ï¿½Ô¼ï¿½)
    public void OnClickJoinPartyButton()
    {
        // ï¿½ï¿½ï¿½ï¿½ Å¬ï¿½ï¿½ï¿½ï¿½ UIï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½.
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

                // ï¿½ï¿½Æ¼ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿?ï¿½ï¿½î°¬ï¿½Ù¸ï¿?
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

    // ÆÄÆ¼ °¡ÀÔ RPC
    [PunRPC]
    public void JoinPartyRPC(int partyID, int playerViewID)
    {
        PhotonView target = PhotonView.Find(partyID);
        Party party = target.GetComponent<Party>();

        //int partyIdx = -1;

        //// °¡ÀÔÇÏ·Á´Â ÆÄÆ¼ÀÇ ID·Î ÆÄÆ¼¸®½ºÆ®¿¡¼­ Ã£¾Æ ÀÎµ¦½º¸¦ ¾ò¾î¿Â´Ù.
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

            // °¡ÀÔÀ» ¿øÇÏ´Â ÇÃ·¹ÀÌ¾î°¡ Æ¯Á¤ ÆÄÆ¼¿¡ °¡ÀÔµÇ¾î ÀÖ´ÂÁö È®ÀÎ
            if (!playerCtrl.isPartyMember)
            {

                playerCtrl.isPartyMember = true;
                playerCtrl.party = party;
                playerCtrl.party.SetPartyMemberID(playerViewID);
            }
        }
        else
        {
            Debug.Log("ÇØ´ç ÆÄÆ¼¸¦ Ã£À» ¼ö ¾ø½À´Ï´Ù!");
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
                PhotonView partyRoomPV = PhotonView.Find(playerCtrl.party.GetComponent<PhotonView>().ViewID);
                Party party = partyRoomPV.GetComponent<Party>();

                // ³ª°£ ÇÃ·¹ÀÌ¾î°¡ ¸®´õ¿´´Ù¸é?
                if (party.GetPartyLeaderID() == playerPV.ViewID)
                {
                    // ±× ÆÄÆ¼°¡ ÀÎ¿øÀÌ ²Ë Âù ÆÄÆ¼¿´´Ù¸é?
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

    // ï¿½ï¿½Æ¼ Å»ï¿½ï¿½(ï¿½ï¿½Æ° ï¿½Ìºï¿½Æ® ï¿½Ô¼ï¿½)
    public void OnLeavePartyButtonClick()
    {
        PlayerCtrl playerCtrl = GetPlayerCtrlByNickname(PhotonNetwork.NickName);

        createPartyButton.SetActive(true);
        readyButton[0].SetActive(false);
        readyButton[1].SetActive(false);
        canvasPV.RPC("OnLeavePartyRPC", RpcTarget.AllBuffered, playerCtrl.GetComponent<PhotonView>().ViewID);
    }

    // ï¿½ï¿½Æ¼ Å»ï¿½ï¿½ ï¿½ï¿½ RPC
    [PunRPC]
    public void OnLeavePartyRPC(int viewID)
    {
        PhotonView targetViewID = PhotonView.Find(viewID);
        PlayerCtrl playerCtrl = targetViewID.GetComponent<PlayerCtrl>();

        // Å»ï¿½ï¿½ï¿½Ï·ï¿½ï¿½ï¿½ ï¿½Ã·ï¿½ï¿½Ì¾ï¿½ï¿½ï¿½ ï¿½ï¿½Æ¼ï¿½ï¿½ ï¿½ï¿½Æ¼ï¿½ï¿½ï¿½ï¿½Æ® ï¿½ñ±³·ï¿½ ï¿½Îµï¿½ï¿½ï¿½ ï¿½ï¿½ï¿?
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
                // Å»ï¿½ï¿½ï¿½Ï·ï¿½ï¿½ï¿½ ï¿½Ã·ï¿½ï¿½Ì¾î°¡ ï¿½Ö´ï¿½ ï¿½ï¿½Æ¼ï¿½ï¿½ ï¿½Î¿ï¿½ï¿½ï¿½ 1ï¿½ï¿½ï¿½Ï¶ï¿½
                if (parties[partyIdx].GetPartyHeadCount() == 1)
                {
                    parties[partyIdx].GetComponent<Button>().onClick.RemoveListener(OnClickJoinPartyButton);
                    Destroy(parties[partyIdx].gameObject);
                    parties.Remove(parties[partyIdx]);
                }
                else
                {
                    // ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½Æ¼ï¿½ï¿½ï¿½Ù¸ï¿½
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
            Debug.Log("ï¿½Ã·ï¿½ï¿½Ì¾ï¿½ï¿?ï¿½ï¿½Æ¼ï¿½ï¿½ ï¿½ï¿½ï¿½ÔµÇ¾ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½Ê½ï¿½ï¿½Ï´ï¿½!");
        }
    }

    //[PunRPC]
    //private void AddPartyRoomList(int partyRoomViewID)
    //{
    //    parties.Add(PhotonView.Find(partyRoomViewID).GetComponent<Party>());
    //}

    // ï¿½ï¿½Æ¼ ï¿½ï¿½ï¿½ï¿½ RPC
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