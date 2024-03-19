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
    private const string redirectUri_second = "https://capstoneproject-8992c.firebaseapp.com/__/auth/handler"; // ���𷺼� URI ����
    private const string new_redirectUri = "https://hallym.capstone.photon.firebaseapp.com/__/auth/handler"; // ���𷺼� URI ����
    private const string clientId_second = "1049753969677-hfu0873d5sgcjf77dm1nbanqv14bk5g8.apps.googleusercontent.com"; // Ŭ���̾�Ʈ ID ����
    private const string scope_second = "email profile"; // ���� ���� ����

    private string authUrl_second;

    //redirection uri�� �ٸ� ������ ��ü�ϴ� ��� : {{�׼��� ���ܵ�: �� ���� ��û�� �߸��Ǿ����ϴ�}}
    //private string normal_google_uri = "https://google.com";

    public void GetGoogleTokens()
    {
        StartCoroutine(OpenOAuthURL());
        //StartCoroutine(OpenLoginWebview());   //rawImage�� Ȱ���� ����� ��Ʈ�� �� �� ����
    }

    IEnumerator OpenOAuthURL()
    {
        auth = FirebaseAuth.DefaultInstance;

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
            Debug.Log("Application.absoluteURL : " + Application.absoluteURL);
            //Debug.Log("Application.temporaryCachePath : " + Application.temporaryCachePath);
            //Debug.Log("Application.identifier : " + Application.identifier);            //hallym.capstone.photon
            //Debug.Log("Application.cloudProjectId : " + Application.cloudProjectId);    //a7a5451d-c406-4b70-80a9-007d670f7277
            

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

    public string GetUserId()
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
    
    public void MakeDB()
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
            { "userName", "projecting name" },
            { "email", GetUserId() },
            { "photon_ID", "rewrite photon id" },
            { "dungeon_progress", dungeonProgress },
        };


        // �����͸� Firestore�� ����
        doc_userdata.SetAsync(userData);
        doc_skill.SetAsync(skillData);
    }
}
