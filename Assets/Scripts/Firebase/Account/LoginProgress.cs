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
    private Button btn_submit;

    private Text loginErrorMsg;
    private string[] errMsg = {
        "이메일과 비밀번호를\n모두 입력해 주세요",
        "이메일 또는 비밀번호가\n일치하지 않습니다"
    };

    private void Start()
    {
        inputField_email = GameObject.Find("InputEmail").GetComponent<InputField>();
        inputField_password = GameObject.Find("InputPassword").GetComponent<InputField>();

        btn_submit = GameObject.Find("Submit").GetComponent<Button>();
        loginErrorMsg = GameObject.Find("Error_msg").GetComponent<Text>();
        loginErrorMsg.text = "";

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
            if (!string.IsNullOrEmpty(inputField_email.text) && !string.IsNullOrEmpty(inputField_password.text))
            {
                //Debug.Log($"before invoke : {string.IsNullOrEmpty(inputField_email.text)}, {string.IsNullOrEmpty(inputField_password.text)}");
                btn_submit.onClick.Invoke();
            }
            else
            {
                loginErrorMsg.text = errMsg[0];
                Debug.Log($"show errMsg 0 - 03 : {inputField_email.text}, {inputField_password.text}");
            }
        }
    }

    //정상 동작하는 이메일 로그인 메소드 (##수정 금지##)
    public void SigninWithEmail()
    {
#pragma warning disable CS4014
        SignInWithEmailAndPassword();
#pragma warning restore CS4014
    }

    private async Task SignInWithEmailAndPassword()
    {
        auth = FirebaseAuth.DefaultInstance;

        string email = GameObject.Find("InputEmail").GetComponentInChildren<Text>().text;
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
#pragma warning disable CS4014
        SignInWithEmailAndPassword_Condition();
        //SignUpAndIn();
#pragma warning restore CS4014
    }

    private async Task SignInWithEmailAndPassword_Condition()
    {
        auth = FirebaseAuth.DefaultInstance;

        string email = inputField_email.text.Trim();
        string password = inputField_password.text.Trim();

        if (string.IsNullOrEmpty(inputField_email.text) || string.IsNullOrEmpty(inputField_password.text))
        {
            loginErrorMsg.text = errMsg[0];
            //Debug.Log($"show errMsg 0 - 01 : {inputField_email.text}, {inputField_password.text}");
            return;
        }
        loginErrorMsg.text = "";


        if (auth.CurrentUser != null)
        {
            auth.SignOut();
        }

        Debug.Log("front of try");

        // 이메일과 비밀번호로 사용자 인증 시도
        //var authResult = await FirebaseAuth.DefaultInstance.SignInWithEmailAndPasswordAsync(email, password);
        await auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            //Debug.Log($"{task.IsCompleted}, {task.IsCompletedSuccessfully}, {task.Id}, {task.Result.User.Email}");

            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }
            else if (task.IsCompletedSuccessfully)
            {
                Debug.Log($"IsCompletedSuccessfully : {auth.CurrentUser}");

                Firebase.Auth.AuthResult result = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);

                UserInfoManager.SetCurrentUser(auth.CurrentUser);

                Check_Condition(email);
            }
            else if (task.IsCompleted)
            {
                Debug.Log($"IsCompleted : {auth.CurrentUser}");

                Firebase.Auth.AuthResult result = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);

                UserInfoManager.SetCurrentUser(auth.CurrentUser);

                Check_Condition(email);
            }
            else
            {
                Debug.Log($"task => {task.IsCanceled}, {task.IsFaulted}, {task.IsCompletedSuccessfully}, {task.IsCompleted}");
            }
        });
    }

    //더미 메일로 회원가입을 시키고, 그 뒤 입력받은 데이터로 로그인을 수행
    //Firebase.FirebaseException: An internal error has occurred.
    private async Task SignUpAndIn()
    {
        auth = FirebaseAuth.DefaultInstance;

        string email = inputField_email.text.Trim();
        string password = inputField_password.text.Trim();

        if (string.IsNullOrEmpty(inputField_email.text) || string.IsNullOrEmpty(inputField_password.text))
        {
            loginErrorMsg.text = errMsg[0];
            //Debug.Log($"show errMsg 0 - 01 : {inputField_email.text}, {inputField_password.text}");
            return;
        }
        loginErrorMsg.text = "";


        if (auth.CurrentUser != null)
        {
            auth.SignOut();
        }

        Debug.Log("SignUpAndIn");

        string dum_email = "dummyMail";
        string dum_password = "qweqwe123!@#";

        var random = new System.Random();
        string randomNumber = random.Next(1000000, 9999999).ToString();
        dum_email = dum_email + randomNumber + "@gmail.com";

        await auth.CreateUserWithEmailAndPasswordAsync(dum_email, dum_password).ContinueWith(async task =>
        {
            if(task.IsCompletedSuccessfully)
            {
                Debug.Log("SignUpAndIn create user success");

                // 이메일과 비밀번호로 사용자 인증 시도
                await auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                    if (task.IsCanceled)
                    {
                        Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                        return;
                    }
                    else if (task.IsFaulted)
                    {
                        Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                        return;
                    }
                    else if (task.IsCompletedSuccessfully)
                    {
                        Debug.Log($"IsCompletedSuccessfully : {auth.CurrentUser}");

                        Firebase.Auth.AuthResult result = task.Result;
                        Debug.LogFormat("User signed in successfully: {0} ({1})",
                            result.User.DisplayName, result.User.UserId);

                        UserInfoManager.SetCurrentUser(auth.CurrentUser);

                        Check_Condition(email);
                    }
                    else if (task.IsCompleted)
                    {
                        Debug.Log($"IsCompleted : {auth.CurrentUser}");

                        Firebase.Auth.AuthResult result = task.Result;
                        Debug.LogFormat("User signed in successfully: {0} ({1})",
                            result.User.DisplayName, result.User.UserId);

                        UserInfoManager.SetCurrentUser(auth.CurrentUser);

                        Check_Condition(email);
                    }
                    else
                    {
                        Debug.Log($"task => {task.IsCanceled}, {task.IsFaulted}, {task.IsCompletedSuccessfully}, {task.IsCompleted}");
                    }
                });
            }
        });
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
