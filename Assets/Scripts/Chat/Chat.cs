using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;


public class Chat : MonoBehaviour
{
    public InputField inputField; // 입력 부분
    public GameObject message; // 프리팹 텍스트
    public GameObject content; // 스크롤 뷰에 존재하는 컨텐트
    public GameObject chatView; // 스크롤 뷰

    private int increaseHeight = 30;

    void Update()
    {
        if (inputField.isFocused)
        {
            chatView.SetActive(true);
        }
    }

    //
    public void CloseChatWindowOnButtonClick()
    {
        chatView.SetActive(false);
    }

    public void SendMessage()
    {   
        // 닉네임 : 메세지 형식으로 전달
        this.GetComponent<PhotonView>().RPC("GetMessage", RpcTarget.All, PhotonNetwork.NickName, inputField.text);

        //inputField.Select(); // 입력 부분의 필드가 활성화되어있으면 비활성화, 비활성화 상태이면 활성화
        inputField.text = ""; // 입력 부분의 텍스트 비워주기
    }

    [PunRPC]
    public void GetMessage(string nickName, string receiveMessage)
    {
        // 10번째 자리마다 개행 문자(\n)를 삽입하여 문자열 다듬기
        string formattedMessage = "";
        int cnt = 0;
        for (int i = 0; i < receiveMessage.Length; i++)
        {
            formattedMessage += receiveMessage[i];

            // 전체 길이로 개행 문자 추가 => 텍스트 필드의 폭과 텍스트의 전체 폭을 비교하여 개행 문자를 넣는 식으로 변경해야 함.
            if ((i + 1) % 13 == 0)
            {
                formattedMessage += "\n";
                cnt++;
            }
                
        }

        // 메세지 생성 후 해당 메세지를 받은 메세지로 채우기
        GameObject msg = Instantiate(message, Vector3.zero, Quaternion.identity, content.transform);
        Message messageScript = msg.GetComponent<Message>();

        messageScript.NickName.text = nickName;
        messageScript.myMessage.text = formattedMessage; 

        Vector2 newHeight = messageScript.myMessage.GetComponent<RectTransform>().sizeDelta;
        newHeight.y += cnt * increaseHeight;

        RectTransform messageRect, textBoxRect;
        messageRect = messageScript.myMessage.GetComponent<RectTransform>();
        textBoxRect = msg.GetComponentInChildren<Image>().GetComponent<RectTransform>();

        messageRect.sizeDelta = newHeight;
        textBoxRect.sizeDelta = new Vector2(
            textBoxRect.sizeDelta.x,
            newHeight.y
            );

        // empty object의 크기도 변경
        msg.GetComponent<RectTransform>().sizeDelta = new Vector2(
        msg.GetComponent<RectTransform>().sizeDelta.x,
        messageScript.NickName.GetComponent<RectTransform>().sizeDelta.y + messageRect.sizeDelta.y + 50
        );

        messageScript.NickName.GetComponent<RectTransform>().position = new Vector2(
            messageScript.NickName.GetComponent<RectTransform>().position.x,
            -messageRect.position.y + messageRect.sizeDelta.y / 2);
    }
}
