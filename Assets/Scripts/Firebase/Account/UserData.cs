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
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserData : MonoBehaviour
{
    private static FirebaseAuth auth;
    private static Text loginErrorMsg;
    //안드로이드 디지털 지문 //https://www.youtube.com/watch?v=AiXIAe6on5M&t=563s

    public static async Task RegisterWithEmail_Password(string email, string password, Dictionary<string, object> initData = null)
    {
        auth = FirebaseAuth.DefaultInstance;

        await auth.CreateUserWithEmailAndPasswordAsync(email, password);
        await auth.SignInWithEmailAndPasswordAsync(email, password);

        UserInfoManager.SetCurrentUser(auth.CurrentUser);

        await MakeDB_New(initData);
        //캐릭터 별 데이터를 생성
        //await MakeDB_Char();

        SceneManager.LoadScene("SelectCharacter");
    }

    public static async Task SetNickname(string nickname)
    {
        auth = FirebaseAuth.DefaultInstance;
        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.Log("failed to update user name: user is null");
            return;
        }

        // Firestore 인스턴스 생성
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // 데이터를 저장할 컬렉션과 문서 참조
        CollectionReference coll_userdata = db.Collection("User");
        CollectionReference coll_nickname = db.Collection("Nickname");

        DocumentReference doc_user = coll_userdata.Document(GetUserId());
        DocumentReference doc_nickname = coll_nickname.Document(GetUserId());

        // 업데이트할 필드와 값 설정
        Dictionary<string, object> updates = new Dictionary<string, object>
        {
            { "userData.userName", nickname }
        };

        // 사용자 문서의 userName 필드 업데이트
        await doc_user.UpdateAsync(updates);

        // 닉네임 컬렉션에 새로운 닉네임 값을 저장
        Dictionary<string, object> nicknameData = new Dictionary<string, object>
        {
            { "nickname", nickname }
        };
        await doc_nickname.SetAsync(nicknameData);

        Debug.Log("User name updated successfully!");
    }

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

        loginErrorMsg = GameObject.Find("Error_msg").GetComponent<Text>();
        loginErrorMsg.text = "";
    }

    public static string GetUserId()
    {
        auth = FirebaseAuth.DefaultInstance;

        string userId = auth.CurrentUser.Email;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                FirebaseUser user = auth.CurrentUser;
                if (user != null)
                {
                    userId = user.UserId;
                    Debug.Log("User ID: " + userId);
                }
                else
                {
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
    
    //이하는 DB 생성 메소드만 존재
    public static async Task MakeDB_New(Dictionary<string, object> param = null)
    {
        auth = FirebaseAuth.DefaultInstance;

        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.Log("failed makeDB : user is null");
            return;
        }

        // Firestore 인스턴스 생성
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;


        // 데이터를 저장할 컬렉션과 문서 참조
        CollectionReference coll_userdata = db.Collection("User");

        DocumentReference doc_user = coll_userdata.Document(GetUserId());

        //공통 스킬
        Dictionary<string, int> char_common_skill = new()
        {
            { "1001", 0 },
            { "1002", 0 },
            { "1003", 0 },
            { "1004", 0 },
            { "1005", 0 },
            { "1006", 0 },
            { "1007", 0 }
        };

        //근접 캐릭터 스킬 정보
        Dictionary<string, int> char_skill_warrior = new()
        {
            { "10001", 0 },
            { "10002", 0 },
            { "10003", 0 },
            { "10004", 0 },
            { "10005", 0 },
        };

        //원거리 캐릭터 스킬 정보
        Dictionary<string, int> char_skill_archer = new()
        {
            { "20001", 0 },
            { "20002", 0 },
            { "20003", 0 },
            { "20004", 0 },
            { "20005", 0 },
        };

        //캐릭터 스텟
        Dictionary<string, object> charStats = new()
        {
            { "moveSpeed", "10" },
            { "attackSpeed", "10" },
            { "maxHealth", "100" },
            { "defence", "100" },
            { "power", "10" },
            { "maxEquipNum", "1" }
        };

        //유저 귀속 스킬 정보
        Dictionary<string, object> charSkill = new()
        {
            { "common", char_common_skill },
            { "warrior", char_skill_warrior },
            { "archer", char_skill_archer }
        };


        //유저의 컬랙션
        Dictionary<string, object> userData = new()
        {
            { "userName", "null" },
            { "email", GetUserId() },
            { "progress", 0 }
        };
        
        //유저의 컬랙션
        Dictionary<string, object> charData = new()
        {
            { "type", "null" },   //캐릭터 선택 후 업데이트
            { "stats", charStats },
            { "skill",  charSkill },
            { "itemList", new List<string>() },
            { "stack", 0 }
        };
        
        Dictionary<string, object> userContainer = new()
        {
            { "userData", userData },
            { "charData", charData }
        };


        // 데이터를 Firestore에 저장
        await doc_user.SetAsync(userContainer);
    }

    public static async Task MakeDB_Char()
    {
        // Firestore 인스턴스 생성
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;


        // 데이터를 저장할 컬렉션과 문서 참조
        CollectionReference coll_char = db.Collection("CharData");

        DocumentReference doc_warrior = coll_char.Document("warrior");
        DocumentReference doc_archer = coll_char.Document("archer");

        Dictionary<string, object> warriorData = new()
        {
            { "attackRange", 20 },
            { "attackDistance", 10 },
            { "rollingDistance", 10 }
        };
        
        Dictionary<string, object> archerData = new()
        {
            { "attackRange", 10 },
            { "attackDistance", 50 },
            { "rollingDistance", 20 }
        };

        await doc_warrior.SetAsync(warriorData);
        await doc_archer.SetAsync(archerData);
    }
}
