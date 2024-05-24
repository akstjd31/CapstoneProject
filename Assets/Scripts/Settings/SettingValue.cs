using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingValue : MonoBehaviour
{
    private static GameObject settingParent;
    private static GameObject nowSettingOption;
    private static GameObject nowSettingObject;
    //private int innerValue;
    private static int beforeMenuOption = -1;

    private static Dictionary<string, int> value1;
    private static Dictionary<string, int> value2;
    private const float MAX_SLIDER_VALUE = 100f;

    public static void Init()
    {
        settingParent = GameObject.Find("Setting_Detail");

        //기본 값
        value1 = new Dictionary<string, int>
        {
            ["sound_effect"] = 50,
            ["sound_bgm"] = 50
        };
        value2 = new Dictionary<string, int>
        {
            ["Performance_Toggle"] = 2,
            ["Temp01_Toggle"] = 2,
            ["Temp02_Toggle"] = 2
        };

        SaveDictionaryData("gameSetting_01", value1);
        SaveDictionaryData("gameSetting_02", value2);
        Debug.Log("end of settingValue's awake");
    }

    //setting detail이 변할 때 마다 실행
    public static void LoadSettingValue(int type)
    {
        //각 설정 ui에 value 값을 적용
        switch (type)
        {
            case 0:
                value1 = LoadDictionaryData("gameSetting_01");

                //GameObject.Find("SoundEffect_Value");
                if (value1 != null && value1.ContainsKey("sound_effect"))
                {
                    nowSettingObject = GameObject.Find("SoundEffect_Value");

                    if(nowSettingObject != null)
                    {
                        Slider innerValue = nowSettingObject.GetComponent<Slider>();

                        if(innerValue != null)
                        {
                            Debug.Log($"load sound_effect : {value1["sound_effect"]} / {innerValue.gameObject.name}");
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

                if(value1 != null && value1.ContainsKey("sound_bgm"))
                {
                    nowSettingObject = GameObject.Find("bgSound_Value");

                    if(nowSettingObject != null)
                    {
                        Slider innerValue = nowSettingObject.GetComponent<Slider>();

                        if(innerValue != null)
                        {
                            Debug.Log($"load sound_effect : {value1["sound_bgm"]} / {innerValue.gameObject.name}");
                            //innerValue.value = (float)value["sound_bgm"] / MAX_SLIDER_VALUE;
                            Canvas.ForceUpdateCanvases();
                            //innerValue.value = value.ContainsKey("sound_bgm") ? value["sound_bgm"] : 50;
                        }
                    }
                }

                break;
            case 1:
                value2 = LoadDictionaryData("gameSetting_02");
                ToggleGroup toggleGroup;
                Toggle toggleToClick;
                int num;

                //KeyNotFoundException: The given key 'Performance_Toggle' was not present in the dictionary.
                Debug.Log($"value's key : {string.Join(", ", value2.Keys)}");
                Debug.Log($"load value in case1 : {value2["Performance_Toggle"]}\n{value2["Temp01_Toggle"]}\n{value2["Temp02_Toggle"]}");

                if (value2 != null && value2.ContainsKey("Performance_Toggle"))
                {
                    num = value2["Performance_Toggle"];

                    toggleGroup = GameObject.Find("Performance_Toggle").GetComponent<ToggleGroup>();
                    toggleToClick = toggleGroup.transform.GetChild(num).GetComponent<Toggle>();
                    toggleToClick.isOn = true;
                }
                if (value2 != null && value2.ContainsKey("Temp01_Toggle"))
                {
                    num = value2["Temp01_Toggle"];

                    toggleGroup = GameObject.Find("Temp01_Toggle").GetComponent<ToggleGroup>();
                    toggleToClick = toggleGroup.transform.GetChild(num).GetComponent<Toggle>();
                    toggleToClick.isOn = true;
                }
                if (value2 != null && value2.ContainsKey("Temp02_Toggle"))
                {
                    num = value2["Temp02_Toggle"];

                    toggleGroup = GameObject.Find("Temp02_Toggle").GetComponent<ToggleGroup>();
                    toggleToClick = toggleGroup.transform.GetChild(num).GetComponent<Toggle>();
                    toggleToClick.isOn = true;
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
                    //Debug.Log("settingEffectSound");
                    GameObject soundEffectValueObject = settingEffectSound.transform.Find("SoundEffect_Value").gameObject;

                    if (soundEffectValueObject != null)
                    {
                        //Debug.Log("soundEffectValueObject");
                        Slider slider = soundEffectValueObject.GetComponent<Slider>();
                        //Debug.Log($"slider {slider}\n{slider.name}\n{slider.maxValue}\n{slider.minValue}\n{slider.value}");
                        //Debug.Log($"value : {value1}");

                        if (slider != null)
                        {
                            Debug.Log($"save sound_effect : {slider.value}");
                            //value["sound_effect"] = (int)slider.value;
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
                            //value["sound_bgm"] = (int)slider.value;
                        }
                    }
                }
                SaveDictionaryData("gameSetting_01", value1);

                break;
            case 1:
                //Dictionary<string, int> settingValue = new Dictionary<string, int>();

                // 각 토글 그룹을 저장할 변수들
                ToggleGroup[] toggleGroups = new ToggleGroup[3];
                toggleGroups[0] = GameObject.Find("Performance_Toggle").GetComponent<ToggleGroup>();
                toggleGroups[1] = GameObject.Find("Temp01_Toggle").GetComponent<ToggleGroup>();
                toggleGroups[2] = GameObject.Find("Temp02_Toggle").GetComponent<ToggleGroup>();

                // 각 토글 그룹에 대해 반복
                for (int i = 0; i < toggleGroups.Length; i++)
                {
                    ToggleGroup toggleGroup = toggleGroups[i];
                    int temp = 0;

                    // 토글 그룹 내의 토글들을 반복하면서 체크된 토글을 찾음
                    foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
                    {
                        if (toggle.isOn)
                        {
                            // 체크된 토글을 찾았을 때 해당 인덱스를 settingValue 딕셔너리에 추가
                            string name = "";
                            switch(i)
                            {
                                case 0:
                                    name = "Performance_Toggle";
                                    break;
                                case 1:
                                    name = "Temp01_Toggle";
                                    break;
                                case 2:
                                    name = "Temp02_Toggle";
                                    break;
                            }


                            value2[name] = temp;
                            break;
                        }
                        else
                        {
                            temp++;
                        }
                    }
                }

                //Debug.Log($"settingValue : {settingValue["Performance_Toggle"]}\n{settingValue["Temp01_Toggle"]}\n{settingValue["Temp02_Toggle"]}");
                SaveDictionaryData("gameSetting_02", value2);

                break;
        }
    }


    //직렬화, 역 직렬화
    private static void SaveDictionaryData(string key, Dictionary<string, int> dictionary)
    {
        string json = JsonUtility.ToJson(dictionary);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
        Debug.Log($"save data origin {json}");
        Debug.Log($"save data {string.Join(", ", dictionary.Keys)}");
    }

    private static Dictionary<string, int> LoadDictionaryData(string key)
    {
        string json = PlayerPrefs.GetString(key);
        Dictionary<string, int> dictionary = JsonUtility.FromJson<Dictionary<string, int>>(json);
        Debug.Log($"load data origin {json}");
        Debug.Log($"load data {string.Join(", ", dictionary.Keys)}");
        return dictionary;
    }
}
