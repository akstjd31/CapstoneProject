using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
