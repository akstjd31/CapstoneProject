using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class ChatManager : MonoBehaviourPunCallbacks
{
    public GameObject chatView;
    public Text messages;
    public Text input;
    [SerializeField] private string username = "";
    private PhotonView playerPV, chatPV;
    [SerializeField] private InputField inputMessage;

    public static bool inputMessageFocusedOn = false;

    void Start()
    {
        chatPV = GetComponent<PhotonView>();
        playerPV = null;

        if (input)
        {
            inputMessage = input.transform.parent.GetComponent<InputField>();
        }
    }

    void Update()
    {
        if (playerPV == null)
        {
            if (GameObject.FindGameObjectWithTag("Player"))
            {
                playerPV = GameObject.FindGameObjectWithTag("Player").GetComponent<PhotonView>();
                username = playerPV.Owner.NickName;
            }
        }

        if (inputMessage != null)
        {
            if (inputMessage.isFocused)
            {
                inputMessageFocusedOn = true;
            }
            else
            {
                inputMessageFocusedOn = false;
            }
        }

        chatView.SetActive(inputMessage.isFocused);
    }

    public void CallMessageRPC()
    {
        string message = input.text;
        chatPV.RPC("RPC_SendMessage", RpcTarget.All, username, message);
    }

    [PunRPC]
    public void RPC_SendMessage(string username, string message)
    {
        messages.text += $"{username}: {message}\n";
        
    }
}