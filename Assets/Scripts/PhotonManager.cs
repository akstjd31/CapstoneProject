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

    private static string nickname;
    public static int leavingPlayer = 0;
    public static int[] dungeonJoiningPlayer = new int[2];

    [SerializeField] private string charType = "";

    public int playerWeaponIdx;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        dungeonJoiningPlayer[0] = 0;
        dungeonJoiningPlayer[1] = 0;
        inputText.ActivateInputField();
        //내용이 변경되었을때
        inputText.onValueChanged.AddListener(OnValueChanged);
        //내용을 제출했을때
        inputText.onSubmit.AddListener(OnSubmit);
        //커서가 다른곳을 누르면
        inputText.onEndEdit.AddListener(
            (string s) =>
            {
                //Debug.Log(s + "님이 입장하셨습니다.");
            }
        );
        inputButton.onClick.AddListener(OnClickConnect);
    }

    private void Update()
    {
        //if (SceneManager.GetActiveScene().name == "DungeonScene")
        //{
        //    if (charType != "")
        //    {
        //        GameObject.FindGameObjectWithTag("PhotonManager").GetComponent<DungeonPhotonMananger>().SetCharType(charType);
        //        charType = "";

        //        Destroy(gameObject);
        //    }
            
        //}
    }
    public void SetCharType(string charType)
    {
        this.charType = charType;
    }

    public string GetCharType()
    {
        return charType;
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
        PhotonNetwork.NickName = inputText != null ? inputText.text : nickname;
        PhotonNetwork.AutomaticallySyncScene = true;
        //로비진입
        PhotonNetwork.JoinLobby();
    }

    //Lobby 진입을 성공했으면 호출되는 함수
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        // OnClickCreateRoom();
        //PhotonNetwork.LoadLevel("LobbyScene");
        PhotonNetwork.LoadLevel("PhotonLobbyScene");
        print("로비 진입 성공");
    }

    public void OnClickConnect()
    {
        //PhotonNetwork.ConnectUsingSettings();        
    }

    //회원가입 시 호출되는 메소드
    public static void ConnectWithRegister()
    {
        // 마스터 서버 접속 요청
        PhotonNetwork.ConnectUsingSettings();
    }

    public static void SetNickname(string nick)
    {
        nickname = nick;
    }
}