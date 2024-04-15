using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Message : MonoBehaviour
{
    public Text NickName;
    public Text myMessage;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {

    }

    public void SetNickName(string nickName)
    {
        NickName.text = nickName;
    }

    public void SetMessage(string message)
    {
        myMessage.text = message;
    }
}
