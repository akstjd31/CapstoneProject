    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUI : MonoBehaviour
{
    private GameObject canvas;
    [SerializeField]    //prefab
    private GameObject SkillCanvas;
    [SerializeField]    //prefab
    private GameObject skillContainer;

    private int skillCount = 20;
    private int skill_inRow = 5;
    private GameObject skillUI;
    private GameObject[] skillRow;

    void Start()
    {
        canvas = GameObject.Find("Canvas");
        skillRow = new GameObject[skillCount / skill_inRow];

        SummonUI();
    }

    private void SummonUI()
    {
        skillUI = Instantiate(SkillCanvas);
        skillUI.transform.SetParent(canvas.transform, false);
        Transform contentTransform = skillUI.transform.Find("SkillView/Viewport/Content");  //skillContainer's parent

        if(contentTransform == null)
        {
            Debug.Log("null");
            return;
        }

        RectTransform skillRectTransform = skillUI.GetComponent<RectTransform>();
        skillRectTransform.anchoredPosition = Vector2.zero;

        for (int i = 0; i < skillCount / skill_inRow; i++)
        {
            skillRow[i] = Instantiate(skillContainer);
            skillRow[i].transform.SetParent(contentTransform.transform);
            skillRow[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -250 * i);
        }
    }
}
