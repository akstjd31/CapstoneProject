using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class CharSkill : MonoBehaviour
{
    public static FirebaseUser currentUser;
    private static Dictionary<string, int> userSkill;  //��ų �������� �ν��Ͻ�

    // Start is called before the first frame update
    async void Start()
    {
        currentUser = UserInfoManager.GetCurrentUser();
        await InitSkill();
    }

    private async Task InitSkill()
    {
        await UserInfoManager.GetCharSkillAsync();
        // ���⿡ �񵿱� �۾� �Ϸ� �� ������ �۾� �߰�
        userSkill = UserInfoManager.GetSkillLevel();
        Debug.Log($"Skill level initialized: ");
        //Show_Dictionary(userSkill);
    }

    public static void SetSkillLevel(int skillNum, int level)
    {
        if (!userSkill.ContainsKey(skillNum.ToString()))
        {
            // �������� �ʴ� ��� ���� �߻�
            throw new KeyNotFoundException($"Skill number {skillNum} does not exist.");
        }

        userSkill[skillNum.ToString()] = level;
        UpdataSkillData();
    }

    public static int GetSkillLevel(int skillNum)
    {
        if (!userSkill.ContainsKey(skillNum.ToString()))
        {
            // �������� �ʴ� ��� ���� �߻�
            throw new KeyNotFoundException($"Skill number {skillNum} does not exist.");
        }

        return userSkill[skillNum.ToString()];
    }

    //��ų�� ������ �ϳ� �ø� �� ���
    public static void LevelUpSkill(int skillNum)
    {
        if (!userSkill.ContainsKey(skillNum.ToString()))
        {
            // �������� �ʴ� ��� ���� �߻�
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

    //�׽�Ʈ �� �޼ҵ�
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
        {1001, "����"},
        {1002, "Ž��"},
        {1003, "����"},
        {1004, "����"},
        {1005, "�Ժ�"},
        {1006, "�г�"},
        {1007, "����"}
    };
    private static readonly Dictionary<int, string> skill_warrior = new()
    {
        {10001, "����1" },
        {10002, "����2" },
        {10003, "����3" },
        {10004, "����4" },
        {10005, "����5" }
    };
    private static readonly Dictionary<int, string> skill_archer = new()
    {
        {20001, "��ó1" },
        {20002, "��ó2" },
        {20003, "��ó3" },
        {20004, "��ó4" },
        {20005, "��ó5" }
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