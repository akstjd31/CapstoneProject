using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Party : MonoBehaviour
{
    public Text Title;
    public string context;

    public List<int> partyMembers;
    private int MAX_MEMBER = 2;

    public int partyID;

    // ��Ƽ ��Ͽ� �������� �ؽ�Ʈ ������Ʈ
    private void Update()
    {
        Title.text = context + " (" + partyMembers.Count + " / " + MAX_MEMBER + ")";
    }
}