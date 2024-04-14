using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingValue : MonoBehaviour
{
    private GameObject settingParent;
    private GameObject nowSettingOption;
    private GameObject nowSettingObject;
    //private dynamic innerValue;

    private Dictionary<string, dynamic> value = null;

    private void Awake()
    {
        settingParent = GameObject.Find("Setting_Detail");
        value = new();
    }

    //setting detail이 변할 때 마다 실행
    public void LoadSettingValue(int type)
    {
        value = LoadDictionaryData("gameSetting");

        //각 설정 ui에 value 값을 적용
        switch (type)
        {
            case 0:
                //GameObject.Find("SoundEffect_Value");
                if(value != null && value.ContainsKey("SoundEffect_Value"))
                {
                    nowSettingObject = GameObject.Find("bgSound_Value");

                    if(nowSettingObject != null)
                    {
                        Slider innerValue = nowSettingObject.GetComponent<Slider>();

                        if(innerValue != null)
                        {
                            Debug.Log("");
                        }
                    }
                }


                //GameObject.Find("bgSound_Value").GetComponent<Slider>().value = value.ContainsKey("sound_bgm") ? 50 : value["sound_bgm"];
                break;
        }
    }
    
    //창을 닫을 때와 다른 메뉴로 넘어갈 때, 저장
    public void SaveSettingValue(int type)
    {
        switch(type)
        {
            case 0:
                value["sound_effect"] = GameObject.Find("SoundEffect_Value").GetComponent<Slider>().value;
                value["sound_bgm"] =    GameObject.Find("bgSound_Value").GetComponent<Slider>().value;
                break;
        }

        SaveDictionaryData("gameSetting", value);
    }


    //직렬화, 역 직렬화
    private void SaveDictionaryData(string key, Dictionary<string, dynamic> dictionary)
    {
        string json = JsonUtility.ToJson(dictionary);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    private Dictionary<string, dynamic> LoadDictionaryData(string key)
    {
        string json = PlayerPrefs.GetString(key);
        return JsonUtility.FromJson<Dictionary<string, dynamic>>(json);
    }
}
