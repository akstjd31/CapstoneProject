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

    // 파티 창에 존재하는 파티를 클릭했을 때 이벤트 발생
    public void OnClickJoinPartyButton()
    {
        // 파티가 이미 꽉 차 있다면
        if (partyMembers.Count >= 2)
        {
            Debug.Log("파티 인원이 꽉 차 있습니다!");
        }
        else
        {
            PhotonView targetPhotonView = PhotonView.Find(partyMembers[0]);

            // 해당 파티에 속해 있는 파티원의 정보를 불러와 비교한다.
            if (PhotonNetwork.NickName.Equals(targetPhotonView.Owner.NickName))
            {
                Debug.Log(PhotonNetwork.NickName + " 님께서는 이미 해당 파티에 가입되어 있습니다!");
            }
            else
            {
                foreach (int playerViewID in lobbyManager.lobbyPlayerViewID)
                {
                    PhotonView playerPhotonView = PhotonView.Find(playerViewID);

                    // 현재 플레이어의 닉네임과 비교하여 찾음
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
