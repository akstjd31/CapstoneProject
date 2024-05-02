using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class CharSkill : MonoBehaviour
{
    public static FirebaseUser currentUser;
    private static Dictionary<string, int> userSkill;  //스킬 데이터의 인스턴스

    // Start is called before the first frame update
    async void Start()
    {
        currentUser = UserInfoManager.GetCurrentUser();
        await InitSkill();
    }

    private static async Task InitSkill()
    {
        await UserInfoManager.GetCharSkillAsync();
        // 여기에 비동기 작업 완료 후 수행할 작업 추가
        userSkill = UserInfoManager.GetSkillLevel();
        Debug.Log($"Skill level initialized: ");
        //Show_Dictionary(userSkill);
    }

    public static void SetSkillLevel(int skillNum, int level)
    {
        if (!userSkill.ContainsKey(skillNum.ToString()))
        {
            // 존재하지 않는 경우 예외 발생
            throw new KeyNotFoundException($"Skill number {skillNum} does not exist.");
        }

        userSkill[skillNum.ToString()] = level;
        UpdataSkillData();
    }

    public async static Task<int> GetSkillLevel(int skillNum)
    {
        if (userSkill == null)
        {
            await InitSkill();
        }

        if (!userSkill.ContainsKey(skillNum.ToString()))
        {
            // 존재하지 않는 경우 예외 발생
            throw new KeyNotFoundException($"Skill number {skillNum} does not exist.");
        }

        return userSkill[skillNum.ToString()];
    }

    public async static Task<List<int>> GetSkillLevelAll(int type)
    {
        if (userSkill == null)
        {
            await InitSkill();
        }

        List<int> keyList = SkillData.GetSkillKeyList(type);
        List<int> levels = new();
        
        for(int i = 0; i < keyList.Count; i++)
        {
            levels.Add(await GetSkillLevel(keyList[i]));
        }

        return levels;
    }

    //스킬의 레벨을 하나 올릴 때 사용
    public static void LevelUpSkill(int skillNum)
    {
        if (!userSkill.ContainsKey(skillNum.ToString()))
        {
            // 존재하지 않는 경우 예외 발생
            throw new KeyNotFoundException($"Skill number {skillNum} does not exist.");
        }

        userSkill[skillNum.ToString()] = ++userSkill[skillNum.ToString()];
        UpdataSkillData();
    }

    //CharSkill.cs (userSkill) => UserInfoManager.cs (skillLevel) & Firestore Server
    private static void UpdataSkillData()
    {
        UserInfoManager.SetSkillLevel(userSkill);
    }

    //테스트 용 메소드
    public static void Debug_SkillUp_1001()
    {
        LevelUpSkill(1001);
    }
    public static void Debug_SkillUp_10001()
    {
        LevelUpSkill(10001);
    }
    public static void Debug_SkillUp_20001()
    {
        LevelUpSkill(20001);
    }

    public static void Debug_ShowSkill()
    {
        Show_Dictionary(userSkill);
    }

    private static void Show_Dictionary(Dictionary<string, int> dict)
    {
        string data = "";

        if (dict != null)
        {
            Debug.Log("Skill Level :");
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

class SkillData
{
    private static readonly Dictionary<int, string> skill_common = new()
    {
        {1001, "교만"},
        {1002, "탐욕"},
        {1003, "색욕"},
        {1004, "질투"},
        {1005, "먹보"},
        {1006, "분노"},
        {1007, "나태"}
    };
    private static readonly Dictionary<int, string> skill_warrior = new()
    {
        {10001, "전사1" },
        {10002, "전사2" },
        {10003, "전사3" },
        {10004, "전사4" },
        {10005, "전사5" }
    };
    private static readonly Dictionary<int, string> skill_archer = new()
    {
        {20001, "아처1" },
        {20002, "아처2" },
        {20003, "아처3" },
        {20004, "아처4" },
        {20005, "아처5" }
    };

    public static string Skill_NumToName(int skillNum)
    {
        //common skill
        if (skillNum < 10000)
        {
            return skill_common[skillNum];
        }
        //warrior skill
        else if (skillNum < 20000)
        {
            return skill_warrior[skillNum];
        }
        //archer skill
        else
        {
            return skill_archer[skillNum];
        }
    }

    public static int Skill_NameToNum(string name)
    {
        var rSkill_common = ReverseSkillDictionary(skill_common);
        var rSkill_warrior = ReverseSkillDictionary(skill_warrior);
        var rSkill_archer = ReverseSkillDictionary(skill_archer);

        if(rSkill_common.ContainsKey(name))
        {
            return rSkill_common[name];
        }
        else if(rSkill_warrior.ContainsKey(name))
        {
            return rSkill_warrior[name];
        }
        else if(rSkill_archer.ContainsKey(name))
        {
            return rSkill_archer[name];
        }
        else
        {
            return -1;
        }
    }

    public static int GetSkillCount(int type = 0)
    {
        int count = 0;

        switch (type)
        {
            case 0:
                count = skill_common.Count;
                break;
            case 1:
                count = skill_warrior.Count;
                break;
            case 2:
                count = skill_archer.Count;
                break;

        }
        return count;
    }

    public static List<int> GetSkillKeyList(int type = 0)
    {
        List<int> rtList = new();

        switch(type)
        {
            case 0:
                rtList = skill_common.Keys.ToList();
                break;
            case 1:
                rtList = skill_warrior.Keys.ToList();
                break;
            case 2:
                rtList = skill_archer.Keys.ToList();
                break;

        }

        return rtList;
    }
    
    public static List<string> GetSkillNameList(int type = 0)
    {
        List<string> rtList = new();

        switch(type)
        {
            case 0:
                rtList = skill_common.Values.ToList();
                break;
            case 1:
                rtList = skill_warrior.Values.ToList();
                break;
            case 2:
                rtList = skill_archer.Values.ToList();
                break;

        }

        return rtList;
    }

    private static Dictionary<string, int> ReverseSkillDictionary(Dictionary<int, string> dictionary)
    {
        return dictionary.ToDictionary(x => x.Value, x => x.Key);
    }
}
