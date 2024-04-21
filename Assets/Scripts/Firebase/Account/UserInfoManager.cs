using Firebase.Auth;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UserInfoManager : MonoBehaviour
{
    private static UserInfoManager instance;
    private static FirebaseUser currentUser; // Firebase 사용자 정보를 저장할 변수
    private static Dictionary<string, int> skillLevel; //스킬 데이터의 원본

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
}
