    using System.Collections;
using System.Collections.Generic;
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
        skillNameList = SkillData.GetSkillList(nowSkillClassify);

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

        skillNameList = SkillData.GetSkillList(nowSkillClassify);
        Re_Init();
    }

    private void SummonUI_Sub()
    {
        for (int i = 0; i < skillCount; i++)
        {
            skillRow[i] = Instantiate(skillContainer);
            skillRow[i].transform.SetParent(contentTransform.transform, false);
            skillRow[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250 * i);
        }

        SetData();
    }

    public void ClickSkill()
    {
        
    }

    private void SetData()
    {
        Transform title;

        //이미지, 이름, 레벨, 설명
        for (int i = 0; i < skillRow.Length; i++)
        {
            title = skillRow[i].transform.Find("SkillName");

            if(title == null)
            {
                Debug.Log($"title {i} is null");
            }

            title.GetComponent<Text>().text = skillNameList[i];
        }
    }

    private Color HexToColor(string hex)
    {
        Color color = new Color();
        ColorUtility.TryParseHtmlString(hex, out color);
        return color;
    }
}
