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

    public AudioSource audioSource;
    public AudioClip notificationSound;

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        if (inputField.isFocused)
        {
            chatView.SetActive(true);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (inputField.text != "")
                {
                   SendMessage();
                }
            }
        }
    }

    // 채팅 끄기(버튼 이벤트 함수)
    public void CloseChatWindowOnButtonClick()
    {
        inputField.interactable = false;
        chatView.SetActive(false);
    }

    public void SendMessage()
    {
        audioSource.PlayOneShot(notificationSound);
        // 닉네임 : 메세지 형식으로 전달
        this.GetComponent<PhotonView>().RPC("GetMessage", RpcTarget.All, PhotonNetwork.NickName, inputField.text);

        //inputField.Select(); // 입력 부분의 필드가 활성화되어있으면 비활성화, 비활성화 상태이면 활성화
        inputField.text = ""; // 입력 부분의 텍스트 비워주기
    }

    [PunRPC]
    public void GetMessage(string nickName, string receiveMessage)
    {
        // 메세지 생성 후 해당 메세지를 받은 메세지로 채우기
        GameObject msg = PhotonNetwork.Instantiate(message.name, Vector3.zero, Quaternion.identity);
        msg.transform.parent = content.transform;

        Message mes = msg.GetComponent<Message>();
        mes.SetNickName(nickName);
        mes.SetMessage(receiveMessage);
    }
}
