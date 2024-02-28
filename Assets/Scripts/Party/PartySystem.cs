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


    // 파티 생성 버튼을 눌렀을 때
    public void OnClickCompleteButton()
    {
        // 어떤 플레이어가 누른건지 확인하기 위한 foreach
        foreach (int playerViewID in lobbyManager.lobbyPlayerViewID)
        {
            PhotonView targetPhotonView = PhotonView.Find(playerViewID);

            // 현재 플레이어의 닉네임과 비교하여 찾음
            if (targetPhotonView != null && targetPhotonView.Owner.NickName.Equals(PhotonNetwork.NickName))
            {
                PlayerCtrl playerCtrl = targetPhotonView.GetComponent<PlayerCtrl>();
                
                // 만약 해당 플레이어가 어느 파티에 속하지 않을 때
                if (!playerCtrl.isPartyMember)
                {
                    // 방 생성
                    playerCtrl.isPartyMember = true;
                    canvasPV.RPC("GetPartyRoom", RpcTarget.All, inputField.text, targetPhotonView.ViewID);
                    Debug.Log(PhotonNetwork.NickName + " 파티가 생성되었습니다.");
                    partyCreator.SetActive(false); // 파티 만들기 창 비활성화

                   //partyMemberHUD[0].GetComponentInChildren<Text>().text = PhotonNetwork.NickName;

                    // 로비에서 굳이 체력바를 전달할 필요가 있나해서 주석함
                    //float hp = playerCtrl.GetComponent<Status>().HP;
                    //float maxHP = playerCtrl.GetComponent<Status>().MAXHP;
                    //partyMemberHUD[0].GetComponentInChildren<Slider>().value = hp / maxHP;
                    //partyMemberHUD[0].SetActive(true); // HUD 활성화
                    break;
                }
                else
                {
                    Debug.Log(PhotonNetwork.NickName + " 님은 이미 파티에 속해 있습니다!");
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
