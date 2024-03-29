using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using System.Web;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UserData : MonoBehaviour
{
    private static FirebaseAuth auth;
    private static string authEmail;
    //private string authPassword = "asdf";
    //�ȵ���̵� �� ���� //https://www.youtube.com/watch?v=AiXIAe6on5M&t=563s
    
    public static bool CanEnter()
    {
        return auth != null;
    }

    public static async void RegisterWithEmail_Password(string email, string password, Dictionary<string, object> initData = null)
    {
        auth = FirebaseAuth.DefaultInstance;
        authEmail = email;

        await auth.CreateUserWithEmailAndPasswordAsync(email, password);
        await auth.SignInWithEmailAndPasswordAsync(email, password);

        MakeDB(initData);

        Button btn_photon = GameObject.Find("Submit")?.GetComponent<Button>();
        if (btn_photon != null)
        {
            btn_photon.onClick.Invoke();
        }
    }

    public void SigninWithEmail()
    {
        SigninWithEmailAsync();
    }

    public async void SigninWithEmailAsync()
    {
        auth = FirebaseAuth.DefaultInstance;

        string email = GameObject.Find("InputEmaiil").GetComponentInChildren<Text>().text;
        string password = GameObject.Find("InputPassword").GetComponentInChildren<Text>().text;

        await auth.SignInWithEmailAndPasswordAsync(email, password);
        await SetNickname();

        Button btn_photon = GameObject.Find("Submit")?.GetComponent<Button>();
        if (btn_photon != null)
        {
            btn_photon.onClick.Invoke();
        }
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

    public async Task SetNickname()
    {
        string name = "";
        // Firebase ���� �� Firestore �ʱ�ȭ
        auth = FirebaseAuth.DefaultInstance;
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // ���� ����� ��������
        FirebaseUser user = auth.CurrentUser;

        // ���� ����ڰ� �α��εǾ� �ִ��� Ȯ��
        if (user != null)
        {
            // ���� ������� UID ��������
            string uid = user.UserId;
            
            // Firestore���� ���� ������� ������ ����
            DocumentReference docRef = db.Collection(authEmail).Document("User_Data");

            // ���� �б�
            await docRef.GetSnapshotAsync().ContinueWith(task =>
            {
                if (task.IsCompleted)
                {
                    DocumentSnapshot snapshot = task.Result;
                    if (snapshot.Exists)
                    {
                        Debug.Log("Document data: " + snapshot.ToDictionary()["userName"]);
                        name = snapshot.ToDictionary()["userName"].ToString();
                        PhotonNetwork.NickName = name;
                    }
                    else
                    {
                        Debug.Log("Document does not exist!");
                    }
                }
            });
        }
        else
        {
            Debug.Log("No user is currently logged in.");
        }
    }

    /*
    public void SignInWithGoogle()
    {
        auth = FirebaseAuth.DefaultInstance;

        if (auth == null)
        {
            Debug.LogError("Firebase Auth is not initialized.");
            return;
        }

        // Google �α��� �˾� ǥ��
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

            // �α��� ����
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

            // �α��� ����
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
    private const string new_redirectUri = "https://hallym.capstone.photon.firebaseapp.com/__/auth/handler"; // ���𷺼� URI ����
    private const string clientId_second = "1049753969677-hfu0873d5sgcjf77dm1nbanqv14bk5g8.apps.googleusercontent.com"; // Ŭ���̾�Ʈ ID ����
    private const string scope_second = "email profile"; // ���� ���� ����

    private string authUrl_second;

    //redirection uri�� �ٸ� ������ ��ü�ϴ� ��� : {{�׼��� ���ܵ�: �� ���� ��û�� �߸��Ǿ����ϴ�}}
    //private string normal_google_uri = "https://google.com";

    public void GetGoogleTokens()
    {
        //StartCoroutine(OpenOAuthURL());
        //StartCoroutine(GetAuthorizationCode());
        GenerateCustomToken();
        //StartCoroutine(OpenOAuthURL3());
        //StartCoroutine(SignInWithGoogle2());
        //StartCoroutine(OpenOAuthURL2());
        //StartCoroutine(SignInWithGoogleCoroutine2());
        //StartCoroutine(SignInWithGoogleCoroutine());  //memory leak & 403 forbidden
        //StartCoroutine(SigninAnonymously());    //�͸� �α����� ���������� ����
        //StartCoroutine(OpenLoginWebview());   //rawImage�� Ȱ���� ����� ��Ʈ�� �� �� ����
    }
    */

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            //Debug.Log($"task : {task}\n{task.Result}");     //System.Threading.Tasks.UnwrapPromise`1[Firebase.DependencyStatus]     //Available

            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
                //Debug.Log("init auth");
            }
            else
            {
                Debug.LogError("Could not resolve Firebase dependencies: " + task.Result);
                return;
            }
        });
    }

    /*
    public void GenerateCustomToken()
    {
        string googleClientId = clientId_second; // Google Cloud Console���� ������ OAuth 2.0 Ŭ���̾�Ʈ ID

        Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(googleClientId, null);
        Debug.Log("end of credential");

        auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
        {
            Debug.Log($"task\n{task}\n{task.Result}");

            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Google sign-in failed: " + task.Exception);
                return;
            }

            // Google �α��� ����
            Firebase.Auth.FirebaseUser user = task.Result;
            Debug.Log("Firebase user signed in: " + user.DisplayName + " (ID: " + user.UserId + ")");
        });

        
    }

    IEnumerator GetAuthorizationCode()
    {
        //string url = $"https://accounts.google.com/o/oauth2/auth?client_id={clientId_second}&redirect_uri=urn:ietf:wg:oauth:2.0:oob&response_type=code&scope=email%20profile";
        string url = $"{authorizationEndpoint_second}?response_type=code&client_id={clientId_second}&scope={scope_second}";

        // ������ ���� �α��� �������� ���𷺼�
        Application.OpenURL(url);

        // ����ڰ� ���ۿ��� �α����ϰ� ���� �ο��� �ϰ� �� ��, ���� �ڵ带 �����ϱ� ���� ���
        while (true)
        {
            Debug.Log("in while");

            if (!Application.absoluteURL.StartsWith("urn:ietf:wg:oauth:2.0:oob"))
            {
                yield return new WaitForSeconds(1);
            }
            else
            {
                break;
            }
        }
        Debug.Log("escape while");

        // ���� �ڵ� ����
        string authorizationCode = Application.absoluteURL.Substring(Application.absoluteURL.IndexOf("code=") + 5);

        // �׼��� ��ū ��û
        yield return new WaitForSeconds(1);
        //yield return StartCoroutine(GetAccessToken(authorizationCode));
    }

    IEnumerator OpenOAuthURL3()
    {
        auth = FirebaseAuth.DefaultInstance;

        authUrl_second = $"{authorizationEndpoint_second}?response_type=code&client_id={clientId_second}&redirect_uri={new_redirectUri}&scope={scope_second}";

        // Google �α��� �������� �̵�
        Application.OpenURL(authUrl_second);

        while(true)
        {
            Debug.Log("abs url" + Application.absoluteURL);
            Debug.Log("data path" + Application.dataPath);

            // �� �������� ���� GET ��û ����
            UnityWebRequest request = UnityWebRequest.Get(authUrl_second);

            // ��û ������
            yield return request.SendWebRequest();

            // ��û�� ���������� �Ϸ�Ǿ����� Ȯ��
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to open web page: " + request.error);
                yield break;
            }

            // �� �������� ���� ���
            //Debug.Log("Web page content: " + request.downloadHandler.text);
        }

        
    }

    //try02
    IEnumerator SignInWithGoogle2()
    {
        auth = FirebaseAuth.DefaultInstance;

        // ����ڰ� ���� �α��� �������� �̵��ϵ��� URL�� �����մϴ�.
        string authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope={scope_second}";

        // ���Ȼ��� ������ �� �並 ����Ͽ� ���� �α����� �����մϴ�.
        // ���� �α��� �������� �̵�
        Application.OpenURL(authUrl);



        yield return new WaitForSeconds(1);
    }

    // ���𷺼� URI���� ���� �ڵ带 �ް� Firebase�� �α����ϴ� �Լ�
    public void HandleGoogleSignInCallback(string authorizationCode)
    {
        Debug.Log("authorizationCode : " + authorizationCode);

        StartCoroutine(ExchangeAuthorizationCodeForAccessToken(authorizationCode));
    }

    IEnumerator ExchangeAuthorizationCodeForAccessToken(string authorizationCode)
    {
        // ���� �ڵ带 ����Ͽ� �׼��� ��ū�� ��û�մϴ�.
        string tokenEndpoint = "https://oauth2.googleapis.com/token";
        WWWForm form = new WWWForm();
        form.AddField("code", authorizationCode);
        form.AddField("client_id", clientId);
        form.AddField("client_secret", clientSecret);
        form.AddField("redirect_uri", redirectUri);
        form.AddField("grant_type", "authorization_code");

        using (WWW www = new WWW(tokenEndpoint, form))
        {
            yield return www;

            if (www.error != null)
            {
                Debug.LogError("Error exchanging authorization code for access token: " + www.error);
            }
            else
            {
                // ���� ���� JSON�� �Ľ��Ͽ� �׼��� ��ū ����
                Dictionary<string, object> responseData = Google.MiniJSON.Json.Deserialize(www.text) as Dictionary<string, object>;
                string accessToken = responseData["access_token"] as string;

                // Firebase�� �׼��� ��ū�� ����Ͽ� �α����մϴ�.
                Credential credential = GoogleAuthProvider.GetCredential(accessToken, null);
                auth.SignInWithCredentialAsync(credential).ContinueWith(task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        Debug.LogError("Firebase authentication failed: " + task.Exception);
                    }
                    else
                    {
                        FirebaseUser user = task.Result;
                        Debug.Log("Firebase authentication successful! User ID: " + user.UserId);
                    }
                });
            }
        }
    }

    //try01
    // Google �α����� ���� Coroutine
    IEnumerator OpenOAuthURL2()
    {
        auth = FirebaseAuth.DefaultInstance;

        authUrl_second = $"{authorizationEndpoint_second}?response_type=code&client_id={clientId_second}&redirect_uri={new_redirectUri}&scope={scope_second}";

        // Google �α��� �������� �̵�
        Application.OpenURL(authUrl_second);

        while(true)
        {
            // URL���� ���� ��Ʈ���� �����մϴ�.
            NameValueCollection queryString = HttpUtility.ParseQueryString(new Uri(new_redirectUri).Query);

            // "code" �Ű������� ����Ͽ� ���� �ڵ带 �����ɴϴ�.
            string authorizationCode = queryString["code"];

            if (!string.IsNullOrEmpty(authorizationCode))
            {
                // ���⼭ ���� �ڵ带 ����Ͽ� Firebase�� �α����ϴ� ���� �۾��� �����մϴ�.
                Debug.Log("Authorization Code: " + authorizationCode);
            }
            else
            {
                Debug.LogError("Authorization Code Not Found.");
            }

            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);
    }

    IEnumerator OpenOAuthURL()
    {
        auth = FirebaseAuth.DefaultInstance;

        var credential = GoogleAuthProvider.GetCredential(clientId, clientSecret);
        //var signInTask = auth.SignInWithCredentialAsync(credential);

        //redirect URI�� ������
        //�׼��� ���ܵ�: ���� ����
        //Missing required parameter: redirect_uri �� ������ ���� �ڼ��� �˾ƺ���
        //400 ����: invalid_request

        //authUrl_second = $"{authorizationEndpoint_second}?response_type=code&client_id={clientId_second}";
        authUrl_second = $"{authorizationEndpoint_second}?response_type=code&client_id={clientId_second}&redirect_uri={new_redirectUri}&scope={scope_second}";

        // Google �α��� �������� �̵�
        Application.OpenURL(authUrl_second);

        // �α��� �Ŀ� ���� �ڵ带 ������ ������ ���
        //// ���� ��δ� OAuth 2.0�� ������ redirect uri�� �ƴϾ �߸��� request��� ������ �߻����� ���� 
        // Unity���� ���� ���� ���� ���ø����̼��� URL�� �־��� ���ڿ��� �����ϴ��� ���θ� Ȯ���ϴ� �Լ�

        while (!Application.absoluteURL.StartsWith(new_redirectUri))
        {
            credential = GoogleAuthProvider.GetCredential(clientId, clientSecret);

            Debug.Log("Application.absoluteURL : " + Application.absoluteURL);
            Debug.Log("credential : " + credential);
            Debug.Log("credential : " + credential.Provider);   //google.com
            //Debug.Log("Application.temporaryCachePath : " + Application.temporaryCachePath);
            //Debug.Log("Application.identifier : " + Application.identifier);            //hallym.capstone.photon
            //Debug.Log("Application.cloudProjectId : " + Application.cloudProjectId);    //a7a5451d-c406-4b70-80a9-007d670f7277
            Debug.Log("get data : " + AppContext.GetData(clientId));
            Debug.Log("domain : " + AppDomain.CurrentDomain);   //Unity Child Domain

            yield return new WaitForSeconds(1);
        }

        // ���� �ڵ� ����
        string authCode = GetAuthCodeFromUrl(Application.absoluteURL);

        // ���� �ڵ带 ����Ͽ� ��ū ��û ���� �۾� ����
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
    */

    /*
    private IEnumerator SigninCustomToken()
    {
        auth = FirebaseAuth.DefaultInstance;
        string customToken = "YOUR_CUSTOM_TOKEN_HERE";

        // ����� ���� ��ū���� �α��� �õ�
        var authTask = auth.SignInWithCustomTokenAsync(customToken);
        yield return new WaitUntil(() => authTask.IsCompleted);

        if (authTask.Exception != null)
        {
            // ���� ������ ���
            Debug.LogError("Failed to sign in with custom token: " + authTask.Exception.Message);
        }
        else
        {
            // ������ ������ ���
            var authResult = authTask.Result;
            Debug.Log("Signed in with custom token with UID: " + authResult.User.UserId);
        }
    }
    */
    /*
    private IEnumerator SignInWithGoogleCoroutine()
    {
        string webApiKey = "AIzaSyA0iuKe5o2kge6nz2zHtysWeT1PCUEhWhQ";
        string googleLoginUrl;
        string accessToken;

        // �� ��û�� ���� URL
        string url = $"https://www.googleapis.com/identitytoolkit/v3/relyingparty/getProjectConfig?key={webApiKey}";

        // �� ��û ������
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        // ��û�� �����ߴ��� Ȯ��
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to get project config: " + www.error);
            yield break;
        }
        Debug.Log("request Result is Success : " + www);

        // ���� ������Ʈ �������� Web API Ű ��������
        webApiKey = JsonUtility.FromJson<ProjectConfigResponse>(www.downloadHandler.text).webApiKey;
        Debug.Log("webApiKey : " + webApiKey);

        // Google �α��� URL ����
        googleLoginUrl = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyAssertion?key=" + webApiKey;

        // ����ڰ� �α����� ���� ������ �׼��� ��ū ��������
        accessToken = "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"; // ���⿡ ������� ���� �׼��� ��ū�� �����ؾ� �մϴ�.

        string requestUri = "https://hallym.capstone.photon.firebaseapp.com/";

        // Google ���� ��ū ��û ������ ����
        GoogleAuthRequestData requestData = new GoogleAuthRequestData
        {
            postBody = "{\"postBody\":\"id_token=" + accessToken + "&providerId=google.com\",\"requestUri\":\"" + requestUri + "\",\"returnIdpCredential\":true,\"returnSecureToken\":true}"
        };

        // Google ���� ��ū ��û ������
        www = new UnityWebRequest(googleLoginUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(requestData));
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        // ��û�� �����ߴ��� Ȯ��
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to get Google authentication token: " + www.error);
            yield break;
        }

        // Google ���� ��� ������ ��������
        GoogleAuthResponseData responseData = JsonUtility.FromJson<GoogleAuthResponseData>(www.downloadHandler.text);

        // FirebaseAuth�� Google ���� ��ū �����Ͽ� ����
        var authTask = auth.SignInWithCredentialAsync(GoogleAuthProvider.GetCredential(responseData.idToken, null));
        yield return new WaitUntil(() => authTask.IsCompleted);

        if (authTask.Exception != null)
        {
            Debug.LogError("Firebase sign-in failed: " + authTask.Exception.Message);
            yield break;
        }

        // ������ ����� ���� ��������
        FirebaseUser user = authTask.Result;

        // ������ ������� ���� ���
        //statusText.text = "User signed in successfully: " + user.DisplayName + " (UID: " + user.UserId + ")";
    }

    // ���� ������Ʈ ���� ���� Ŭ����
    [System.Serializable]
    private class ProjectConfigResponse
    {
        public string webApiKey;
    }

    // Google ���� ��û ������ Ŭ����
    [System.Serializable]
    private class GoogleAuthRequestData
    {
        public string postBody;
    }

    // Google ���� ���� ������ Ŭ����
    [System.Serializable]
    private class GoogleAuthResponseData
    {
        public string idToken;
    }

    //�͸� �α��� ���� ����
    private IEnumerator SigninAnonymously()
    {
        auth = FirebaseAuth.DefaultInstance;

        var authTask = auth.SignInAnonymouslyAsync();
        yield return new WaitUntil(() => authTask.IsCompleted);

        if (authTask.Exception != null)
        {
            // ���� ������ ���
            Debug.LogError("Failed to sign in anonymously: " + authTask.Exception.Message);
        }
        else
        {
            // ������ ������ ���
            var authResult = authTask.Result;
            Debug.Log("Signed in anonymously with UID: " + authResult.User.UserId);
        }
    }

    //https://chat.openai.com/share/6c741b14-c6ab-41de-aecb-01658a21903b
    private IEnumerator GetTokensCoroutine()
    {
        // Google OAuth 2.0 ���� ��û URL ����
        string authUrl = $"{authorizationEndpoint}?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope={scope}";

        // ���⼭�� ���並 ����Ͽ� ����ڿ��� Google �α��� �������� ������
        // ����ڰ� �α��� �Ŀ� authorization code�� ���� ��Ȳ�� �����Ͽ� �ڵ带 �ۼ��մϴ�
        // ����ڰ� �α��� �Ŀ� ���� authorization code�� authCode ������ �Ҵ��մϴ�
        string authCode = "YOUR_AUTHORIZATION_CODE";

        // ���� �ڵ� (authorization code)�� �̿��Ͽ� ������ ��ū �� ID ��ū ��û
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

    // Google OAuth 2.0 ��ū ������ �Ľ��ϱ� ���� Ŭ����
    [System.Serializable]
    private class GoogleTokenResponse
    {
        public string access_token;
        public string id_token;
    }
    */

    public static string GetUserId()
    {
        string userId = authEmail;

        // Firebase �ʱ�ȭ
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                // ����� ���� ���� Ȯ��
                FirebaseUser user = auth.CurrentUser;
                if (user != null)
                {
                    // ����ڰ� �����Ǿ�����, �ĺ��ڸ� ����ϴ�.
                    userId = user.UserId;
                    Debug.Log("User ID: " + userId);
                }
                else
                {
                    // ����ڰ� �������� �ʾҽ��ϴ�.
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
    
    /*
    public void MakeDB_GoogleSocial()
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.Log("failed makeDB : user is null");
            return;
        }

        Debug.Log("processing makeDB");

        //�̹� �����Ͱ� �����ϴ� ���� �ٷ� return


        // Firestore �ν��Ͻ� ����
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;



        // �����͸� ������ �÷��ǰ� ���� ����
        CollectionReference coll_userdata = db.Collection(GetUserId());

        DocumentReference doc_userdata = coll_userdata.Document("User_Data");
        DocumentReference doc_skill = coll_userdata.Document("Skill");


        // ������ ������ ����
        Dictionary<string, object> dungeonProgress = new Dictionary<string, object>
        {
            { "first", 0 },
            { "second", 0 }
        };

        //���� ĳ���� ��ų ����
        Dictionary<string, object> char_skill_Melee = new Dictionary<string, object>
        {
            { "melee_skill_01", "0" },
            { "melee_skill_02", "0" },
            { "melee_skill_03", "0" },
            { "melee_skill_04", "0" },
            { "melee_skill_05", "0" }
        };

        //���Ÿ� ĳ���� ��ų ����
        Dictionary<string, object> char_skill_Ranged = new Dictionary<string, object>
        {
            { "range_skill_01", "0" },
            { "range_skill_02", "0" },
            { "range_skill_03", "0" },
            { "range_skill_04", "0" },
            { "range_skill_05", "0" }
        };

        //���� �ͼ� ��ų ����
        Dictionary<string, object> skillData = new Dictionary<string, object>
        {
            { "Melee", char_skill_Melee },
            { "Range", char_skill_Ranged }
        };

        //������ �÷���
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "isGoogleSocial", true },
            { "userName", "projecting name" },
            { "email", GetUserId() },
            { "photon_ID", "rewrite photon id" },
            { "dungeon_progress", dungeonProgress },
        };


        // �����͸� Firestore�� ����
        doc_userdata.SetAsync(userData);
        doc_skill.SetAsync(skillData);
    }
    */
    
    public static void MakeDB(Dictionary<string, object> param)
    {
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.Log("failed makeDB : user is null");
            return;
        }

        Debug.Log("processing makeDB");

        //�̹� �����Ͱ� �����ϴ� ���� �ٷ� return


        // Firestore �ν��Ͻ� ����
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;



        // �����͸� ������ �÷��ǰ� ���� ����
        CollectionReference coll_userdata = db.Collection(GetUserId());

        DocumentReference doc_userdata = coll_userdata.Document("User_Data");
        DocumentReference doc_skill = coll_userdata.Document("Skill");


        // ������ ������ ����
        Dictionary<string, object> dungeonProgress = new Dictionary<string, object>
        {
            { "first", 0 },
            { "second", 0 }
        };

        //���� ĳ���� ��ų ����
        Dictionary<string, object> char_skill_Melee = new Dictionary<string, object>
        {
            { "melee_skill_01", "0" },
            { "melee_skill_02", "0" },
            { "melee_skill_03", "0" },
            { "melee_skill_04", "0" },
            { "melee_skill_05", "0" }
        };

        //���Ÿ� ĳ���� ��ų ����
        Dictionary<string, object> char_skill_Ranged = new Dictionary<string, object>
        {
            { "range_skill_01", "0" },
            { "range_skill_02", "0" },
            { "range_skill_03", "0" },
            { "range_skill_04", "0" },
            { "range_skill_05", "0" }
        };

        //���� �ͼ� ��ų ����
        Dictionary<string, object> skillData = new Dictionary<string, object>
        {
            { "Melee", char_skill_Melee },
            { "Range", char_skill_Ranged }
        };

        //������ �÷���
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "userName", param["nickname"] != null ? param["nickname"] : "null" },
            { "email", GetUserId() },
            { "photon_ID", "rewrite photon id" },
            { "dungeon_progress", dungeonProgress },
        };


        // �����͸� Firestore�� ����
        doc_userdata.SetAsync(userData);
        doc_skill.SetAsync(skillData);
    }
}
