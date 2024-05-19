using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;

public class CharSkill : MonoBehaviour
{
    public static FirebaseUser currentUser;
    private static Dictionary<string, int> userSkill;  //스킬 데이터의 인스턴스
    private static Button[] btn_skill = new Button[7];  //버튼의 순서를 보장하지 않음
    private static List<string> skill_name = new()
    {
        "pride", "greed", "lust", "envy", "glutny", "wrath", "sloth"
        //교만, 탐욕, 색욕, 질투, 먹보, 분노, 나태
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
    [SerializeField]
    private GameObject btn_prefab;
    private static GameObject btn_Lobby_close;
    private static string col_name = "";

    private static string skill_point_text = "";
    private static Text point_text;

    private static GameObject dy_coll;

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
        point_text = GameObject.Find("SkillPoint").GetComponentInChildren<Text>();
        pc = FindObjectOfType<PlayerCtrl>();
        pc.DisableLobbyUI();

        for (int i = 0; i< btn_skill.Length; i++)
        {
            int index = i;

            btn_skill[i].onClick.AddListener(async () =>
            {
                skill_point = await UserInfoManager.GetSkillPoint();
                Debug.Log($"Call Upgrade with : {btn_skill[index].transform.parent.name}");
                UpgradeSkill2(btn_skill[index].transform.parent.name);
            });
            //Debug.Log($"{btn_skill[i].transform.parent.name}");
        }

        //explane.SetActive(false);
        explane = pc.GetSkillExplane();
        layerMask = LayerMask.GetMask("Skill_UI");
        //Invoke();

        pc.SetIsSkillUI(true);
    }

    private void Update()
    {
        pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        hit = Physics2D.Raycast(pos, Vector2.zero, Mathf.Infinity, layerMask);

        if (hit.collider != null)
        {
            //Debug.Log($"hit!! {hit.collider.name}");
            explane.SetActive(true);
            
            col_name = hit.collider.name;

            Explane_Pos.SetSkillExplanePos_SetMousePos();
        }
        else
        {
            //Debug.Log("no hit@@");
            col_name = "";
            explane.SetActive(false);
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            CloseSkillUI();
        }
    }

    public static string GetSkillDesc()
    {
        return SkillData.GetSkillDesc(
                SkillData.Skill_NameToNum(
                SkillData.GetSkillNameKr(col_name)));
    }

    private void OnDestroy()
    {
        pc.SetIsSkillUI(false);
        pc.EnableLobbyUI();
    }

    private static async Task InitSkill()
    {
        await UserInfoManager.GetCharSkillAsync();
        // 여기에 비동기 작업 완료 후 수행할 작업 추가
        userSkill = UserInfoManager.GetSkillLevel();
        Debug.Log($"Skill level initialized: ");
        Show_Dictionary(userSkill);
        skill_point = await UserInfoManager.GetSkillPoint();

        skill_point_text = "SKILL Point : " + skill_point;
        point_text.text = skill_point_text;
        //await SetLevelState();
        await SetNowSkillLevel();
    }

    public static async Task DecreaseSkillPoint()
    {
        skill_point = await UserInfoManager.GetSkillPoint();
        skill_point--;

        skill_point_text = "SKILL Point : " + skill_point;
        point_text.text = skill_point_text;

        await UserInfoManager.SetSkillPoint(skill_point);
        await SetNowSkillLevel();
    }

    private static async Task SetNowSkillLevel()
    {
        //reload
        await UserInfoManager.GetCharSkillAsync();

        Dictionary<string, int> skill = UserInfoManager.GetSkillLevel();
        Debug.Log("show skill");
        Show_Dictionary(skill);

        BoxCollider2D[] coll_List = GameObject.Find("Images").GetComponentsInChildren<BoxCollider2D>();
        GameObject[] skill_Level_text = new GameObject[7];

        //find level text = gameobject
        int index = 0;
        for (int i = 0; i < coll_List.Length; i++)
        {
            var parent = coll_List[i].gameObject;
            Transform levelTransform = parent.transform.Find("level");

            if (levelTransform != null)
            {
                skill_Level_text[index] = levelTransform.gameObject;
                index++;
            }

            // Ensure the array does not exceed its bounds
            if (index >= skill_Level_text.Length)
            {
                break;
            }
        }

        for (int i = 0; i < skill_Level_text.Length; i++)
        {
            string parent = skill_Level_text[i].transform.parent.name;
            index = -1;

            switch (parent)
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

            //Debug.Log($"parent index : {index}"); //0~6
            int skillKey_int = index + 1001;
            string skillKey_string = skillKey_int.ToString();

            Debug.Log($"skill level : {UserInfoManager.GetSkillLevelByKey(skillKey_string)}");

            skill_Level_text[i].GetComponent<TextMeshProUGUI>().text = "Lv. " + UserInfoManager.GetSkillLevelByKey(skillKey_string);
        }
    }

    public static async Task SetLevelState()
    {
        Debug.Log("call SetLevelState");
        await UserInfoManager.GetCharSkillAsync();
        userSkill = UserInfoManager.GetSkillLevel();

        BoxCollider2D[] images = GameObject.Find("Images").GetComponentsInChildren<BoxCollider2D>();

        GameObject[] list = new GameObject[images.Length];
        for(int i = 0; i < list.Length; i++)
        {
            list[i] = images[i].gameObject;
        }


        List<string> skillName_kr = new()
        {
            "교만", "탐욕", "색욕", "질투", "먹보", "분노", "나태"
        };

        List<int> value = await GetSkillLevelAll(0);

        string temp = "";
        for(int i = 0; i < value.Count; i++)
        {
            temp += value[i].ToString() + "_";
        }
        //Debug.Log($"value : {temp}");

        for(int i = 0; i < list.Length; i++)
        {
            int index = -1;

            //Debug.Log($"list name : {list[i].name} {list[i].transform.parent.name} {list[i].transform.parent.parent.name}");

            switch (list[i].name)
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
                default:
                    switch(list[i].transform.parent.name)
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
                    break;
            }

            //Debug.Log($"i : {i} => index : {index}");
            if(index == -1)
            {
                Debug.Log($"not valid skill : {list[i].name} {index}");
                return;
            }

            //Debug.Log($"{value[index]} {list[i].name}");

            var now = list[i].transform.Find("level");
            //Debug.Log($"now : {now}");

            now.GetComponent<TextMeshProUGUI>().text = "Lv." + value[index];
        }
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

        userSkill = UserInfoManager.GetSkillLevel();
        Show_Dictionary(userSkill);
        //Debug.Log($"skillNum in GetSkillLevel : {skillNum}");   //1001~1007
        //Debug.Log($"keys : {userSkill.Keys}");
        var keys = userSkill.Keys;
        string temp = "";

        foreach (string key in keys)
        {
            temp += key + " ";
        }
        Debug.Log($"keys : {temp}");    //keys : 1001 1002 1003 1004 1005 1006 1007 


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
    public static async Task LevelUpSkill(int skillNum)
    {
        Debug.Log($"LevelUpSkill : {skillNum}");
        /*
        if (!UserInfoManager.GetSkillLevel().ContainsKey(skillNum.ToString()))
        {
            // 존재하지 않는 경우 예외 발생
            throw new KeyNotFoundException($"Skill number {skillNum} does not exist.");
        }
        */

        await UserInfoManager.UpgradeSkill(skillNum.ToString(), UserInfoManager.GetSkillLevelByKey(skillNum.ToString()) + 1);

        //userSkill[skillNum.ToString()] = ++userSkill[skillNum.ToString()];
        //UpdataSkillData();
    }

    //UI에서 접근하는 메소드
    public static async void UpgradeSkill2(string skillName)
    {
        Debug.Log($"in UpgradeSkill2 : clicked {skillName}");

        string skillKr = SkillData.GetSkillNameKr(skillName);
        int skillNum = SkillData.Skill_NameToNum(skillKr);
        Debug.Log($"Skill kr : {skillKr} // skillNum : {skillNum}");

        await LevelUpSkill(skillNum);
    }

    //UI에서 접근하는 메소드
    //parameter는 영어 이름
    public static async void UpgradeSkill(string skillName)
    {
        if(skill_point < 1)
        {
            return;
        }

        //"pride", "greed", "lust", "envy", "glutny", "wrath", "sloth"
        //교만, 탐욕, 색욕, 질투, 먹보, 분노, 나태
        List<string> skillName_kr = new()
        {
            "교만", "탐욕", "색욕", "질투", "먹보", "분노", "나태"
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

        //parameter는 한글 이름
        int skillNum = SkillData.Skill_NameToNum(skillName_kr[index]);
        Debug.Log($"Upgrade by button => {skillNum} {skillName}");

        await UserInfoManager.SetSkillPoint(--skill_point);
        skill_point_text = "SKILL Point : " + skill_point;
        point_text.text = skill_point_text;

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

    //for build debug
    public static void SetHitName(string name)
    {
        if(point_text == null)
        {
            point_text = GameObject.Find("SkillPoint").GetComponentInChildren<Text>();
        }

        //point_text.text = name;
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

    private static readonly Dictionary<int, string> skill_desc = new()
    {
        {1001, "체력이 30%보다\n낮은 적에게 주는\n피해량 증가\n그 대신 적에게 받는\n피해량 10% 증가"},
        {1002, "골드 획득량 증가"},
        {1003, "피격 당하지 않고\n공격 5회 이상 시\n공격력 10% 증가"},
        {1004, "회피율 50%\n그 대신 피격 시\n플레이어와 팀원\n또한 같이 피격"},
        {1005, "공격 시\n현재 체력의 5퍼센트의\n피해량 흡혈\n체력의 50%\n이하가 되면\n피격 데미지 30% 증가"},
        {1006, "적에게 피격 시\n슈퍼 아머 시간 증가\n슈퍼 아머 시간 동안\n공격력 30퍼 증가"},
        {1007, "공격이\n장전 형식으로 변경\n1회 공격 후\n3초간 공격 불가\n그 대신 공격력 데미지의 200%"},
        {10001, "전사1 스킬 설명" },
        {10002, "전사2 스킬 설명" },
        {10003, "전사3 스킬 설명" },
        {10004, "전사4 스킬 설명" },
        {10005, "전사5 스킬 설명" },
        {20001, "아처1 스킬 설명" },
        {20002, "아처2 스킬 설명" },
        {20003, "아처3 스킬 설명" },
        {20004, "아처4 스킬 설명" },
        {20005, "아처5 스킬 설명" }
    };

    public static string GetSkillDesc(int skillNum)
    {
        return skill_desc[skillNum];
    }

    public static string GetSkillNameKr(string en)
    {
        List<string> skillName_kr = new()
        {
            "교만", "탐욕", "색욕", "질투", "먹보", "분노", "나태"
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
