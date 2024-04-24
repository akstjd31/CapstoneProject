using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginProgress : MonoBehaviour
{
    FirebaseAuth auth;
    Credential credential;

    // Start is called before the first frame update
    void Start()
    {
        /*
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
            }
            else
            {
                Debug.LogError("Could not resolve Firebase dependencies: " + task.Result);
                return;
            }
        });
        */
    }

    public void SigninWithEmail()
    {
        /*
        auth = FirebaseAuth.DefaultInstance;

        string email = GameObject.Find("InputEmaiil").GetComponentInChildren<Text>().text.Trim();
        string password = GameObject.Find("InputPassword").GetComponentInChildren<Text>().text.Trim();

        auth.SignInWithEmailAndPasswordAsync(email, password);
        */

        SignInWithEmailAndPassword();
    }

    private async void ForceLoginWithCred()
    {
        auth = FirebaseAuth.DefaultInstance;
        credential = EmailAuthProvider.GetCredential("ggem@gmail.com", "qweqwe123@@qwe");
        Debug.Log($"credential : {credential.IsValid()}");

        await auth.SignInWithCredentialAsync(credential);
        Debug.Log("complete login");
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

    //비밀번호를 체크하지 않음
    private async void Signin_Register_Trick()
    {
        auth = FirebaseAuth.DefaultInstance;

        string email = GameObject.Find("InputEmaiil").GetComponentInChildren<Text>().text;
        string password = GameObject.Find("InputPassword").GetComponentInChildren<Text>().text;

        //로그인 가능한 조건
        int SignType = await Trick_IsExistDB(email);
        switch(SignType)
        {
            //정상
            case 0:
                await auth.CreateUserWithEmailAndPasswordAsync(email, password);
                await auth.SignInWithEmailAndPasswordAsync(email, password);

                UserInfoManager.SetCurrentUser(auth.CurrentUser);
                SceneManager.LoadScene("SelectCharacter");
                break;
            //account is not exist
            case 1:
                break;
            //db is not exitst
            case 2:
                break;
            //nickname is not set
            case 3:
                break;

        }
    }

    private async Task<int> Trick_IsExistDB(string email)
    {
        //가입 여부
        bool existAccount = await IsExistAccount(email);
        if (!existAccount)
        {
            return 1;
        }
        //DB 생성 여부
        bool existDB = await IsUserDocumentExists(email);
        if (!existDB)
        {
            return 2;
        }

        //닉네임 생성 여부
        bool isNicknameSet = await IsNicknameDocumentExists(email);
        if (!isNicknameSet)
        {
            return 3;
        }

        return 0;
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
            Debug.Log($"Nickname document with email {userEmail} exists.");
            return true;
        }
        else
        {
            Debug.Log($"Nickname document with email {userEmail} does not exist.");
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
            Debug.Log($"Document with email {userEmail} exists.");
            return true;
        }
        else
        {
            Debug.Log($"Document with email {userEmail} does not exist.");
            return false;
        }
    }

    private async Task<bool> IsExistAccount(string email)
    {
        auth = FirebaseAuth.DefaultInstance;
        bool isRegisteredAccount = false;
        Debug.Log($"check accout : {email}");

        //가입된 이메일인지 확인
        await auth.FetchProvidersForEmailAsync(email).ContinueWith((authTask) =>
        {
            if (authTask.IsCanceled)
            {
                Debug.Log("Provider fetch canceled.");
            }
            else if (authTask.IsFaulted)
            {
                Debug.Log("Provider fetch encountered an error.");
                Debug.Log(authTask.Exception.ToString());
            }
            else if (authTask.IsCompleted)
            {
                Debug.Log($"Email Providers: {authTask.Result}");
                isRegisteredAccount = true;
            }
        });

        return isRegisteredAccount;
    }

    private async void SigninWithEmailAsync2()
    {
        auth = FirebaseAuth.DefaultInstance;

        string email = GameObject.Find("InputEmaiil").GetComponentInChildren<Text>().text;
        string password = GameObject.Find("InputPassword").GetComponentInChildren<Text>().text;

        var authTask = await auth.SignInWithEmailAndPasswordAsync(email, password);
        Debug.Log($"authTask : {authTask}");
    }

    private async void SigninWithEmailAsync()
    {
        auth = FirebaseAuth.DefaultInstance;

        string email = GameObject.Find("InputEmaiil").GetComponentInChildren<Text>().text.Trim();
        string password = GameObject.Find("InputPassword").GetComponentInChildren<Text>().text.Trim();

        AuthResult result;

        //�α��� �õ�
        try
        {
            Debug.Log($"auth2 : {auth.CurrentUser}\n{email}\t{password}");
            //await auth.SignInWithEmailAndPasswordAsync(email, password);

            // SignInWithEmailAndPasswordAsync �޼��带 �񵿱������� ȣ���ϰ� ����� ��ٸ�
            await auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                AuthResult result = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);
            });

            Debug.Log("end of sign in");
        }
        catch (Exception ex)
        {
            Debug.Log($"auth2 failed : {ex.StackTrace}");
            Debug.Log($"auth2 failed : {ex.Message}");
            return;
        }
    }

    public void BreakLogin()
    {
        //SceneManager.LoadScene("SelectCharacter");
        PhotonManager.ConnectWithRegister();
    }
}
