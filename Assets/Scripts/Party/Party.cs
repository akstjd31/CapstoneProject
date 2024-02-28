using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Party : MonoBehaviour
{
    public Text Title;

    public List<int> partyMembers;


    PartySystem partySystemScript;

    string textContent;

    LobbyManager lobbyManager;

    private void Start()
    {
        partySystemScript = this.GetComponentInParent<PartySystem>();
        lobbyManager = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();

        textContent = Title.text;
    }

    private void Update()
    {
        if (partyMembers.Count < 2)
        {
            Title.text = textContent + "\n (1 / 2)";
        }
        else
        {
            Title.text = textContent + "\n (2 / 2)";
        }
    }

    // ��Ƽ â�� �����ϴ� ��Ƽ�� Ŭ������ �� �̺�Ʈ �߻�
    public void OnClickJoinPartyButton()
    {
        // ��Ƽ�� �̹� �� �� �ִٸ�
        if (partyMembers.Count >= 2)
        {
            Debug.Log("��Ƽ �ο��� �� �� �ֽ��ϴ�!");
        }
        else
        {
            PhotonView targetPhotonView = PhotonView.Find(partyMembers[0]);

            // �ش� ��Ƽ�� ���� �ִ� ��Ƽ���� ������ �ҷ��� ���Ѵ�.
            if (PhotonNetwork.NickName.Equals(targetPhotonView.Owner.NickName))
            {
                Debug.Log(PhotonNetwork.NickName + " �Բ����� �̹� �ش� ��Ƽ�� ���ԵǾ� �ֽ��ϴ�!");
            }
            else
            {
                foreach (int playerViewID in lobbyManager.lobbyPlayerViewID)
                {
                    PhotonView playerPhotonView = PhotonView.Find(playerViewID);

                    // ���� �÷��̾��� �г��Ӱ� ���Ͽ� ã��
                    if (playerPhotonView != null && playerPhotonView.Owner.NickName.Equals(PhotonNetwork.NickName))
                    {
                        PlayerCtrl secondPlayerCtrl = playerPhotonView.GetComponent<PlayerCtrl>();
                        secondPlayerCtrl.isPartyMember = true;

                        partyMembers.Add(playerPhotonView.ViewID);
                        //GetComponent<PhotonView>().RPC("SyncPartyMembers", RpcTarget.All, partyMembers);
                        //PlayerCtrl firstPlayerCtrl = targetPhotonView.GetComponent<PlayerCtrl>();
                        //firstPlayerCtrl.party.partyMembers.Add(playerPhotonView.ViewID);
                        secondPlayerCtrl.party = this;

                        //PhotonView partyLeaderPhotonView = PhotonView.Find(partyMembers[0]);
                        //partySystemScript.partyMemberHUD[0].GetComponentInChildren<Text>().text = partyLeaderPhotonView.Owner.NickName;
                        //partySystemScript.partyMemberHUD[0].SetActive(true);

                        //partySystemScript.partyMemberHUD[1].GetComponentInChildren<Text>().text = PhotonNetwork.NickName;
                        //partySystemScript.partyMemberHUD[1].SetActive(true);
                    }
                }
            }
        }
    }

    [PunRPC]
    public void SyncPartyMembers(List<int> syncedPartyMembers)
    {
        partyMembers = syncedPartyMembers;
    }

    public bool HasPartyLeaderViewID()
    {
        if (partyMembers.Count < 1)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public int GetPartyLeaderViewID()
    {
        return partyMembers[1];
    }

    public bool HasPartyMemberViewID()
    {
        if (partyMembers.Count < 2)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public int GetPartyMemberViewID()
    {
        return partyMembers[1];
    }
}
