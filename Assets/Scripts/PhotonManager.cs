using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.PointerEventData;
using Firebase.Auth;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private PhotonManager s_instance;
    public PhotonManager Instance { get { return s_instance; } }

    [SerializeField]
    InputField inputText;
    [SerializeField]
    InputField inputPassword;
    [SerializeField]
    Button inputButton;

    private int isSocialLogin = 0;

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
                Debug.Log(s + "님이 입장하셨습니다. temp");
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
        /*
        //다른 로그인 방법을 사용 중인지
        if(isSocialLogin == 1)
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        //이메일과 비밀번호가 정상적으로 입력 되었는지 확인
        if(inputText.text == "" || inputPassword.text == "")
        {
            return;
        }
        //입력한 이메일과 비밀번호로 로그인이 되는지
        else if(!UserRegister.IsValidAccount(inputText.text, inputPassword.text))
        {
            //이메일과 비밀번호가 잘 못 입력된 경우
            return;
        }

        // 마스터 서버 접속 요청
        if(UserData.CanEnter())
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        */

        if(UserData.GetLoginState())
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    //회원가입 시 호출되는 메소드
    public static void ConnectWithRegister()
    {
        // 마스터 서버 접속 요청
        PhotonNetwork.ConnectUsingSettings();
    }
}