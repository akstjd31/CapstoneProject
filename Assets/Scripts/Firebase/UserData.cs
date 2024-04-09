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
    //안드로이드 디지털 지문 //https://www.youtube.com/watch?v=AiXIAe6on5M&t=563s
    
    public static bool CanEnter()
    {
        return auth != null;
    }

    public static async void RegisterWithEmail_Password(string email, string password, Dictionary<string, object> initData = null)
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        //회원가입
        auth.CreateUserWithEmailAndPasswordAsync(email, password);
        //로그인
        await auth.SignInWithEmailAndPasswordAsync(email, password);

        //auth = FirebaseAuth.DefaultInstance;
        //authEmail = email;

        //await auth.CreateUserWithEmailAndPasswordAsync(email, password);
        //await auth.SignInWithEmailAndPasswordAsync(email, password);

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
        // Firebase 占쏙옙占쏙옙 占쏙옙 Firestore 占십깍옙화
        auth = FirebaseAuth.DefaultInstance;
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // 占쏙옙占쏙옙 占쏙옙占쏙옙占 占쏙옙占쏙옙占쏙옙占쏙옙
        FirebaseUser user = auth.CurrentUser;

        // 占쏙옙占쏙옙 占쏙옙占쏙옙微占 占싸깍옙占싸되억옙 占쌍댐옙占쏙옙 확占쏙옙
        if (user != null)
        {
            // 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙占 UID 占쏙옙占쏙옙占쏙옙占쏙옙
            string uid = user.UserId;
            
            // Firestore占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙占 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙
            DocumentReference docRef = db.Collection(authEmail).Document("User_Data");

            // 占쏙옙占쏙옙 占싻깍옙
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

        // Google 占싸깍옙占쏙옙 占싯억옙 표占쏙옙
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

            // 占싸깍옙占쏙옙 占쏙옙占쏙옙
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

            // 占싸깍옙占쏙옙 占쏙옙占쏙옙
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
    private const string new_redirectUri = "https://hallym.capstone.photon.firebaseapp.com/__/auth/handler"; // 占쏙옙占쏜렉쇽옙 URI 占쏙옙占쏙옙
    private const string clientId_second = "1049753969677-hfu0873d5sgcjf77dm1nbanqv14bk5g8.apps.googleusercontent.com"; // 클占쏙옙占싱억옙트 ID 占쏙옙占쏙옙
    private const string scope_second = "email profile"; // 占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙

    private string authUrl_second;

    //redirection uri占쏙옙 占쌕몌옙 占쏙옙占쏙옙占쏙옙 占쏙옙체占싹댐옙 占쏙옙占 : {{占쌓쇽옙占쏙옙 占쏙옙占쌤듸옙: 占쏙옙 占쏙옙占쏙옙 占쏙옙청占쏙옙 占쌩몌옙占실억옙占쏙옙占싹댐옙}}
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
        //StartCoroutine(SigninAnonymously());    //占싶몌옙 占싸깍옙占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙
        //StartCoroutine(OpenLoginWebview());   //rawImage占쏙옙 활占쏙옙占쏙옙 占쏙옙占쏙옙占 占쏙옙트占쏙옙 占쏙옙 占쏙옙 占쏙옙占쏙옙
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
        string googleClientId = clientId_second; // Google Cloud Console占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 OAuth 2.0 클占쏙옙占싱억옙트 ID

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

            // Google 占싸깍옙占쏙옙 占쏙옙占쏙옙
            Firebase.Auth.FirebaseUser user = task.Result;
            Debug.Log("Firebase user signed in: " + user.DisplayName + " (ID: " + user.UserId + ")");
        });

        
    }

    IEnumerator GetAuthorizationCode()
    {
        //string url = $"https://accounts.google.com/o/oauth2/auth?client_id={clientId_second}&redirect_uri=urn:ietf:wg:oauth:2.0:oob&response_type=code&scope=email%20profile";
        string url = $"{authorizationEndpoint_second}?response_type=code&client_id={clientId_second}&scope={scope_second}";

        // 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙 占싸깍옙占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙 占쏙옙占쏜렉쇽옙
        Application.OpenURL(url);

        // 占쏙옙占쏙옙微占 占쏙옙占쌜울옙占쏙옙 占싸깍옙占쏙옙占싹곤옙 占쏙옙占쏙옙 占싸울옙占쏙옙 占싹곤옙 占쏙옙 占쏙옙, 占쏙옙占쏙옙 占쌘드를 占쏙옙占쏙옙占싹깍옙 占쏙옙占쏙옙 占쏙옙占
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

        // 占쏙옙占쏙옙 占쌘듸옙 占쏙옙占쏙옙
        string authorizationCode = Application.absoluteURL.Substring(Application.absoluteURL.IndexOf("code=") + 5);

        // 占쌓쇽옙占쏙옙 占쏙옙큰 占쏙옙청
        yield return new WaitForSeconds(1);
        //yield return StartCoroutine(GetAccessToken(authorizationCode));
    }

    IEnumerator OpenOAuthURL3()
    {
        auth = FirebaseAuth.DefaultInstance;

        authUrl_second = $"{authorizationEndpoint_second}?response_type=code&client_id={clientId_second}&redirect_uri={new_redirectUri}&scope={scope_second}";

        // Google 占싸깍옙占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙 占싱듸옙
        Application.OpenURL(authUrl_second);

        while(true)
        {
            Debug.Log("abs url" + Application.absoluteURL);
            Debug.Log("data path" + Application.dataPath);

            // 占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙 GET 占쏙옙청 占쏙옙占쏙옙
            UnityWebRequest request = UnityWebRequest.Get(authUrl_second);

            // 占쏙옙청 占쏙옙占쏙옙占쏙옙
            yield return request.SendWebRequest();

            // 占쏙옙청占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙占쏙옙 占싹뤄옙퓸占쏙옙占쏙옙占 확占쏙옙
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to open web page: " + request.error);
                yield break;
            }

            // 占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙占
            //Debug.Log("Web page content: " + request.downloadHandler.text);
        }

        
    }

    //try02
    IEnumerator SignInWithGoogle2()
    {
        auth = FirebaseAuth.DefaultInstance;

        // 占쏙옙占쏙옙微占 占쏙옙占쏙옙 占싸깍옙占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙 占싱듸옙占싹듸옙占쏙옙 URL占쏙옙 占쏙옙占쏙옙占쌌니댐옙.
        string authUrl = $"https://accounts.google.com/o/oauth2/v2/auth?client_id={clientId}&redirect_uri={redirectUri}&response_type=code&scope={scope_second}";

        // 占쏙옙占싫삼옙占쏙옙 占쏙옙占쏙옙占쏙옙 占쏙옙 占썰를 占쏙옙占쏙옙臼占 占쏙옙占쏙옙 占싸깍옙占쏙옙占쏙옙 占쏙옙占쏙옙占쌌니댐옙.
        // 占쏙옙占쏙옙 占싸깍옙占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙 占싱듸옙
        Application.OpenURL(authUrl);



        yield return new WaitForSeconds(1);
    }

    // 占쏙옙占쏜렉쇽옙 URI占쏙옙占쏙옙 占쏙옙占쏙옙 占쌘드를 占쌨곤옙 Firebase占쏙옙 占싸깍옙占쏙옙占싹댐옙 占쌉쇽옙
    public void HandleGoogleSignInCallback(string authorizationCode)
    {
        Debug.Log("authorizationCode : " + authorizationCode);

        StartCoroutine(ExchangeAuthorizationCodeForAccessToken(authorizationCode));
    }

    IEnumerator ExchangeAuthorizationCodeForAccessToken(string authorizationCode)
    {
        // 占쏙옙占쏙옙 占쌘드를 占쏙옙占쏙옙臼占 占쌓쇽옙占쏙옙 占쏙옙큰占쏙옙 占쏙옙청占쌌니댐옙.
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
                // 占쏙옙占쏙옙 占쏙옙占쏙옙 JSON占쏙옙 占식쏙옙占싹울옙 占쌓쇽옙占쏙옙 占쏙옙큰 占쏙옙占쏙옙
                Dictionary<string, object> responseData = Google.MiniJSON.Json.Deserialize(www.text) as Dictionary<string, object>;
                string accessToken = responseData["access_token"] as string;

                // Firebase占쏙옙 占쌓쇽옙占쏙옙 占쏙옙큰占쏙옙 占쏙옙占쏙옙臼占 占싸깍옙占쏙옙占쌌니댐옙.
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
    // Google 占싸깍옙占쏙옙占쏙옙 占쏙옙占쏙옙 Coroutine
    IEnumerator OpenOAuthURL2()
    {
        auth = FirebaseAuth.DefaultInstance;

        authUrl_second = $"{authorizationEndpoint_second}?response_type=code&client_id={clientId_second}&redirect_uri={new_redirectUri}&scope={scope_second}";

        // Google 占싸깍옙占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙 占싱듸옙
        Application.OpenURL(authUrl_second);

        while(true)
        {
            // URL占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙트占쏙옙占쏙옙 占쏙옙占쏙옙占쌌니댐옙.
            NameValueCollection queryString = HttpUtility.ParseQueryString(new Uri(new_redirectUri).Query);

            // "code" 占신곤옙占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙臼占 占쏙옙占쏙옙 占쌘드를 占쏙옙占쏙옙占심니댐옙.
            string authorizationCode = queryString["code"];

            if (!string.IsNullOrEmpty(authorizationCode))
            {
                // 占쏙옙占썩서 占쏙옙占쏙옙 占쌘드를 占쏙옙占쏙옙臼占 Firebase占쏙옙 占싸깍옙占쏙옙占싹댐옙 占쏙옙占쏙옙 占쌜억옙占쏙옙 占쏙옙占쏙옙占쌌니댐옙.
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

        //redirect URI占쏙옙 占쏙옙占쏙옙占쏙옙
        //占쌓쇽옙占쏙옙 占쏙옙占쌤듸옙: 占쏙옙占쏙옙 占쏙옙占쏙옙
        //Missing required parameter: redirect_uri 占쏙옙 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙 占쌘쇽옙占쏙옙 占싯아븝옙占쏙옙
        //400 占쏙옙占쏙옙: invalid_request

        //authUrl_second = $"{authorizationEndpoint_second}?response_type=code&client_id={clientId_second}";
        authUrl_second = $"{authorizationEndpoint_second}?response_type=code&client_id={clientId_second}&redirect_uri={new_redirectUri}&scope={scope_second}";

        // Google 占싸깍옙占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙 占싱듸옙
        Application.OpenURL(authUrl_second);

        // 占싸깍옙占쏙옙 占식울옙 占쏙옙占쏙옙 占쌘드를 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 占쏙옙占
        //// 占쏙옙占쏙옙 占쏙옙灌占 OAuth 2.0占쏙옙 占쏙옙占쏙옙占쏙옙 redirect uri占쏙옙 占싣니어도 占쌩몌옙占쏙옙 request占쏙옙占 占쏙옙占쏙옙占쏙옙 占쌩삼옙占쏙옙占쏙옙 占쏙옙占쏙옙 
        // Unity占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙占시몌옙占쏙옙占싱쇽옙占쏙옙 URL占쏙옙 占쌍억옙占쏙옙 占쏙옙占쌘울옙占쏙옙 占쏙옙占쏙옙占싹댐옙占쏙옙 占쏙옙占싸몌옙 확占쏙옙占싹댐옙 占쌉쇽옙

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

        // 占쏙옙占쏙옙 占쌘듸옙 占쏙옙占쏙옙
        string authCode = GetAuthCodeFromUrl(Application.absoluteURL);

        // 占쏙옙占쏙옙 占쌘드를 占쏙옙占쏙옙臼占 占쏙옙큰 占쏙옙청 占쏙옙占쏙옙 占쌜억옙 占쏙옙占쏙옙
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

        // 占쏙옙占쏙옙占 占쏙옙占쏙옙 占쏙옙큰占쏙옙占쏙옙 占싸깍옙占쏙옙 占시듸옙
        var authTask = auth.SignInWithCustomTokenAsync(customToken);
        yield return new WaitUntil(() => authTask.IsCompleted);

        if (authTask.Exception != null)
        {
            // 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 占쏙옙占
            Debug.LogError("Failed to sign in with custom token: " + authTask.Exception.Message);
        }
        else
        {
            // 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 占쏙옙占
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

        // 占쏙옙 占쏙옙청占쏙옙 占쏙옙占쏙옙 URL
        string url = $"https://www.googleapis.com/identitytoolkit/v3/relyingparty/getProjectConfig?key={webApiKey}";

        // 占쏙옙 占쏙옙청 占쏙옙占쏙옙占쏙옙
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        // 占쏙옙청占쏙옙 占쏙옙占쏙옙占쌩댐옙占쏙옙 확占쏙옙
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to get project config: " + www.error);
            yield break;
        }
        Debug.Log("request Result is Success : " + www);

        // 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙트 占쏙옙占쏙옙占쏙옙占쏙옙 Web API 키 占쏙옙占쏙옙占쏙옙占쏙옙
        webApiKey = JsonUtility.FromJson<ProjectConfigResponse>(www.downloadHandler.text).webApiKey;
        Debug.Log("webApiKey : " + webApiKey);

        // Google 占싸깍옙占쏙옙 URL 占쏙옙占쏙옙
        googleLoginUrl = "https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyAssertion?key=" + webApiKey;

        // 占쏙옙占쏙옙微占 占싸깍옙占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 占쌓쇽옙占쏙옙 占쏙옙큰 占쏙옙占쏙옙占쏙옙占쏙옙
        accessToken = "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"; // 占쏙옙占썩에 占쏙옙占쏙옙占쏙옙占 占쏙옙占쏙옙 占쌓쇽옙占쏙옙 占쏙옙큰占쏙옙 占쏙옙占쏙옙占쌔억옙 占쌌니댐옙.

        string requestUri = "https://hallym.capstone.photon.firebaseapp.com/";

        // Google 占쏙옙占쏙옙 占쏙옙큰 占쏙옙청 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙
        GoogleAuthRequestData requestData = new GoogleAuthRequestData
        {
            postBody = "{\"postBody\":\"id_token=" + accessToken + "&providerId=google.com\",\"requestUri\":\"" + requestUri + "\",\"returnIdpCredential\":true,\"returnSecureToken\":true}"
        };

        // Google 占쏙옙占쏙옙 占쏙옙큰 占쏙옙청 占쏙옙占쏙옙占쏙옙
        www = new UnityWebRequest(googleLoginUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(JsonUtility.ToJson(requestData));
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        // 占쏙옙청占쏙옙 占쏙옙占쏙옙占쌩댐옙占쏙옙 확占쏙옙
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to get Google authentication token: " + www.error);
            yield break;
        }

        // Google 占쏙옙占쏙옙 占쏙옙占 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙
        GoogleAuthResponseData responseData = JsonUtility.FromJson<GoogleAuthResponseData>(www.downloadHandler.text);

        // FirebaseAuth占쏙옙 Google 占쏙옙占쏙옙 占쏙옙큰 占쏙옙占쏙옙占싹울옙 占쏙옙占쏙옙
        var authTask = auth.SignInWithCredentialAsync(GoogleAuthProvider.GetCredential(responseData.idToken, null));
        yield return new WaitUntil(() => authTask.IsCompleted);

        if (authTask.Exception != null)
        {
            Debug.LogError("Firebase sign-in failed: " + authTask.Exception.Message);
            yield break;
        }

        // 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙
        FirebaseUser user = authTask.Result;

        // 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙占 占쏙옙占쏙옙 占쏙옙占
        //statusText.text = "User signed in successfully: " + user.DisplayName + " (UID: " + user.UserId + ")";
    }

    // 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙트 占쏙옙占쏙옙 占쏙옙占쏙옙 클占쏙옙占쏙옙
    [System.Serializable]
    private class ProjectConfigResponse
    {
        public string webApiKey;
    }

    // Google 占쏙옙占쏙옙 占쏙옙청 占쏙옙占쏙옙占쏙옙 클占쏙옙占쏙옙
    [System.Serializable]
    private class GoogleAuthRequestData
    {
        public string postBody;
    }

    // Google 占쏙옙占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 클占쏙옙占쏙옙
    [System.Serializable]
    private class GoogleAuthResponseData
    {
        public string idToken;
    }

    //占싶몌옙 占싸깍옙占쏙옙 占쏙옙占쏙옙 占쏙옙占쏙옙
    private IEnumerator SigninAnonymously()
    {
        auth = FirebaseAuth.DefaultInstance;

        var authTask = auth.SignInAnonymouslyAsync();
        yield return new WaitUntil(() => authTask.IsCompleted);

        if (authTask.Exception != null)
        {
            // 占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 占쏙옙占
            Debug.LogError("Failed to sign in anonymously: " + authTask.Exception.Message);
        }
        else
        {
            // 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 占쏙옙占
            var authResult = authTask.Result;
            Debug.Log("Signed in anonymously with UID: " + authResult.User.UserId);
        }
    }

    //https://chat.openai.com/share/6c741b14-c6ab-41de-aecb-01658a21903b
    private IEnumerator GetTokensCoroutine()
    {
        // Google OAuth 2.0 占쏙옙占쏙옙 占쏙옙청 URL 占쏙옙占쏙옙
        string authUrl = $"{authorizationEndpoint}?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&scope={scope}";

        // 占쏙옙占썩서占쏙옙 占쏙옙占썰를 占쏙옙占쏙옙臼占 占쏙옙占쏙옙悶占쏙옙占 Google 占싸깍옙占쏙옙 占쏙옙占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙
        // 占쏙옙占쏙옙微占 占싸깍옙占쏙옙 占식울옙 authorization code占쏙옙 占쏙옙占쏙옙 占쏙옙황占쏙옙 占쏙옙占쏙옙占싹울옙 占쌘드를 占쌜쇽옙占쌌니댐옙
        // 占쏙옙占쏙옙微占 占싸깍옙占쏙옙 占식울옙 占쏙옙占쏙옙 authorization code占쏙옙 authCode 占쏙옙占쏙옙占쏙옙 占쌀댐옙占쌌니댐옙
        string authCode = "YOUR_AUTHORIZATION_CODE";

        // 占쏙옙占쏙옙 占쌘듸옙 (authorization code)占쏙옙 占싱울옙占싹울옙 占쏙옙占쏙옙占쏙옙 占쏙옙큰 占쏙옙 ID 占쏙옙큰 占쏙옙청
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

    // Google OAuth 2.0 占쏙옙큰 占쏙옙占쏙옙占쏙옙 占식쏙옙占싹깍옙 占쏙옙占쏙옙 클占쏙옙占쏙옙
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

        // Firebase 占십깍옙화
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                // 占쏙옙占쏙옙占 占쏙옙占쏙옙 占쏙옙占쏙옙 확占쏙옙
                FirebaseUser user = auth.CurrentUser;
                if (user != null)
                {
                    // 占쏙옙占쏙옙微占 占쏙옙占쏙옙占실억옙占쏙옙占쏙옙, 占식븝옙占쌘몌옙 占쏙옙占쏙옙求占.
                    userId = user.UserId;
                    Debug.Log("User ID: " + userId);
                }
                else
                {
                    // 占쏙옙占쏙옙微占 占쏙옙占쏙옙占쏙옙占쏙옙 占십았쏙옙占싹댐옙.
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

        //占싱뱄옙 占쏙옙占쏙옙占싶곤옙 占쏙옙占쏙옙占싹댐옙 占쏙옙占쏙옙 占쌕뤄옙 return


        // Firestore 占싸쏙옙占싹쏙옙 占쏙옙占쏙옙
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;



        // 占쏙옙占쏙옙占싶몌옙 占쏙옙占쏙옙占쏙옙 占시뤄옙占실곤옙 占쏙옙占쏙옙 占쏙옙占쏙옙
        CollectionReference coll_userdata = db.Collection(GetUserId());

        DocumentReference doc_userdata = coll_userdata.Document("User_Data");
        DocumentReference doc_skill = coll_userdata.Document("Skill");


        // 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙占쏙옙 占쏙옙占쏙옙
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

        // 유저의 정보를 Firestore DB에 저장
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
            { "userName", param["nickname"] != null ? param["nickname"] : "null" },
            { "email", GetUserId() },
            { "photon_ID", "rewrite photon id" },
            { "dungeon_progress", dungeonProgress },
        };


        // 데이터를 Firestore에 저장
        doc_userdata.SetAsync(userData);
        doc_skill.SetAsync(skillData);
    }
}
