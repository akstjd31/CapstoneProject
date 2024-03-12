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
