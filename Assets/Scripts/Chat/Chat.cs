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

    void Update()
    {
        chatView.SetActive(inputField.isFocused); // 입력 부분의 필드가 Focus on 상태이면 스크롤 뷰 활성화
    }

    public void SendMessage()
    {   
        // 닉네임 : 메세지 형식으로 전달
        GetComponent<PhotonView>().RPC("GetMessage", RpcTarget.All, (PhotonNetwork.NickName + " : "  + inputField.text));

        inputField.Select(); // 입력 부분의 필드가 활성화되어있으면 비활성화, 비활성화 상태이면 활성화
        inputField.text = ""; // 입력 부분의 텍스트 비워주기
    }

    [PunRPC]
    public void GetMessage(string receiveMessage)
    {
        // 메세지 생성 후 해당 메세지를 받은 메세지로 채우기
        GameObject msg = Instantiate(message, Vector3.zero, Quaternion.identity, content.transform);
        msg.GetComponent<Message>().myMessage.text = receiveMessage;
    }
}
