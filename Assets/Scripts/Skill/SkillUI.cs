    using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    private GameObject canvas;
    [SerializeField]    //prefab
    private GameObject SkillCanvas;
    [SerializeField]    //prefab
    private GameObject skillContainer;

    private int skillCount = -1;
    private GameObject skillUI;
    private GameObject[] skillRow;

    private Transform contentTransform;
    List<string> skillNameList = new();
    List<int> skillLevelList = new();

    Text skillTitle, skillDesc;

    private int nowSkillClassify = 0;   //0 : common, 1 : warrior/archer

    void Start()
    {
        Init();
    }

    public void Init()
    {
        canvas = GameObject.Find("Canvas");
        skillCount = SkillData.GetSkillCount();
        skillRow = new GameObject[skillCount];

        //summon UI
        skillUI = Instantiate(SkillCanvas);
        skillUI.transform.SetParent(canvas.transform, false);
        contentTransform = skillUI.transform.Find("SkillView/Viewport/Content");  //skillContainer's parent

        if (contentTransform == null)
        {
            Debug.Log("Content Transform not found");
            return;
        }

        RectTransform skillRectTransform = skillUI.GetComponent<RectTransform>();
        skillRectTransform.anchoredPosition = Vector2.zero;

        GameObject.Find("Menu_Common").GetComponent<Toggle>().isOn = true;
        Toggle commonMenu = GameObject.Find("Menu_Common").GetComponent<Toggle>();
        Toggle classMenu = GameObject.Find("Menu_Class").GetComponent<Toggle>();


        commonMenu.onValueChanged.AddListener(delegate { ClickSkillMenu(); });
        classMenu.GetComponent<Toggle>().onValueChanged.AddListener(delegate { ClickSkillMenu(); });

        //GameObject.Find("Menu_Common").GetComponentInChildren<Image>().color = HexToColor("#FFFFFFCC");
        //GetSkillData();

        SummonUI_Sub();
    }

    public void Re_Init()
    {
        //num of skill
        skillCount = SkillData.GetSkillCount(nowSkillClassify);

        if(skillRow != null)
        {
            // 기존의 UI 요소들을 제거합니다.
            foreach (GameObject row in skillRow)
            {
                Destroy(row);
            }
        }
        
        skillRow = new GameObject[skillCount];
        SummonUI_Sub();
    }

    public void ClickSkillMenu()
    {
        //GameObject.Find("Menu_Common").GetComponentInChildren<Image>().color = HexToColor("#C8C8C8CC");
        if (GameObject.Find("Menu_Common").GetComponent<Toggle>().isOn)
        {
            nowSkillClassify = 0;
        }
        else if(GameObject.Find("Menu_Class").GetComponent<Toggle>().isOn)
        {
            nowSkillClassify = 1;
        }

        //GetSkillData();
        Re_Init();
    }

    private void SummonUI_Sub()
    {
        for (int i = 0; i < skillCount; i++)
        {
            int index = i;

            skillRow[i] = Instantiate(skillContainer);
            skillRow[i].transform.SetParent(contentTransform.transform, false);
            skillRow[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250 * i);

            Button skill_detail = skillRow[i].GetComponent<Button>();
            if (skill_detail == null)
            {
                Debug.Log($"detail btn is null");
            }

            Button upgrade_btn = skillRow[i].transform.Find("Skill_Upgrade").GetComponent<Button>();
            if(upgrade_btn == null)
            {
                Debug.Log($"btn is null");
            }

            skill_detail.onClick.AddListener(delegate { ShowSkillDesc(index); });
            upgrade_btn.onClick.AddListener(delegate { UpgradeSkill(index); });
        }

        SetData();
    }

    private void UpgradeSkill(int index)
    {
        string skillName = skillNameList[index];
        int skillNum = SkillData.Skill_NameToNum(skillName);

        //Debug.Log($"pressed Upgrade => index : {index} {skillName} {skillNum}");
        CharSkill.LevelUpSkill(skillNum);
        SetData();
    }

    private void ShowSkillDesc(int index)
    {
        string skillName = skillNameList[index];
        int skillNum = SkillData.Skill_NameToNum(skillName);

        skillTitle = GameObject.Find("SkillDescTitle").GetComponent<Text>();
        skillDesc = GameObject.Find("SkillDesc").GetComponent<Text>();

        skillTitle.text = skillNameList[index];
        skillDesc.text = SkillData.GetSkillDesc(skillNum);
    }

    //로그인을 해야 값을 불러올 수 있음
    private async Task GetSkillData()
    {
        skillNameList = SkillData.GetSkillNameList(nowSkillClassify);
        skillLevelList = await CharSkill.GetSkillLevelAll(nowSkillClassify);

        //Debug.Log($"GetSkillData // skillNameList {skillNameList.Count}, skillLevelList {skillLevelList.Count}");
    }

    private async void SetData()
    {
        await GetSkillData();

        Transform title, level;
        string text_level = "레벨 ";
        int maxSkillLevel = 5;
        //Debug.Log($"setdata skillLevelList len : {skillLevelList.Count}");

        //이미지, 이름, 레벨, 설명
        for (int i = 0; i < skillRow.Length; i++)
        {
            //name
            title = skillRow[i].transform.Find("SkillName");
            level = skillRow[i].transform.Find("Skill_Level");
            maxSkillLevel = 5;

            if (title == null)
            {
                Debug.Log($"title {i} is null");
            }

            title.GetComponent<Text>().text = skillNameList[i];
            level.GetComponent<Text>().text = text_level + skillLevelList[i] + "/" + maxSkillLevel;

        }
    }

    private Color HexToColor(string hex)
    {
        Color color = new Color();
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}
