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

    // ��Ƽ�� ���� ID�� ã�� �Լ�
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

    // ��Ƽ ���� ��ư�� ������ ��
    public void OnClickCompleteButton()
    {
        // � �÷��̾ �������� Ȯ���ϱ� ���� foreach
        foreach (int playerViewID in lobbyManager.lobbyPlayerViewID)
        {
            PhotonView targetPhotonView = PhotonView.Find(playerViewID);

            // ���� �÷��̾��� �г��Ӱ� ���Ͽ� ã��
            if (targetPhotonView.Owner.NickName.Equals(PhotonNetwork.NickName))
            {
                PlayerCtrl playerCtrl = targetPhotonView.GetComponent<PlayerCtrl>();
                
                // ���� �ش� �÷��̾ ��� ��Ƽ�� ������ ���� ��
                if (!playerCtrl.isPartyMember)
                {
                    // �� ����
                    canvasPV.RPC("GetPartyRoom", RpcTarget.AllBuffered, inputField.text, targetPhotonView.ViewID);
                    Debug.Log(PhotonNetwork.NickName + " ��Ƽ�� �����Ǿ����ϴ�.");
                    partyCreator.SetActive(false); // ��Ƽ ����� â ��Ȱ��ȭ

                   //partyMemberHUD[0].GetComponentInChildren<Text>().text = PhotonNetwork.NickName;

                    break;
                }
                else
                {
                    Debug.Log(PhotonNetwork.NickName + " ���� �̹� ��Ƽ�� ���� �ֽ��ϴ�!");
                }
            }
        }
    }

    // ��Ƽ ���� ��ư
    public void OnClickJoinPartyButton()
    {
        GameObject clickObject = EventSystem.current.currentSelectedGameObject;
        Party party = clickObject.GetComponent<Party>();

        // ��Ƽ �ο��� �� �� ���
        if (party.GetPartyHeadCount() == 2)
        {
            Debug.Log("��Ƽ �ο��� �� �� �ֽ��ϴ�!");
        }
        else
        {
            PhotonView targetPhotonView = PhotonView.Find(party.GetPartyLeaderID());

            // ��Ƽ�� �ִ� 1���� �ο��� ������ ���
            if (PhotonNetwork.NickName.Equals(targetPhotonView.Controller.NickName))
            {
                Debug.Log(PhotonNetwork.NickName + " �Բ����� �̹� �ش� ��Ƽ�� ���ԵǾ� �ֽ��ϴ�!");
            }
            else
            {
                PlayerCtrl secondPlayerCtrl = GetPlayerCtrlByNickname(PhotonNetwork.NickName);

                if (secondPlayerCtrl != null)
                {
                    // �� ��° �÷��̾��� ��Ƽ ������ ������Ʈ�ϱ� ���� RPC ȣ��
                    canvasPV.RPC("JoinPartyRPC", RpcTarget.AllBuffered, party.partyID, secondPlayerCtrl.GetComponent<PhotonView>().ViewID);
                }
            }
        }
    }

    // ��Ƽ ���� RPC
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

        // ��Ƽ ����� �ƴ� ���
        if (!playerCtrl.isPartyMember)
        {
            playerCtrl.isPartyMember = true;
            playerCtrl.party = parties[partyIdx];
            playerCtrl.party.SetPartyMemberID(playerViewID);
        }
        else
        {
            Debug.Log(PhotonNetwork.NickName + " ���� �̹� ��Ƽ�� ���� �ֽ��ϴ�!");
        }
    }

    // Ư�� ��Ƽ�� ���� �ִ� �÷��̾ ���� �� ȣ�� (LobbyManager.cs)
    [PunRPC]
    public void HandlePlayerGameExit(string playerName)
    {
        PlayerCtrl playerCtrl = GetPlayerCtrlByNickname(playerName);

        if (playerCtrl)
        {
            PhotonView playerPV = playerCtrl.GetComponent<PhotonView>();

            // ��Ƽ�� ���ԵǾ� �ִ� ���
            if (playerCtrl.isPartyMember)
            {
                for (var i = 0; i < parties.Count; i++)
                {
                    if (playerCtrl.party.partyID == parties[i].partyID)
                    {
                        // ���� ��Ƽ���� �����Ÿ�
                        if (parties[i].GetPartyLeaderID() == playerPV.ViewID)
                        {
                            // ��Ƽ���� �����ߴ� ��Ƽ�� �ش� ��Ƽ���� ��Ƽ������ �����Ѵ�.
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

    // ��Ƽ Ż�� ��ư
    public void OnLeavePartyButtonClick()
    {
        PlayerCtrl playerCtrl = GetPlayerCtrlByNickname(PhotonNetwork.NickName);

        canvasPV.RPC("OnLeavePartyRPC", RpcTarget.AllBuffered, playerCtrl.GetComponent<PhotonView>().ViewID);
    }

    // ��Ƽ�� ���� �� ���̴� RPC
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
                // ��Ƽ�� �ش�Ǵ� �ο��� 1���̸� ��Ƽ�� �����.
                if (parties[partyIdx].GetPartyHeadCount() == 1)
                {
                    parties[partyIdx].GetComponent<Button>().onClick.RemoveListener(OnClickJoinPartyButton);
                    Destroy(parties[partyIdx].gameObject);
                    parties.Remove(parties[partyIdx]);
                }
                else
                {
                    // 2���̸� �ش� �÷��̾ ��Ƽ������ ��Ƽ������ Ȯ���Ѵ�.
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
            Debug.Log("�ش� �÷��̾�� ��Ƽ�� ���ԵǾ����� �ʽ��ϴ�!");
        }
    }

    // ��Ƽ �� ����
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
