using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginProgress : MonoBehaviour
{
    private FirebaseAuth auth;

    private InputField inputField_email;
    private InputField inputField_password;
    private Text input_email;
    private Text input_password;
    private Button btn_submit;

    private void Start()
    {
        inputField_email = GameObject.Find("InputEmail").GetComponent<InputField>();
        inputField_password = GameObject.Find("InputPassword").GetComponent<InputField>();

        input_email = GameObject.Find("InputEmail").GetComponentInChildren<Text>();
        input_password = GameObject.Find("InputPassword").GetComponentInChildren<Text>();
        btn_submit = GameObject.Find("Submit").GetComponent<Button>();

        inputField_email.onEndEdit.AddListener(delegate { OnEndEdit(); });
        inputField_password.onEndEdit.AddListener(delegate { OnEndEdit(); });
    }

    private void Update()
    {
        if (inputField_email.isFocused)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                inputField_password.Select();
                inputField_password.ActivateInputField();
            }
        }
        else if (inputField_password.isFocused)
        {
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    inputField_email.Select();
                    inputField_email.ActivateInputField();
                }
            }
        }
    }

    public void OnEndEdit()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!inputField_email.text.Equals("") || !inputField_password.Equals(""))
            {
                btn_submit.onClick.Invoke();
            }
        }
    }

    //정상 동작하는 이메일 로그인 메소드 (##수정 금지##)
    public void SigninWithEmail()
    {
        SignInWithEmailAndPassword();
    }

    private async Task SignInWithEmailAndPassword()
    {
        auth = FirebaseAuth.DefaultInstance;

        string email = GameObject.Find("InputEmaiil").GetComponentInChildren<Text>().text;
        string password = GameObject.Find("InputPassword").GetComponentInChildren<Text>().text;

        try
        {
            // 이메일과 비밀번호로 사용자 인증 시도
            var authResult = await FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);
            Debug.Log($"User signed in successfully: {authResult.User.Email}");
            // 로그인 성공 시 추가 작업 수행
            UserInfoManager.SetCurrentUser(auth.CurrentUser);
            SceneManager.LoadScene("SelectCharacter");
        }
        catch (Exception e)
        {
            // 인증 실패 시 오류 처리
            Debug.LogError($"Sign in failed: {e.Message}");
        }
    }
    //↑ 정상 동작하는 이메일 로그인 메소드 (##수정 금지##)

    //정상 동작 + 가입 진행도에 따른 조건 추가
    public void SigninWithEmail_Condition()
    {
        SignInWithEmailAndPassword_Condition();
    }

    private async Task SignInWithEmailAndPassword_Condition()
    {
        auth = FirebaseAuth.DefaultInstance;

        string email = inputField_email.text;
        string password = inputField_password.text;

        if (auth.CurrentUser != null)
        {
            auth.SignOut();
        }

        try
        {
            // 이메일과 비밀번호로 사용자 인증 시도
            var authResult = await FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);
            Debug.Log($"User signed in successfully: {authResult.User.Email}");
            // 로그인 성공 시 추가 작업 수행
            UserInfoManager.SetCurrentUser(auth.CurrentUser);
            //SceneManager.LoadScene("SelectCharacter");
            Check_Condition(email);
        }
        catch (Exception e)
        {
            // 인증 실패 시 오류 처리
            Debug.LogError($"Sign in failed: {e.Message}");
        }
    }

    private async void Check_Condition(string email)
    {
        //DB 생성 여부
        bool existDB = await IsUserDocumentExists(email);
        //Debug.Log($"Check_Condition // existDB : {existDB}");
        if (!existDB)
        {
            await UserData.MakeDB_New();
            SceneManager.LoadScene("SelectCharacter");
            return;
        }

        //닉네임 생성 여부 (+ 캐릭터 종류 선택 여부)
        bool isNicknameSet = await IsNicknameDocumentExists(email);
        //Debug.Log($"Check_Condition // isNicknameSet : {isNicknameSet}");
        if (!isNicknameSet)
        {
            SceneManager.LoadScene("SelectCharacter");
            return;
        }

        //Debug.Log($"{email} already set charType & nickname");
        //모든 가입 과정이 완료
        PhotonManager.ConnectWithRegister();
    }

    private async Task<bool> IsNicknameDocumentExists(string userEmail)
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.Log("Failed to check nickname document existence: user is null");
            return false;
        }

        // Firestore 인스턴스 생성
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // 데이터를 조회할 닉네임 컬렉션 문서 참조
        CollectionReference coll_nickname = db.Collection("Nickname");
        DocumentReference doc_nickname = coll_nickname.Document(userEmail);

        // 해당 이메일을 가진 닉네임 문서가 존재하는지 확인
        DocumentSnapshot snapshot = await doc_nickname.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            //Debug.Log($"Nickname document with email {userEmail} exists.");
            return true;
        }
        else
        {
            //Debug.Log($"Nickname document with email {userEmail} does not exist.");
            return false;
        }
    }


    private async Task<bool> IsUserDocumentExists(string userEmail)
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.Log("Failed to check document existence: user is null");
            return false;
        }

        // Firestore 인스턴스 생성
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // 데이터를 조회할 컬렉션과 문서 참조
        CollectionReference coll_userdata = db.Collection("User");
        DocumentReference doc_user = coll_userdata.Document(userEmail);

        // 해당 이메일을 가진 문서가 존재하는지 확인
        DocumentSnapshot snapshot = await doc_user.GetSnapshotAsync();

        if (snapshot.Exists)
        {
            //Debug.Log($"Document with email {userEmail} exists.");
            return true;
        }
        else
        {
            //Debug.Log($"Document with email {userEmail} does not exist.");
            return false;
        }
    }

    public void BreakLogin()
    {
        //SceneManager.LoadScene("SelectCharacter");
        PhotonManager.ConnectWithRegister();
    }
}
