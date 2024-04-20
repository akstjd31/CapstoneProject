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
                if (charSkill.ContainsKey("warrior") && charSkill["warrior"] is Dictionary<string, int> commonSkills)
                {
                    foreach (var kvp in commonSkills)
                    {
                        skill[kvp.Key] = kvp.Value;
                    }
                }
                if (charSkill.ContainsKey("warrior") && charSkill["warrior"] is Dictionary<string, int> warriorSkills)
                {
                    foreach (var kvp in warriorSkills)
                    {
                        skill[kvp.Key] = kvp.Value;
                    }
                }
                if (charSkill.ContainsKey("archer") && charSkill["archer"] is Dictionary<string, int> archerSkills)
                {
                    foreach (var kvp in archerSkills)
                    {
                        skill[kvp.Key] = kvp.Value;
                    }
                }
            }

            skillLevel = skill;
        }
        else
        {
            Debug.Log("Document does not exist!");
        }
    }

    //CharSkill.cs에서만 사용
    public static void SetSkillLevel(Dictionary<int, int> skill)
    {

    }

    public static UserInfoManager GetInstance()
    {
        return instance;
    }
}
