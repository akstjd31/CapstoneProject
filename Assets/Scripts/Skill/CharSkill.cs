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
    void Start()
    {
        currentUser = UserInfoManager.GetCurrentUser();
        StartCoroutine(InitSkill());
    }

    private IEnumerator InitSkill()
    {
        yield return UserInfoManager.GetCharSkillAsync();
        // 여기에 비동기 작업 완료 후 수행할 작업 추가
        Debug.Log($"Skill level initialized: {UserInfoManager.GetSkillLevel()}");
    }

    public static void SetSkillLevel(string skillNum, int level)
    {
        if (!userSkill.ContainsKey(skillNum))
        {
            // 존재하지 않는 경우 예외 발생
            throw new KeyNotFoundException($"Skill number {skillNum} does not exist.");
        }

        userSkill[skillNum] = level;
    }

    public static int GetSkillLevel(string skillNum)
    {
        if (!userSkill.ContainsKey(skillNum))
        {
            // 존재하지 않는 경우 예외 발생
            throw new KeyNotFoundException($"Skill number {skillNum} does not exist.");
        }

        return userSkill[skillNum];
    }

    //스킬의 레벨을 하나 올릴 때 사용
    public static void LevelUpSkill(string skillNum)
    {
        if (!userSkill.ContainsKey(skillNum))
        {
            // 존재하지 않는 경우 예외 발생
            throw new KeyNotFoundException($"Skill number {skillNum} does not exist.");
        }

        userSkill[skillNum] = ++userSkill[skillNum];
    }
}

class SkillData
{
    static Dictionary<int, string> skill_common = new()
    {
        {1001, "교만"},
        {1002, "탐욕"},
        {1003, "색욕"},
        {1004, "질투"},
        {1005, "먹보"},
        {1006, "분노"},
        {1007, "나태"}
    };
    static Dictionary<int, string> skill_warrior = new()
    {
        {10001, "전사1" },
        {10002, "전사2" },
        {10003, "전사3" },
        {10004, "전사4" },
        {10005, "전사5" }
    };
    static Dictionary<int, string> skill_archer = new()
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

    private static Dictionary<string, int> ReverseSkillDictionary(Dictionary<int, string> dictionary)
    {
        return dictionary.ToDictionary(x => x.Value, x => x.Key);
    }
}
