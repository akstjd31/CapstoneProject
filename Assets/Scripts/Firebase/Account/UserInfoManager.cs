using Firebase.Auth;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserInfoManager : MonoBehaviour
{
    private static UserInfoManager instance;
    private static FirebaseUser currentUser; // Firebase 사용자 정보를 저장할 변수
    private static Dictionary<string, int> skillLevel; //스킬 데이터의 원본
    private static Inventory inv;

    private bool isDebug = true;
    private static int nowMoney = 0;

    void Awake()
    {
        // UserInfoManager가 다른 씬으로 전환되어도 파괴되지 않도록 함
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        inv = FindObjectOfType<Inventory>();

        // 3초마다 GetUserMoney 함수를 호출합니다.
        InvokeRepeating("WrapGetUserMoney", 0f, 3f);
    }

    //디버그인 경우 유저를 FirebaseAuth에서 자동 삭제
    private void OnApplicationQuit()
    {
        if(isDebug && currentUser != null)
        {
            currentUser.DeleteAsync();
        }
    }

    // 사용자 정보를 설정하는 메서드
    public static void SetCurrentUser(FirebaseUser user)
    {
        currentUser = user;
        Debug.Log($"set user {currentUser}");
    }
    
    public static FirebaseUser GetCurrentUser()
    {
        return currentUser;
    }

    public static Dictionary<string, int> GetSkillLevel()
    {
        return skillLevel;
    }

    //use for publish
    public static async Task SetUserMoney_Async(int change)
    {
        try
        {
            nowMoney = await GetUserMoney_Async();

            if (nowMoney < 0)
            {
                Debug.Log($"user money is not valid => {nowMoney}");
                return;
            }

            //update DB
            FirebaseAuth auth = FirebaseAuth.DefaultInstance;
            FirebaseUser user = auth.CurrentUser;

            if (user == null)
            {
                Debug.Log("Failed to update money: User is null");
                return;
            }

            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

            DocumentReference docUser = db.Collection("User").Document(UserData.GetUserId());
            DocumentSnapshot snapshot = await docUser.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> userData = snapshot.ToDictionary();

                if (userData != null && userData.ContainsKey("userData"))
                {
                    Dictionary<string, object> userContainer = (Dictionary<string, object>)userData["userData"];
                    //Debug.Log($"in setMoney : contain key \"money\" : {userContainer.ContainsKey("money")}");

                    int updateMoney = userContainer.ContainsKey("money") ? Convert.ToInt32(userContainer["money"]) : 0;
                    updateMoney += change;
                    userContainer["money"] = updateMoney;

                    // 업데이트된 userData를 다시 Firestore에 저장
                    await docUser.UpdateAsync("userData", userContainer);

                    nowMoney = updateMoney;

                    //상점 이용 시, 인벤토리에 반영
                    if(inv != null)
                    {
                        inv.Refresh_InvMoney();
                    }
                    else
                    {
                        inv = FindObjectOfType<Inventory>();
                        inv.Refresh_InvMoney();
                    }
                }
                else
                {
                    Debug.Log("Failed to update money: userData is null or does not contain 'userData'");
                }
            }
            else
            {
                Debug.Log("Failed to update money: User document does not exist");
            }
        }
        finally
        {
            NpcShop.ReleaseSemaphore();
        }
    }

    public static int GetNowMoney()
    {
        return nowMoney;
    }

    private void WrapGetUserMoney()
    {
        GetUserMoney();
    }

    public static void GetUserMoney()
    {
#pragma warning disable CS4014
        if (SceneManager.GetActiveScene().name == "LobbyScene")
        {
            Debug.Log($"Call GetUserMoney : {nowMoney}");
            GetUserMoney_Async();
        }

        //instance.StartCoroutine(instance.RunGetUserMoneyAsync());
#pragma warning restore CS4014
    }

    private IEnumerator RunGetUserMoneyAsync()
    {
        //추후 수정
        if(SceneManager.GetActiveScene().name != "LobbyScene")
        {
            yield return null;
        }

        yield return GetUserMoney_Async();
    }

    public static async Task<int> GetUserMoney_Async()
    {
        try
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

            CollectionReference coll_userdata = db.Collection("User");
            DocumentReference doc_user = coll_userdata.Document(UserData.GetUserId());

            DocumentSnapshot snapshot = await doc_user.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> userData = snapshot.ToDictionary();
                if (userData != null && userData.ContainsKey("userData"))
                {
                    Dictionary<string, object> userDataMap = (Dictionary<string, object>)userData["userData"];
                    if (userDataMap.ContainsKey("money"))
                    {
                        //Debug.Log($"userDataMap[\"money\"] : {userDataMap["money"]}");
                        nowMoney = Convert.ToInt32(userDataMap["money"]);
                        //Debug.Log($"get money from server : {money} {FirebaseAuth.DefaultInstance.CurrentUser.Email}");

                        //통신 때마다 보유 금액 업데이트
                        if (inv != null)
                        {
                            inv.Refresh_InvMoney();
                        }
                        else
                        {
                            inv = FindObjectOfType<Inventory>();
                            inv.Refresh_InvMoney();
                        }

                        return nowMoney;
                    }
                    else
                    {
                        Debug.Log("userDataMap does not contain money key or is null");

                        return -1;
                    }
                }
                else
                {
                    Debug.Log("userData does not contain \'userData\' key or is null");

                    return -2;
                }
            }
            else
            {
                Debug.Log("Document does not exist\nMake User DB now...");
                await UserData.MakeDB_New();
                return 0;
            }
        }
        finally
        {
            //Debug.Log("call NpcShop.ReleaseSemaPhore in GetMoney");
            NpcShop.ReleaseSemaphore();
        }
    }

    public static async Task GetCharSkillAsync()
    {
        if (currentUser == null)
        {
            Debug.Log("Failed to get char_skill_warrior: user is null");
            return;
        }

        Dictionary<string, int> skill = new();

        // Firestore 인스턴스 생성
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

        // 데이터를 가져올 컬렉션과 문서 참조
        CollectionReference coll_userdata = db.Collection("User");
        DocumentReference doc_user = coll_userdata.Document(UserData.GetUserId());

        // 문서 스냅샷 가져오기
        DocumentSnapshot snapshot = await doc_user.GetSnapshotAsync();

        // 스냅샷이 존재하는지 확인
        if (snapshot.Exists)
        {
            Dictionary<string, object> userData = snapshot.ToDictionary();

            if (userData.ContainsKey("charData") && userData["charData"] is Dictionary<string, object> charData &&
                charData.ContainsKey("skill") && charData["skill"] is Dictionary<string, object> charSkill)
            {
                //Dictionary<string, object> => Dictionary<string, int>
                //kvp.Value.GetType()는 System.Int64 == long 타입
                if (charSkill.ContainsKey("common") && charSkill["common"] is Dictionary<string, object> commonSkills)
                {
                    foreach (var kvp in commonSkills)
                    {
                        //Debug.Log($"in foreach1 : {kvp}, {kvp.Key}, {kvp.Value}, {kvp.Value.GetType()}");

                        if (kvp.Value is int intValue)
                        {
                            skill[kvp.Key] = intValue;
                        }
                        else if (kvp.Value is long longValue)
                        {
                            skill[kvp.Key] = (int)longValue;
                        }
                    }
                }
                if (charSkill.ContainsKey("warrior") && charSkill["warrior"] is Dictionary<string, object> warriorSkills)
                {
                    foreach (var kvp in warriorSkills)
                    {
                        //Debug.Log($"in foreach2 : {kvp}, {kvp.Key}, {kvp.Value}, {kvp.Value.GetType()}");

                        if (kvp.Value is int intValue)
                        {
                            skill[kvp.Key] = intValue;
                        }
                        else if (kvp.Value is long longValue)
                        {
                            skill[kvp.Key] = (int)longValue;
                        }
                    }
                }
                if (charSkill.ContainsKey("archer") && charSkill["archer"] is Dictionary<string, object> archerSkills)
                {
                    foreach (var kvp in archerSkills)
                    {
                        //Debug.Log($"in foreach3 : {kvp}, {kvp.Key}, {kvp.Value}, {kvp.Value.GetType()}");

                        if (kvp.Value is int intValue)
                        {
                            skill[kvp.Key] = intValue;
                        }
                        else if (kvp.Value is long longValue)
                        {
                            skill[kvp.Key] = (int)longValue;
                        }
                    }
                }
            }

            skillLevel = new();
            foreach (var kvp in skill)
            {
                skillLevel[kvp.Key] = kvp.Value;
            }

            //Debug.Log("in UserInfoManager ");
            //Show_Dictionary(skillLevel);
        }
        else
        {
            Debug.Log("Document does not exist!");
        }
    }

    //CharSkill.cs에서만 사용
    public static void SetSkillLevel(Dictionary<string, int> skill)
    {
        SetSkillLevel_Async(skill);
    }

    public static async void SetSkillLevel_Async(Dictionary<string, int> skill)
    {
        skillLevel = skill;

        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        CollectionReference coll_userdata = db.Collection("User");
        DocumentReference doc_user = coll_userdata.Document(UserData.GetUserId());

        // 기존 문서의 데이터를 가져옵니다.
        DocumentSnapshot snapshot = await doc_user.GetSnapshotAsync();
        if (snapshot.Exists)
        {
            Dictionary<string, object> userData = snapshot.ToDictionary();

            if (userData.ContainsKey("charData") && userData["charData"] is Dictionary<string, object> charData)
            {
                // charSkill을 업데이트할 새로운 값으로 설정합니다.
                charData["skill"] = skill;

                // 업데이트된 데이터를 문서에 반영합니다.
                await doc_user.UpdateAsync("charData", charData);
            }
        }
        else
        {
            Debug.Log("Document does not exist!");
        }
    }

    public static UserInfoManager GetInstance()
    {
        return instance;
    }

    private static void Show_Dictionary(Dictionary<string, int> dict)
    {
        string data = "";

        if (dict != null)
        {
            foreach (var kvp in dict)
            {
                data += $"Key: {kvp.Key}, Value: {kvp.Value}\n";
            }
            Debug.Log(data);
        }
        else
        {
            Debug.Log("Skill Level is null.");
        }
    }

    //use in SelectCharacter Scene
    public static async Task SetCharClass(string type)
    {
        try
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

            CollectionReference coll_userdata = db.Collection("User");
            DocumentReference doc_user = coll_userdata.Document(UserData.GetUserId());

            DocumentSnapshot snapshot = await doc_user.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> userData = snapshot.ToDictionary();
                if (userData != null && userData.ContainsKey("charData"))
                {
                    Dictionary<string, object> charDataMap = (Dictionary<string, object>)userData["charData"];
                    if (charDataMap.ContainsKey("stats"))
                    {
                        // Update charDataMap with the new type
                        charDataMap["type"] = type;

                        Dictionary<string, object> updateData = new Dictionary<string, object>
                    {
                        { "charData", charDataMap }
                    };

                        await doc_user.UpdateAsync(updateData);
                    }
                    else
                    {
                        Debug.Log("charDataMap does not contain stats key or is null");
                    }
                }
                else
                {
                    Debug.Log("userData does not contain charData key or is null");
                }
            }
            else
            {
                Debug.Log("Document does not exist\nMake User DB now...");
                await UserData.MakeDB_New();
            }
        }
        finally
        {

        }
    }

    public static async Task<string> GetCharClass()
    {
        try
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

            CollectionReference coll_userdata = db.Collection("User");
            DocumentReference doc_user = coll_userdata.Document(UserData.GetUserId());

            DocumentSnapshot snapshot = await doc_user.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> userData = snapshot.ToDictionary();
                if (userData != null && userData.ContainsKey("charData"))
                {
                    Dictionary<string, object> charDataMap = (Dictionary<string, object>)userData["charData"];
                    if (charDataMap.ContainsKey("type"))
                    {
                        return charDataMap["type"].ToString();
                    }
                    else
                    {
                        Debug.Log("charDataMap does not contain type key or is null");
                        return null; // or throw an exception depending on how you want to handle this case
                    }
                }
                else
                {
                    Debug.Log("userData does not contain charData key or is null");
                    return null; // or throw an exception depending on how you want to handle this case
                }
            }
            else
            {
                Debug.Log("Document does not exist\nMake User DB now...");
                await UserData.MakeDB_New();
                return null;
            }
        }
        finally
        {

        }
    }

    public static async Task<int> GetLevel()
    {
        try
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

            CollectionReference coll_userdata = db.Collection("User");
            DocumentReference doc_user = coll_userdata.Document(UserData.GetUserId());

            DocumentSnapshot snapshot = await doc_user.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> userData = snapshot.ToDictionary();
                if (userData != null && userData.ContainsKey("charData"))
                {
                    Dictionary<string, object> charDataMap = (Dictionary<string, object>)userData["charData"];
                    if (charDataMap.ContainsKey("stats"))
                    {
                        Dictionary<string, object> charStats = (Dictionary<string, object>)charDataMap["stats"];
                        if (charStats.ContainsKey("level"))
                        {
                            return Convert.ToInt32(charStats["level"]);
                        }
                        else
                        {
                            Debug.Log("charStats does not contain level key or is null");
                            return -1;
                        }
                    }
                    else
                    {
                        Debug.Log("charDataMap does not contain stats key or is null");
                        return -2;
                    }
                }
                else
                {
                    Debug.Log("userData does not contain charData key or is null");
                    return -3;
                }
            }
            else
            {
                Debug.Log("Document does not exist\nMake User DB now...");
                await UserData.MakeDB_New();
                return 0;
            }
        }
        finally
        {

        }
    }

    public static async Task SetLevel(int newLevel)
    {
        try
        {
            FirebaseFirestore db = FirebaseFirestore.DefaultInstance;

            CollectionReference coll_userdata = db.Collection("User");
            DocumentReference doc_user = coll_userdata.Document(UserData.GetUserId());

            DocumentSnapshot snapshot = await doc_user.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> userData = snapshot.ToDictionary();
                if (userData != null && userData.ContainsKey("charData"))
                {
                    Dictionary<string, object> charDataMap = (Dictionary<string, object>)userData["charData"];
                    if (charDataMap.ContainsKey("stats"))
                    {
                        Dictionary<string, object> charStats = (Dictionary<string, object>)charDataMap["stats"];
                        charStats["level"] = newLevel;
                        charDataMap["stats"] = charStats;

                        Dictionary<string, object> updateData = new Dictionary<string, object>
                    {
                        { "charData", charDataMap }
                    };

                        await doc_user.UpdateAsync(updateData);
                    }
                    else
                    {
                        Debug.Log("charDataMap does not contain stats key or is null");
                    }
                }
                else
                {
                    Debug.Log("userData does not contain charData key or is null");
                }
            }
            else
            {
                Debug.Log("Document does not exist\nMake User DB now...");
                await UserData.MakeDB_New();
            }
        }
        finally
        {

        }
    }
}
