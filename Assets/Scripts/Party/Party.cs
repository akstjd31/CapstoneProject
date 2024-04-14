using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Party : MonoBehaviourPun, IPunObservable
{
    public Text Title;
    private string context;

    [SerializeField] private int partyLeaderID = -1;
    [SerializeField] private int partyMemberID = -1;
    public PlayerCtrl[] partyMembers = new PlayerCtrl[2];

    private int MAX_MEMBER = 2;

    public int partyID;

    public int GetPartyHeadCount()
    {
        if (partyLeaderID != -1 && partyMemberID != -1)
        {
            return 2;
        }
        else
        {
            return 1;
        }
    }

    public void SetPartyLeaderID(int leaderID)
    {
        this.partyLeaderID = leaderID;
    }

    public int GetPartyLeaderID()
    {
        return partyLeaderID;
    }

    public void SetPartyMemberID(int memberID)
    {
        this.partyMemberID = memberID;
    }

    public int GetPartyMemberID()
    {
        return partyMemberID;
    }

    public string GetContext()
    {
        return context;
    }

    public void SetContext(string context)
    {
        this.context = context;
    }

    // 파티 목록에 보여지는 텍스트 업데이트
    private void Update()
    {
        Title.text = context + " ( " + GetPartyHeadCount() + " / " + MAX_MEMBER + " )";
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(partyLeaderID);
            stream.SendNext(partyMemberID);
        }
        else
        {
            this.partyLeaderID = (int)stream.ReceiveNext();
            this.partyMemberID = (int)stream.ReceiveNext();
        }
    }
}
