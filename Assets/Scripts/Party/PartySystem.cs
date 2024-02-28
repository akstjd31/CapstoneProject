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

    private List<Party> parties = new List<Party>();

    LobbyManager lobbyManager;

    PhotonView canvasPV;

    private void Start()
    {
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        canvasPV = GetComponent<PhotonView>();
    }

    public Party GetLastPartyList()
    {
        return parties[parties.Count - 1];
    }

    public void OpenPartyViewer()
    {
        partyView.SetActive(!partyView.activeSelf);
    }

    public void OpenPartyCreator()
    {
        partyCreator.SetActive(!partyCreator.activeSelf);
        inputField.text = "";
    }


    // ��Ƽ ���� ��ư�� ������ ��
    public void OnClickCompleteButton()
    {
        // � �÷��̾ �������� Ȯ���ϱ� ���� foreach
        foreach (int playerViewID in lobbyManager.lobbyPlayerViewID)
        {
            PhotonView targetPhotonView = PhotonView.Find(playerViewID);

            // ���� �÷��̾��� �г��Ӱ� ���Ͽ� ã��
            if (targetPhotonView != null && targetPhotonView.Owner.NickName.Equals(PhotonNetwork.NickName))
            {
                PlayerCtrl playerCtrl = targetPhotonView.GetComponent<PlayerCtrl>();
                
                // ���� �ش� �÷��̾ ��� ��Ƽ�� ������ ���� ��
                if (!playerCtrl.isPartyMember)
                {
                    // �� ����
                    playerCtrl.isPartyMember = true;
                    canvasPV.RPC("GetPartyRoom", RpcTarget.All, inputField.text, targetPhotonView.ViewID);
                    Debug.Log(PhotonNetwork.NickName + " ��Ƽ�� �����Ǿ����ϴ�.");
                    partyCreator.SetActive(false); // ��Ƽ ����� â ��Ȱ��ȭ

                   //partyMemberHUD[0].GetComponentInChildren<Text>().text = PhotonNetwork.NickName;

                    // �κ񿡼� ���� ü�¹ٸ� ������ �ʿ䰡 �ֳ��ؼ� �ּ���
                    //float hp = playerCtrl.GetComponent<Status>().HP;
                    //float maxHP = playerCtrl.GetComponent<Status>().MAXHP;
                    //partyMemberHUD[0].GetComponentInChildren<Slider>().value = hp / maxHP;
                    //partyMemberHUD[0].SetActive(true); // HUD Ȱ��ȭ
                    break;
                }
                else
                {
                    Debug.Log(PhotonNetwork.NickName + " ���� �̹� ��Ƽ�� ���� �ֽ��ϴ�!");
                }
            }
        }
    }

    [PunRPC]
    public void GetPartyRoom(string receiveMessage, int ViewID)
    {
        GameObject room = PhotonNetwork.Instantiate(partyRoom.name, Vector3.zero, Quaternion.identity);
        room.transform.parent = content.transform;

        Party newParty = room.GetComponent<Party>();
        newParty.Title.text = receiveMessage;
        newParty.partyMembers.Add(ViewID);
        parties.Add(newParty);

        PhotonView.Find(ViewID).GetComponent<PlayerCtrl>().party = newParty;
    }
}
