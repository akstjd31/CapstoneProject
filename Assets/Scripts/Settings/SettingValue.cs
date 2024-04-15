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
    private const float MAX_SLIDER_VALUE = 100f;

    private void Awake()
    {
        settingParent = GameObject.Find("Setting_Detail");

        //기본 값
        value["sound_effect"] = 50f;
        value["sound_bgm"] = 50f;
    }

    //setting detail이 변할 때 마다 실행
    public static void LoadSettingValue(int type)
    {
        LoadDictionaryData("gameSetting", value);

        //각 설정 ui에 value 값을 적용
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
                            Debug.Log($"load sound_effect : {value["sound_effect"]} / {innerValue.gameObject.name}");
                            //innerValue.value = (float)value["sound_effect"] / MAX_SLIDER_VALUE;
                            //nowSettingObject.GetComponent<Slider>().value = (float)value["sound_effect"];
                            Canvas.ForceUpdateCanvases();
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
                            Debug.Log($"load sound_effect : {value["sound_bgm"]} / {innerValue.gameObject.name}");
                            //innerValue.value = (float)value["sound_bgm"] / MAX_SLIDER_VALUE;
                            Canvas.ForceUpdateCanvases();
                            //innerValue.value = value.ContainsKey("sound_bgm") ? value["sound_bgm"] : 50;
                        }
                    }
                }

                break;
        }

        //ui 강제 업데이트
        Canvas.ForceUpdateCanvases();
    }
    
    //창을 닫을 때와 다른 메뉴로 넘어갈 때, 저장
    public static void SaveSettingValue(int type)
    {
        switch (type)
        {
            case 0:
                GameObject settingEffectSound = GameObject.Find("Setting_EffectSound");

                if (settingEffectSound != null)
                {
                    Debug.Log("settingEffectSound");
                    GameObject soundEffectValueObject = settingEffectSound.transform.Find("SoundEffect_Value").gameObject;

                    if (soundEffectValueObject != null)
                    {
                        Debug.Log("soundEffectValueObject");
                        Slider slider = soundEffectValueObject.GetComponent<Slider>();

                        if (slider != null)
                        {
                            Debug.Log($"save sound_effect : {slider.value}");
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
                            Debug.Log($"save sound_bgm : {slider.value}");
                            value["sound_bgm"] = slider.value;
                        }
                    }
                }


                break;
        }

        SaveDictionaryData("gameSetting", value);
    }


    //직렬화, 역 직렬화
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
