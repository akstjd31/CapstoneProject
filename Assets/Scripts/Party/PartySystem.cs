using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;


public class PartySystem : MonoBehaviourPunCallbacks
{
    public GameObject createPartyButton;    // ��Ƽ ���� ��ư
    public GameObject[] readyButton = new GameObject[2];    
    public GameObject partyView;    // ��Ƽ��� ����
    public GameObject partyCreator; // ��Ƽ����
    public InputField inputField;   // ��Ƽ����
    public GameObject partyRoom;    // ��Ƽ��
    public GameObject content;  // ��Ƽ���� �����Ǵ� ��ġ

    public GameObject[] partyMemberHUD; // ��Ƽ���� �� ����Ǵ� HP UI

    [SerializeField] private List<Party> parties = new List<Party>();   // ���� ������ ��Ƽ ����Ʈ

    LobbyManager lobbyManager;

    PhotonView canvasPV;

    private int partyRoomID = 100;  // ��Ƽ �� ID�� �ο��ϱ� ���� ���� ID

    private const string PartiesKey = "Parties";

    private void Start()
    {
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        canvasPV = GetComponent<PhotonView>();

        //StartCoroutine(WaitForSecond());
    }

    // ���̿���Ű�� ������ ��Ƽ �� ã�� (�ӽ÷� ����� ��.)
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


    // ��Ƽ ��� ����(��ư �̺�Ʈ �Լ�)
    public void OpenPartyViewer()
    {
        if (!partyCreator.activeSelf && !partyView.activeSelf)
        {
            partyView.SetActive(true);
        }
    }

    // ��Ƽ ���� �ϱ�(��ư �̺�Ʈ �Լ�)
    public void OpenPartyCreator()
    {
        if (!partyView.activeSelf && !partyCreator.activeSelf)
        {
            partyCreator.SetActive(true);
        }
    }

    // ��Ƽ ���� â ����
    public void CreatorOnExitButtonClick()
    {
        inputField.text = "";
        partyCreator.SetActive(false);
    }

    // ��Ƽ ��� â ����
    public void ViewerOnExitButtonClick()
    {
        partyView.SetActive(false);
    }

    // ��Ƽ�� �ο��� ID�� ��Ƽ����Ʈ���� ã��.
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

    // �÷��̾� �г������� �ش� �÷��̾� ã��.
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

    // ��Ƽ ���� �Ϸ� ��ư�� ������ ��
    public void OnClickCompleteButton()
    {
        // ���� �κ� �����ϴ� �÷��̾���߿�
        foreach (int playerViewID in lobbyManager.lobbyPlayerViewID)
        {
            PhotonView targetPhotonView = PhotonView.Find(playerViewID);

            // ���� �г��Ӱ� ���Ͽ�
            if (targetPhotonView.Owner.NickName.Equals(PhotonNetwork.NickName))
            {
                PlayerCtrl playerCtrl = targetPhotonView.GetComponent<PlayerCtrl>();

                // �ش� �÷��̾ ��Ƽ�� ������ �Ǿ��ִ���
                if (!playerCtrl.isPartyMember)
                {
                    GameObject room = PhotonNetwork.InstantiateRoomObject(partyRoom.name, Vector3.zero, Quaternion.identity);
                    this.GetComponent<PhotonView>().RPC("PartyRoomSetting", RpcTarget.AllBuffered, inputField.text, targetPhotonView.ViewID, room.GetComponent<PhotonView>().ViewID);
                    Debug.Log(PhotonNetwork.NickName + " ���� ��Ƽ�� �����Ͽ����ϴ�.");
                    partyCreator.SetActive(false);
                    playerCtrl.party.SetPartyLeaderID(playerCtrl.GetComponent<PhotonView>().ViewID);
                    createPartyButton.SetActive(false);
                    readyButton[0].SetActive(true);

                    partyMemberHUD[0].GetComponentInChildren<Text>().text = PhotonNetwork.NickName;

                    break;
                }
                else
                {
                    Debug.Log(PhotonNetwork.NickName + " �÷��̾�� ��Ƽ�� ������ �� �����ϴ�!");
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


    // ��Ƽ �����ϱ�(��ư �̺�Ʈ �Լ�)
    public void OnClickJoinPartyButton()
    {
        // ���� Ŭ���� UI�� ���� ������ ������.
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        Party party = clickObject.GetComponent<Party>();

        // ���� ������ ��Ƽ�� �� á�ٸ�
        if (party.GetPartyHeadCount() == 2)
        {
            Debug.Log("�ش� ��Ƽ�� �� á���ϴ�!");
        }
        else
        {
            PhotonView targetPhotonView = PhotonView.Find(party.GetPartyLeaderID());

            // �ش� ��Ƽ�� ������ �������� Ȯ��
            if (PhotonNetwork.NickName.Equals(targetPhotonView.Controller.NickName))
            {
                Debug.Log(PhotonNetwork.NickName + "�÷��̾�� �̹� ��Ƽ�� ���ԵǾ� �ֽ��ϴ�!");
            }
            else
            {
                PlayerCtrl secondPlayerCtrl = GetPlayerCtrlByNickname(PhotonNetwork.NickName);
                //PhotonView partyPV = party.GetComponent<PhotonView>();

                // ��Ƽ������ ����� ���ٸ� 
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
    // ��Ƽ ���� RPC
    [PunRPC]
    public void JoinPartyRPC(int partyID, int playerViewID)
    {
        PhotonView target = PhotonView.Find(partyID);
        Party party = target.GetComponent<Party>();

        
        //int partyIdx = -1;

        //// �����Ϸ��� ��Ƽ�� ID�� ��Ƽ����Ʈ���� ã�� �ε����� ���´�.
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

            // ������ ���ϴ� �÷��̾ Ư�� ��Ƽ�� ���ԵǾ� �ִ��� Ȯ��
            if (!playerCtrl.isPartyMember)
            {

                playerCtrl.isPartyMember = true;
                playerCtrl.party = party;
                playerCtrl.party.SetPartyMemberID(playerViewID);
            }
        }
        else
        {
            Debug.Log("�ش� ��Ƽ�� ã�� �� �����ϴ�!");
        }
    }

    // ��Ƽ�� ���Ե� ���·� ������ ���������� �� ����Ǵ� RPC (LobbyManager.cs)
    [PunRPC]
    public void HandlePlayerGameExit(string playerName)
    {
        PlayerCtrl playerCtrl = GetPlayerCtrlByNickname(playerName);

        if (playerCtrl)
        {
            PhotonView playerPV = playerCtrl.GetComponent<PhotonView>();

            // ���� �÷��̾ ��Ƽ�� ���� �־��ٸ�
            if (playerCtrl.isPartyMember)
            {
                PhotonView partyRoomPV = PhotonView.Find(playerCtrl.party.GetComponent<PhotonView>().ViewID);
                Party party = partyRoomPV.GetComponent<Party>();

                // ���� �÷��̾ �������ٸ�?
                if (party.GetPartyLeaderID() == playerPV.ViewID)
                {
                    // �� ��Ƽ�� �ο��� �� �� ��Ƽ���ٸ�?
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

    // ��Ƽ Ż��(��ư �̺�Ʈ �Լ�)
    public void OnLeavePartyButtonClick()
    {
        PlayerCtrl playerCtrl = GetPlayerCtrlByNickname(PhotonNetwork.NickName);

        createPartyButton.SetActive(true);
        readyButton[0].SetActive(false);
        readyButton[1].SetActive(false);
        canvasPV.RPC("OnLeavePartyRPC", RpcTarget.AllBuffered, playerCtrl.GetComponent<PhotonView>().ViewID);
    }

    // ��Ƽ Ż�� �� RPC
    [PunRPC]
    public void OnLeavePartyRPC(int viewID)
    {
        PhotonView targetViewID = PhotonView.Find(viewID);
        PlayerCtrl playerCtrl = targetViewID.GetComponent<PlayerCtrl>();

        // Ż���Ϸ��� �÷��̾��� ��Ƽ�� ��Ƽ����Ʈ �񱳷� �ε��� ���
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
                // Ż���Ϸ��� �÷��̾ �ִ� ��Ƽ�� �ο��� 1���϶�
                if (parties[partyIdx].GetPartyHeadCount() == 1)
                {
                    parties[partyIdx].GetComponent<Button>().onClick.RemoveListener(OnClickJoinPartyButton);
                    Destroy(parties[partyIdx].gameObject);
                    parties.Remove(parties[partyIdx]);
                }
                else
                {
                    // �� �� ��Ƽ���ٸ�
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
            Debug.Log("�÷��̾�� ��Ƽ�� ���ԵǾ� ���� �ʽ��ϴ�!");
        }
    }

    //[PunRPC]
    //private void AddPartyRoomList(int partyRoomViewID)
    //{
    //    parties.Add(PhotonView.Find(partyRoomViewID).GetComponent<Party>());
    //}

    // ��Ƽ ���� RPC
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