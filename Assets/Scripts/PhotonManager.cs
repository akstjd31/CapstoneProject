using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.PointerEventData;


public class PhotonManager : MonoBehaviourPunCallbacks
{
    private PhotonManager s_instance;
    public PhotonManager Instance { get { return s_instance; } }

    [SerializeField]
    InputField inputText;
    [SerializeField]
    Button inputButton;

    void Start()
    {
        inputText.ActivateInputField();
        //내용이 변경되었을때
        inputText.onValueChanged.AddListener(OnValueChanged);
        //내용을 제출했을때
        inputText.onSubmit.AddListener(OnSubmit);
        //커서가 다른곳을 누르면
        inputText.onEndEdit.AddListener(
            (string s) =>
            {
                Debug.Log(s + "님이 입장하셨습니다.");
            }
        );
        inputButton.onClick.AddListener(OnClickConnect);
    }

    void OnValueChanged(string s)
    {
        inputButton.interactable = s.Length > 0;
    }
    
    void OnSubmit(string s)
    {
        Debug.Log(s + "님 입장하셨습니다.");
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("마스터 서버 접속 성공");

        //나의 이름을 포톤에 설정
        PhotonNetwork.NickName = inputText.text;
        PhotonNetwork.AutomaticallySyncScene = true;
        //로비진입
        PhotonNetwork.JoinLobby();
    }

    //Lobby 진입을 성공했으면 호출되는 함수
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        // OnClickCreateRoom();
        PhotonNetwork.LoadLevel("LobbyScene");
        print("로비 진입 성공");
    }

    public void OnClickConnect()
    {
        // 마스터 서버 접속 요청
        PhotonNetwork.ConnectUsingSettings();
    }
}