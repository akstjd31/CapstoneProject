using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CharSkill : MonoBehaviour
{
    public static FirebaseUser currentUser;
    private static Dictionary<string, int> userSkill;  //��ų �������� �ν��Ͻ�
    private static Button[] btn_skill = new Button[7];  //��ư�� ������ �������� ����
    private static List<string> skill_name = new()
    {
        "pride", "greed", "lust", "envy", "glutny", "wrath", "sloth"
        //����, Ž��, ����, ����, �Ժ�, �г�, ����
        //1001~1007
    };
    private static int skill_point;

    //explantion
    [SerializeField]
    private static GameObject explane;
    private Vector2 pos;
    private RaycastHit2D hit;
    private int layerMask;

    private PlayerCtrl pc;
    private Button btn_close;
    [SerializeField]
    private GameObject btn_prefab;
    private static GameObject btn_Lobby_close;
    private static string col_name = "";

    private async void Init()
    {
        await InitSkill();
    }

    void Start()
    {
        Scene skillUIScene = SceneManager.GetSceneByName("Skill_UI");
        if (skillUIScene.IsValid() && skillUIScene.isLoaded)
        {
            SceneManager.SetActiveScene(skillUIScene);
            currentUser = UserInfoManager.GetCurrentUser();
            GameObject.Find("Main Camera").GetComponent<Camera>().enabled = true;

            btn_Lobby_close = Instantiate(btn_prefab, GameObject.Find("Canvas").transform);
            btn_Lobby_close.name = "prefab_close_btn";
            btn_Lobby_close.transform.position = new Vector2(1880, 1040);
            btn_Lobby_close.GetComponent<Button>().onClick.AddListener(() =>
            {
                CloseSkillUI();
            });
            Init();
        }
        else
        {
            Debug.LogError("Skill_UI scene is not loaded.");
        }

        //OpenPartyButton, CreatePartyButton

        btn_skill = GameObject.Find("Images").GetComponentsInChildren<Button>();
        btn_close = GameObject.Find("ButtonX").GetComponent<Button>();
        pc = FindObjectOfType<PlayerCtrl>();
        pc.DisableLobbyUI();

        for (int i = 0; i< btn_skill.Length; i++)
        {
            int index = i;

            btn_skill[i].onClick.AddListener(async () =>
            {
                skill_point = await UserInfoManager.GetSkillPoint();
                UpgradeSkill(btn_skill[index].transform.parent.name);
            });
            //Debug.Log($"{btn_skill[i].transform.parent.name}");
        }

        //explane.SetActive(false);
        explane = pc.GetSkillExplane();
        layerMask = LayerMask.GetMask("Skill_UI");
        //Invoke();
        
    }

    private void Update()
    {
        pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity, layerMask);

        if (hit.collider != null)
        {
            Debug.Log($"hit coll : {hit.collider.name} {hit.transform.tag}");
            explane.SetActive(true);

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log($"pos : {mousePosition}");
            //explane.transform.position = mousePosition;
            //pc.SetSkillExplanePos(mousePosition);
            col_name = hit.collider.name;

            Explane_Pos.SetSkillExplanePos_SetMousePos();

            /*
            if (hit.collider.name.Equals("envy") || hit.collider.name.Equals("pride") || hit.collider.name.Equals("greed"))
            {
                Explane_Pos.SetSkillExplanePos_Under(mousePosition);
            }
            else if(hit.collider.name.Equals("sloth") || hit.collider.name.Equals("lust"))
            {
                Explane_Pos.SetSkillExplanePos_Collider(hit.transform.position);
            }
            else
            {
                Explane_Pos.SetSkillExplanePos_Collider2(hit.transform.position);
            }
            */
        }
        else
        {
            col_name = "";
            explane.SetActive(false);
        }

        Debug.DrawRay(pos, Vector3.forward * 10, Color.red);
    }

    public static string GetSkillDesc()
    {
        SkillData.GetSkillDesc(
            SkillData.Skill_NameToNum(
                SkillData.GetSkillNameKr(col_name)));

        return col_name;
    }

    private void OnDestroy()
    {
        pc.EnableLobbyUI();
    }

    private static async Task InitSkill()
    {
        await UserInfoManager.GetCharSkillAsync();
        // ���⿡ �񵿱� �۾� �Ϸ� �� ������ �۾� �߰�
        userSkill = UserInfoManager.GetSkillLevel();
        Debug.Log($"Skill level initialized: ");
        //Show_Dictionary(userSkill);
        skill_point = await UserInfoManager.GetSkillPoint();
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

    public async static Task<int> GetSkillLevel(int skillNum)
    {
        if (userSkill == null)
        {
            await InitSkill();
        }

        if (!userSkill.ContainsKey(skillNum.ToString()))
        {
            // �������� �ʴ� ��� ���� �߻�
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

    //UI���� �����ϴ� �޼ҵ�
    //parameter�� ���� �̸�
    public static async void UpgradeSkill(string skillName)
    {
        if(skill_point < 1)
        {
            return;
        }

        //"pride", "greed", "lust", "envy", "glutny", "wrath", "sloth"
        //����, Ž��, ����, ����, �Ժ�, �г�, ����
        List<string> skillName_kr = new()
        {
            "����", "Ž��", "����", "����", "�Ժ�", "�г�", "����"
        };

        int index = -1;
        switch(skillName)
        {
            case "pride":
                index = 0;
                break;
            case "greed":
                index = 1;
                break;
            case "lust":
                index = 2;
                break;
            case "envy":
                index = 3;
                break;
            case "glutny":
                index = 4;
                break;
            case "wrath":
                index = 5;
                break;
            case "sloth":
                index = 6;
                break;
        }

        if(index == -1)
        {
            Debug.Log($"skillName {skillName} is not exist");
            return;
        }

        //parameter�� �ѱ� �̸�
        int skillNum = SkillData.Skill_NameToNum(skillName_kr[index]);
        Debug.Log($"Upgrade by button => {skillNum} {skillName}");

        await UserInfoManager.SetSkillPoint(--skill_point);

        LevelUpSkill(skillNum);
    }

    //CharSkill.cs (userSkill) => UserInfoManager.cs (skillLevel) & Firestore Server
    private static void UpdataSkillData()
    {
        UserInfoManager.SetSkillLevel(userSkill);
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

    public static void CloseSkillUI()
    {
        if(btn_Lobby_close != null)
        {
            Destroy(btn_Lobby_close);
        }
        SceneManager.UnloadSceneAsync("Skill_UI");
    }

    public static void SetExplane(GameObject ex)
    {
        explane = ex;
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

    private static readonly Dictionary<int, string> skill_desc = new()
    {
        {1001, "ü���� 30%���� ���� ������ �ִ� ���ط� ����\n�� ��� ������ �޴� ���ط� 10% ����"},
        {1002, "��� ȹ�淮 ����"},
        {1003, "�ǰ� ������ �ʰ� ���� 5ȸ �̻� �� ���ݷ� 10�� ����"},
        {1004, "ȸ���� 50%\n�� ��� �ǰ� �� �÷��̾�� ���� �÷��̾� ���� ���� �ǰ�"},
        {1005, "���� �� ����ü���� 5�ۼ�Ʈ�� ���ط� ����\nü���� 50�� �Ʒ��� �Ǹ� �ǰ� ������ 30�� ����"},
        {1006, "������ �ǰ� �� ���� �Ƹ� �ð� ����\n���� �Ƹ� �ð� ���� ���ݷ� 30�� ����"},
        {1007, "������ ���� �������� ����\n1ȸ ������ 3�ʰ� ���� �Ұ�\n�� ��� ���ݷ��� 200�ۼ�Ʈ ������"},
        {10001, "����1 ��ų ����" },
        {10002, "����2 ��ų ����" },
        {10003, "����3 ��ų ����" },
        {10004, "����4 ��ų ����" },
        {10005, "����5 ��ų ����" },
        {20001, "��ó1 ��ų ����" },
        {20002, "��ó2 ��ų ����" },
        {20003, "��ó3 ��ų ����" },
        {20004, "��ó4 ��ų ����" },
        {20005, "��ó5 ��ų ����" }
    };

    public static string GetSkillDesc(int skillNum)
    {
        return skill_desc[skillNum];
    }

    public static string GetSkillNameKr(string en)
    {
        List<string> skillName_kr = new()
        {
            "����", "Ž��", "����", "����", "�Ժ�", "�г�", "����"
        };

        int index = -1;
        switch (en)
        {
            case "pride":
                index = 0;
                break;
            case "greed":
                index = 1;
                break;
            case "lust":
                index = 2;
                break;
            case "envy":
                index = 3;
                break;
            case "glutny":
                index = 4;
                break;
            case "wrath":
                index = 5;
                break;
            case "sloth":
                index = 6;
                break;
        }

        if (index == -1)
        {
            Debug.Log($"skillName {en} is not exist");
            return "";
        }
        return skillName_kr[index];
    }

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
