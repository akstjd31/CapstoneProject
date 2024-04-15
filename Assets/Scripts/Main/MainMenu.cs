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
        settingUI = GameObject.Find("Setting_Panel");
        settingDetailUI = GameObject.Find("Setting_Detail");
        settingUI.SetActive(false);
    }

    public void ClickMenuOption()
    {
        string buttonName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        Debug.Log($"name : {buttonName}");
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
        beforeMenuOption = btn_type;
        nowMenuOption = btn_type;

        SettingValue.SaveSettingValue(btn_type);
        ShowMenuDetail(btn_type);
    }

    private void ShowMenuDetail(int opt = 0)
    {
        if (spawnedPrefab != null)
        {
            //변경된 값을 저장
            SettingValue.SaveSettingValue(opt);
            Destroy(spawnedPrefab);
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

        spawnedPrefab = Instantiate(nowPrefab, settingDetailUI.transform);

        //변경되는 값을 얻기 위한 클래스
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
        settingUI.SetActive(false);
    }
}
