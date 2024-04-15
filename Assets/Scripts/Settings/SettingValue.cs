using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingValue : MonoBehaviour
{
    private static GameObject settingParent;
    private static GameObject nowSettingOption;
    private static GameObject nowSettingObject;
    //private dynamic innerValue;
    private static int beforeMenuOption = -1;

    private static Dictionary<string, dynamic> value;

    private void Awake()
    {
        settingParent = GameObject.Find("Setting_Detail");
    }

    //setting detail�� ���� �� ���� ����
    public static void LoadSettingValue(int type)
    {
        if(beforeMenuOption == type)
        {
            return;
        }
        //value�� ��� null�� ���� �ذ� �ʿ�
        if(value == null)
        {
            value = new Dictionary<string, dynamic>();
            value["sound_effect"] = 50;
            value["sound_bgm"] = 50;
        }
        beforeMenuOption = type;

        LoadDictionaryData("gameSetting", value);
        Debug.Log($"load value : {value}");

        //�� ���� ui�� value ���� ����
        switch (type)
        {
            case 0:
                //GameObject.Find("SoundEffect_Value");
                if(value != null && value.ContainsKey("sound_effect"))
                {
                    nowSettingObject = GameObject.Find("SoundEffect_Value");

                    if(nowSettingObject != null)
                    {
                        Slider innerValue = nowSettingObject.GetComponent<Slider>();

                        if(innerValue != null)
                        {
                            //innerValue.value = 10;
                            Debug.Log($"value : {value["sound_effect"]}");
                           
                            //innerValue.value = value.ContainsKey("sound_effect") ? temp : 50;
                        }
                        else
                        {
                            Debug.Log("innerValue is null");
                        }
                    }
                    else
                    {
                        Debug.Log("nowSettingObject is null");
                    }
                }
                else
                {
                    Debug.Log("value is null");
                }

                if(value != null && value.ContainsKey("sound_bgm"))
                {
                    nowSettingObject = GameObject.Find("bgSound_Value");

                    if(nowSettingObject != null)
                    {
                        Slider innerValue = nowSettingObject.GetComponent<Slider>();

                        if(innerValue != null)
                        {
                            Debug.Log($"value2 : {value["sound_bgm"]}");
                            //innerValue.value = value.ContainsKey("sound_bgm") ? value["sound_bgm"] : 50;
                        }
                    }
                }

                break;
        }
    }
    
    //â�� ���� ���� �ٸ� �޴��� �Ѿ ��, ����
    public static void SaveSettingValue(int type)
    {
        if (beforeMenuOption == type)
        {
            return;
        }
        beforeMenuOption = type;

        switch (type)
        {
            case 0:
                GameObject settingEffectSound = GameObject.Find("Setting_EffectSound");

                if (settingEffectSound != null)
                {
                    GameObject soundEffectValueObject = settingEffectSound.transform.Find("SoundEffect_Value").gameObject;

                    if (soundEffectValueObject != null)
                    {
                        Slider slider = soundEffectValueObject.GetComponent<Slider>();

                        if (slider != null)
                        {
                            value["sound_effect"] = slider.value;
                        }
                    }
                }

                GameObject settingBgSound = GameObject.Find("Setting_bgSound");

                if (settingBgSound != null)
                {
                    GameObject soundEffectValueObject = settingBgSound.transform.Find("bgSound_Value").gameObject;

                    if (soundEffectValueObject != null)
                    {
                        Slider slider = soundEffectValueObject.GetComponent<Slider>();

                        if (slider != null)
                        {
                            value["sound_bgm"] = slider.value;
                        }
                    }
                }


                break;
        }

        Debug.Log($"save value : {value}");
        SaveDictionaryData("gameSetting", value);
    }


    //����ȭ, �� ����ȭ
    private static void SaveDictionaryData(string key, Dictionary<string, dynamic> dictionary)
    {
        string json = JsonUtility.ToJson(dictionary);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    private static void LoadDictionaryData(string key, Dictionary<string, dynamic> dictionary)
    {
        string json = PlayerPrefs.GetString(key);
        dictionary = JsonUtility.FromJson<Dictionary<string, dynamic>>(json);
    }
}
