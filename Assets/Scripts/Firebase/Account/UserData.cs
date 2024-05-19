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

        try
        {
            await auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith((task) =>
            {
                if (task.IsCompletedSuccessfully)
                {
                    Debug.Log("register IsCompletedSuccessfully");
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("register IsCompleted");
                }
                else if (task.IsCanceled)
                {
                    Debug.Log("register IsCanceled");
                }
                else if (task.IsFaulted)
                {
                    Debug.Log("register IsFaulted");
                }
            });
            await auth.SignInWithEmailAndPasswordAsync(email, password);

            UserInfoManager.SetCurrentUser(auth.CurrentUser);

            await MakeDB_New(initData);
            //캐릭터 별 데이터를 생성
            //await MakeDB_Char();

            SceneManager.LoadScene("SelectCharacter");
            //SceneManager.LoadScene("TestScene");
        }
        catch (FirebaseException e)
        {
            Debug.Log($"FirebaseException : {e.StackTrace}");
        }
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
        /*
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
        */

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
                    //Debug.Log("User ID: " + userId);
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

    public static void CallFixDB()
    {
#pragma warning disable CS4014
        FixDB();
#pragma warning restore CS4014
    }

    public static async Task FixDB(Dictionary<string, object> param = null)
    {
        auth = FirebaseAuth.DefaultInstance;

        FirebaseUser user = auth.CurrentUser;
        if (user == null)
        {
            Debug.Log("failed makeDB : user is null");
            return;
        }

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        CollectionReference coll_userdata = db.Collection("User");
        DocumentReference doc_user = coll_userdata.Document(GetUserId());

        // 해당 문서가 이미 존재하는지 확인
        DocumentSnapshot snapshot = await doc_user.GetSnapshotAsync();

        //create new document reference
        if (!snapshot.Exists)
        {
            await MakeDB_New();
            return;
        }
        // 이미 존재하는 경우에는 추가로 비교
        else
        {
            // 기존의 필드를 확인하여 값이 없는 경우에만 기본 값을 설정
            //Field - UserData
            Dictionary<string, object> origin_UserData = snapshot.ToDictionary().ContainsKey("userData") ?
                    (Dictionary<string, object>)snapshot.ToDictionary()["userData"] : null;

            if (origin_UserData == null)
            {
                // userData 필드가 없는 경우 새로 생성
                origin_UserData = new Dictionary<string, object>
                {
                    { "userName", "null" },
                    { "email", GetUserId() },
                    { "progress", 0 },
                    { "money", 0 }
                };
            }
            else
            {
                // userData 필드가 있는 경우, 필요한 경우에만 기본 값을 설정
                origin_UserData = UpdateField(origin_UserData, "userName", "null");
                origin_UserData = UpdateField(origin_UserData, "email", GetUserId());
                origin_UserData = UpdateField(origin_UserData, "progress", 0);
                origin_UserData = UpdateField(origin_UserData, "money", 0);
            }

            //Field - CharData
            Dictionary<string, int> charStats = new()
            {
                { "level", 1},
                { "moveSpeed", 10 },
                { "attackSpeed", 10 },
                { "maxHealth", 100 },
                { "defence", 100 },
                { "power", 10 },
                { "maxEquipNum", 1 }
            };

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

            //유저 귀속 스킬 정보
            Dictionary<string, object> charSkill = new()
            {
                { "common", char_common_skill },
                { "warrior", char_skill_warrior },
                { "archer", char_skill_archer }
            };

            Dictionary<string, object> origin_CharData = snapshot.ToDictionary().ContainsKey("charData") ?
                    (Dictionary<string, object>)snapshot.ToDictionary()["charData"] : null;



            if (origin_CharData == null)
            {
                // charData 필드가 없는 경우 새로 생성
                origin_CharData = new Dictionary<string, object>
                {
                    { "type", "null" },
                    { "stats", charStats },
                    { "skill",  charSkill },
                    { "itemList", new List<string>() },
                    { "stack", 0 }
                };
            }
            else
            {
                // charData 필드가 있는 경우, 필요한 경우에만 기본 값을 설정
                // 기본 값으로 설정할 charData 필드가 추가된 경우에도 여기에 추가
                origin_CharData = UpdateField(origin_CharData, "type", "null");
                origin_CharData = UpdateField(origin_CharData, "stats", charStats);
                origin_CharData = UpdateField(origin_CharData, "skill", charSkill);
                origin_CharData = UpdateField(origin_CharData, "itemList", new List<string>());
                origin_CharData = UpdateField(origin_CharData, "stack", 0);
            }

            // 수정된 userData 및 charData를 다시 문서에 저장
            await snapshot.Reference.SetAsync(new Dictionary<string, object>
            {
                { "userData", origin_UserData },
                { "charData", origin_CharData }
            });
        }
    }

    private static Dictionary<string, object> UpdateField(Dictionary<string, object> data, string key, object defaultValue)
    {
        if (!data.ContainsKey(key))
        {
            data[key] = defaultValue;
        }
        return data;
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

        //캐릭터 스텟
        Dictionary<string, int> charStats = new()
        {
            { "level", 1},
            { "moveSpeed", 10 },
            { "attackSpeed", 10 },
            { "maxHealth", 100 },
            { "defence", 100 },
            { "power", 10 },
            { "maxEquipNum", 1 }
        };

        //유저 귀속 스킬 정보
        Dictionary<string, object> charSkill = new()
        {
            { "common", char_common_skill }
        };


        //유저의 컬랙션
        Dictionary<string, object> userData = new()
        {
            { "userName", "null" },
            { "email", GetUserId() },
            { "progress", 0 },
            { "money", 0 }
        };
        
        //유저의 컬랙션
        Dictionary<string, object> charData = new()
        {
            { "type", "null" },   //캐릭터 선택 후 업데이트
            { "stats", charStats },
            { "skill",  char_common_skill },
            { "itemList", new List<string>() },
            { "stack", 0 },
            { "skillPoint", 0 }
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
