using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private GameObject settingUI;
    private GameObject settingDetailUI;

    [SerializeField]
    private GameObject detailPrefab_01;
    [SerializeField]
    private GameObject detailPrefab_02;
    [SerializeField]
    private GameObject detailPrefab_03;

    private GameObject nowPrefab;
    private GameObject spawnedPrefab;

    private int beforeMenuOption = -1;
    private int nowMenuOption = 0;

    private void Start()
    {
        SettingValue.Init();

        settingUI = GameObject.Find("Setting_Panel");
        settingDetailUI = GameObject.Find("Setting_Detail");
        settingUI.SetActive(false);
    }

    public void ClickMenuOption()
    {
        string buttonName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        //Debug.Log($"name : {buttonName}");
        int btn_type = 0;

        switch(buttonName)
        {
            case "Menu_Option_01":
                btn_type = 0;
                break;
            case "Menu_Option_02":
                btn_type = 1;
                break;
            case "Menu_Option_03":
                btn_type = 2;
                break;
        }

        if (beforeMenuOption == btn_type)
        {
            return;
        }

        ShowMenuDetail(btn_type);
    }

    private void ShowMenuDetail(int opt = 0)
    {
        if (spawnedPrefab != null)
        {
            if(beforeMenuOption != -1)
            {
                SettingValue.SaveSettingValue(beforeMenuOption);
                Destroy(spawnedPrefab);
            }
        }
        nowMenuOption = opt;

        switch (opt)
        {
            case 0:
                nowPrefab = detailPrefab_01;
                break;
            case 1:
                nowPrefab = detailPrefab_02;
                break;
            case 2:
                nowPrefab = detailPrefab_03;
                break;
        }

        //Debug.Log($"ShowDetail {nowPrefab == detailPrefab_01} {nowPrefab == detailPrefab_02} {nowPrefab == detailPrefab_03}");
        spawnedPrefab = Instantiate(nowPrefab, settingDetailUI.transform);
        //Debug.Log("showDetail instantiate");

        beforeMenuOption = nowMenuOption;
        nowMenuOption = opt;

        //����Ǵ� ���� ��� ���� Ŭ����
        SettingValue.LoadSettingValue(opt);
    }

    public void LoadSettingUI()
    {
        settingUI.SetActive(true);
        ShowMenuDetail();
    }
    
    public void CloseSettingUI()
    {
        SettingValue.SaveSettingValue(nowMenuOption);
        if(spawnedPrefab != null)
        {
            Destroy(spawnedPrefab);
        }
        nowPrefab = null;
        settingUI.SetActive(false);
    }
}
