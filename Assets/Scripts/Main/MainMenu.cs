using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private GameObject settingUI;

    private void Start()
    {
        settingUI = GameObject.Find("Setting_Panel");
        settingUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickMenuOption()
    {
        string buttonName = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        Debug.Log($"name : {buttonName}");
    }

    public void LoadSettingUI()
    {
        settingUI.SetActive(true);
    }
    
    public void CloseSettingUI()
    {
        settingUI.SetActive(false);
    }
}
