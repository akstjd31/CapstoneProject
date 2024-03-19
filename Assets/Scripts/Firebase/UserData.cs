using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserData : MonoBehaviour
{
    private FirebaseAuth auth;
    private string authEmail = "test0311@gmail.com";
    private string authPassword = "asdf";

    public void SignInWithEmail_Password()
    {
        auth = FirebaseAuth.DefaultInstance;

        auth.CreateUserWithEmailAndPasswordAsync(authEmail, authPassword);

        Debug.Log("auth : " + auth);

        MakeDB();
    }

    public void LogoutWithEmail_Password()
    {
        if (auth != null)
        {
            auth.SignOut();

            Debug.Log("Complete Logout (auth : " + auth + ")");
        }
        else
        {
            Debug.Log("auth is already null");
        }
    }

    public void SignInWithGoogle()
    {
        auth = FirebaseAuth.DefaultInstance;

        if (auth == null)
        {
            Debug.LogError("Firebase Auth is not initialized.");
            return;
        }

        // Google 로그인 팝업 표시
        auth.SignInWithCredentialAsync(GoogleAuthProvider.GetCredential(null, null)).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            // 로그인 성공
            FirebaseUser user = task.Result;
            Debug.Log("Google User Signed In: " + user.DisplayName + " (" + user.Email + ")");
        });
    }

    public void GetCredential()
    {
        auth = FirebaseAuth.DefaultInstance;

        var apiKey = auth.App.Options.ApiKey;
        Debug.Log("apiKey : " + apiKey);    //apiKey : AIzaSyAvJu_xoV2eaiQmuy9Dsm2GvBXcPvhqw0I

        

        var credential = GoogleAuthProvider.GetCredential(null, null);

        //Debug.Log("auth : " + auth);    //auth : Firebase.Auth.FirebaseAuth
        //Debug.Log("credential : " + credential + ", isValid : " + credential.IsValid());    //credential : Firebase.Auth.Credential
        //var result = auth.SignInAndRetrieveDataWithCredentialAsync(credential).Result;    //error
        //Debug.Log("result : " + result);
        
        string clientId = "1049753969677-hfu0873d5sgcjf77dm1nbanqv14bk5g8.apps.googleusercontent.com";
        string clientSecret = "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";
        credential = GoogleAuthProvider.GetCredential(clientId, null);
        Debug.Log("credential : " + credential + ", isValid : " + credential.IsValid());


        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception);
                return;
            }

            Debug.Log("SignInAndRetrieveDataWithCredentialAsync Success");

            // 로그인 성공
            var result = task.Result;
            Debug.Log("result : " + result);
        });
        //auth.SignInWithCredentialAsync
    }

    //https://chat.openai.com/share/6c741b14-c6ab-41de-aecb-01658a21903b
    private const string clientId = "1049753969677-hfu0873d5sgcjf77dm1nbanqv14bk5g8.apps.googleusercontent.com";
    private const string clientSecret = "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";
    private const string redirectUri = "https://capstoneproject-8992c.firebaseapp.com/__/auth/handler";
    private const string authorizationEndpoint = "https://accounts.google.com/o/oauth2/auth";
    private const string tokenEndpoint = "https://oauth2.googleapis.com/token";
    private const string scope = "email%20profile";

    //https://chat.openai.com/c/e2745705-fb02-4669-9261-4e1ae30daaf5
    private const string authorizationEndpoint_second = "https://accounts.google.com/o/oauth2/auth";
    private const string redirectUri_second = "https://capstoneproject-8992c.firebaseapp.com/__/auth/handler"; // 리디렉션 URI 설정
    private const string new_redirectUri = "https://hallym.capstone.photon.firebaseapp.com/__/auth/handler"; // 리디렉션 URI 설정
    private const string clientId_second = "1049753969677-hfu0873d5sgcjf77dm1nbanqv14bk5g8.apps.googleusercontent.com"; // 클라이언트 ID 설정
    private const string scope_second = "email profile"; // 승인 범위 설정

    private string authUrl_second;

    //redirection uri를 다른 값으로 대체하는 경우 : {{액세스 차단됨: 이 앱의 요청이 잘못되었습니다}}
    //private string normal_google_uri = "https://google.com";

    public void GetGoogleTokens()
    {
        StartCoroutine(OpenOAuthURL());
        //StartCoroutine(OpenLoginWebview());   //rawImage를 활용한 웹뷰는 컨트롤 할 수 없음
    }

    IEnumerator OpenOAuthURL()
    {
        auth = FirebaseAuth.DefaultInstance;

        //redirect URI가 없으면
        //액세스 차단됨: 승인 오류
        //Missing required parameter: redirect_uri 이 오류에 관해 자세히 알아보기
        //400 오류: invalid_request

        //authUrl_second = $"{authorizationEndpoint_second}?response_type=code&client_id={clientId_second}";
        authUrl_second = $"{authorizationEndpoint_second}?response_type=code&client_id={clientId_second}&redirect_uri={new_redirectUri}&scope={scope_second}";

        // Google 로그인 페이지로 이동
        Application.OpenURL(authUrl_second);

        // 로그인 후에 인증 코드를 수신할 때까지 대기
        //// 여기 경로는 OAuth 2.0에 지정된 redirect uri가 아니어도 잘못된 request라는 오류가 발생하지 않음 
        // Unity에서 현재 실행 중인 애플리케이션의 URL이 주어진 문자열로 시작하는지 여부를 확인하는 함수

        while (!Application.absoluteURL.StartsWith(new_redirectUri))
        {
            Debug.Log("Application.absoluteURL : " + Application.absoluteURL);
            //Debug.Log("Application.temporaryCachePath : " + Application.temporaryCachePath);
            //Debug.Log("Application.identifier : " + Application.identifier);            //hallym.capstone.photon
            //Debug.Log("Application.cloudProjectId : " + Application.cloudProjectId);    //a7a5451d-c406-4b70-80a9-007d670f7277
            

            yield return new WaitForSeconds(1);
        }

        // 인증 코드 추출
        string authCode = GetAuthCodeFromUrl(Application.absoluteURL);

        // 인증 코드를 사용하여 토큰 요청 등의 작업 수행
        Debug.Log("Received auth code: " + authCode);

        MakeDB_GoogleSocial();
    }

    private string GetAuthCodeFromUrl(string url)
    {
        string[] urlParts = url.Split('?');
        string[] queryParams = urlParts[1].Split('&');
        string authCode = "";
        foreach (string param in queryParams)
        {
            if (param.StartsWith("code="))
            {
                authCode = param.Split('=')[1];
                break;
            }
        }
        return authCode;
    }


    //https://chat.openai.com/share/6c741b14-c6ab-41de-aecb-01658a21903b
    private IEnumerator GetTokensCoroutine()
    {
        // Google OAuth 2.0 인증 요청 URL 생성
        string authUrl = $"{authorizationEndpoint}?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope={scope}";

        // 여기서는 웹뷰를 사용하여 사용자에게 Google 로그인 페이지를 보여줌
        // 사용자가 로그인 후에 authorization code를 얻은 상황을 가정하여 코드를 작성합니다
        // 사용자가 로그인 후에 얻은 authorization code를 authCode 변수에 할당합니다
        string authCode = "YOUR_AUTHORIZATION_CODE";

        // 인증 코드 (authorization code)를 이용하여 엑세스 토큰 및 ID 토큰 요청
        string tokenRequestParams = $"code={authCode}&client_id={clientId}&client_secret={clientSecret}&redirect_uri={redirectUri}&grant_type=authorization_code";
        UnityWebRequest tokenRequest = UnityWebRequest.Post(tokenEndpoint, tokenRequestParams);
        tokenRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

        yield return tokenRequest.SendWebRequest();

        if (tokenRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to request Google tokens: " + tokenRequest.error);
            yield break;
        }

        string responseJson = tokenRequest.downloadHandler.text;
        GoogleTokenResponse tokenResponse = JsonUtility.FromJson<GoogleTokenResponse>(responseJson);

        string accessToken = tokenResponse.access_token;
        string idToken = tokenResponse.id_token;

        Debug.Log("Access Token: " + accessToken);
        Debug.Log("ID Token: " + idToken);

        MakeDB_GoogleSocial();
    }

    // Google OAuth 2.0 토큰 응답을 파싱하기 위한 클래스
    [System.Serializable]
    private class GoogleTokenResponse
    {
        public string access_token;
        public string id_token;
    }

    public string GetUserId()
    {
        string userId = authEmail;

        // Firebase 초기화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                // 사용자 인증 상태 확인
                FirebaseUser user = auth.CurrentUser;
                if (user != null)
                {
                    // 사용자가 인증되었으며, 식별자를 얻습니다.
                    userId = user.UserId;
                    Debug.Log("User ID: " + userId);
                }
                else
                {
                    // 사용자가 인증되지 않았습니다.
                    Debug.Log("User is not authenticated.");
                }
            }
            else
            {
                Debug.LogError("Failed to initialize Firebase: " + task.Exception);
            }
        });

        return userId;
    }

    public void MakeDB_GoogleSocial()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.Log("failed makeDB : user is null");
            return;
        }

        Debug.Log("processing makeDB");

        //이미 데이터가 존재하는 경우는 바로 return


        // Firestore 인스턴스 생성
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;



        // 데이터를 저장할 컬렉션과 문서 참조
        CollectionReference coll_userdata = db.Collection(GetUserId());

        DocumentReference doc_userdata = coll_userdata.Document("User_Data");
        DocumentReference doc_skill = coll_userdata.Document("Skill");


        // 저장할 데이터 생성
        Dictionary<string, object> dungeonProgress = new Dictionary<string, object>
        {
            { "first", 0 },
            { "second", 0 }
        };

        //근접 캐릭터 스킬 정보
        Dictionary<string, object> char_skill_Melee = new Dictionary<string, object>
        {
            { "melee_skill_01", "0" },
            { "melee_skill_02", "0" },
            { "melee_skill_03", "0" },
            { "melee_skill_04", "0" },
            { "melee_skill_05", "0" }
        };

        //원거리 캐릭터 스킬 정보
        Dictionary<string, object> char_skill_Ranged = new Dictionary<string, object>
        {
            { "range_skill_01", "0" },
            { "range_skill_02", "0" },
            { "range_skill_03", "0" },
            { "range_skill_04", "0" },
            { "range_skill_05", "0" }
        };

        //유저 귀속 스킬 정보
        Dictionary<string, object> skillData = new Dictionary<string, object>
        {
            { "Melee", char_skill_Melee },
            { "Range", char_skill_Ranged }
        };

        //유저의 컬랙션
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "isGoogleSocial", true },
            { "userName", "projecting name" },
            { "email", GetUserId() },
            { "photon_ID", "rewrite photon id" },
            { "dungeon_progress", dungeonProgress },
        };


        // 데이터를 Firestore에 저장
        doc_userdata.SetAsync(userData);
        doc_skill.SetAsync(skillData);
    }
    
    public void MakeDB()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.Log("failed makeDB : user is null");
            return;
        }

        Debug.Log("processing makeDB");

        //이미 데이터가 존재하는 경우는 바로 return


        // Firestore 인스턴스 생성
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;



        // 데이터를 저장할 컬렉션과 문서 참조
        CollectionReference coll_userdata = db.Collection(GetUserId());

        DocumentReference doc_userdata = coll_userdata.Document("User_Data");
        DocumentReference doc_skill = coll_userdata.Document("Skill");


        // 저장할 데이터 생성
        Dictionary<string, object> dungeonProgress = new Dictionary<string, object>
        {
            { "first", 0 },
            { "second", 0 }
        };

        //근접 캐릭터 스킬 정보
        Dictionary<string, object> char_skill_Melee = new Dictionary<string, object>
        {
            { "melee_skill_01", "0" },
            { "melee_skill_02", "0" },
            { "melee_skill_03", "0" },
            { "melee_skill_04", "0" },
            { "melee_skill_05", "0" }
        };

        //원거리 캐릭터 스킬 정보
        Dictionary<string, object> char_skill_Ranged = new Dictionary<string, object>
        {
            { "range_skill_01", "0" },
            { "range_skill_02", "0" },
            { "range_skill_03", "0" },
            { "range_skill_04", "0" },
            { "range_skill_05", "0" }
        };

        //유저 귀속 스킬 정보
        Dictionary<string, object> skillData = new Dictionary<string, object>
        {
            { "Melee", char_skill_Melee },
            { "Range", char_skill_Ranged }
        };

        //유저의 컬랙션
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "userName", "projecting name" },
            { "email", GetUserId() },
            { "photon_ID", "rewrite photon id" },
            { "dungeon_progress", dungeonProgress },
        };


        // 데이터를 Firestore에 저장
        doc_userdata.SetAsync(userData);
        doc_skill.SetAsync(skillData);
    }
}
