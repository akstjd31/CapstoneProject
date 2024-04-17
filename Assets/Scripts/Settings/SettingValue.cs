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

    private static Dictionary<string, int> value;
    private const float MAX_SLIDER_VALUE = 100f;

    private void Awake()
    {
        settingParent = GameObject.Find("Setting_Detail");

        //�⺻ ��
        value = new Dictionary<string, int>
        {
            ["sound_effect"] = 50,
            ["sound_bgm"] = 50
        };
    }

    //setting detail�� ���� �� ���� ����
    public static void LoadSettingValue(int type)
    {
        //�� ���� ui�� value ���� ����
        switch (type)
        {
            case 0:
                LoadDictionaryData("gameSetting_01", value);

                //GameObject.Find("SoundEffect_Value");
                if (value != null && value.ContainsKey("sound_effect"))
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
            case 1:
                LoadDictionaryData("gameSetting_02", value);
                ToggleGroup toggleGroup;
                Toggle toggleToClick;
                int num;

                if (value != null && value.ContainsKey("Performance_Toggle"))
                {
                    num = value["settingQuality"];

                    toggleGroup = GameObject.Find("Performance_Toggle").GetComponent<ToggleGroup>();
                    toggleToClick = toggleGroup.transform.GetChild(num).GetComponent<Toggle>();
                    toggleToClick.isOn = true;
                }
                if (value != null && value.ContainsKey("Temp01_Toggle"))
                {
                    num = value["Temp01_Toggle"];

                    toggleGroup = GameObject.Find("Temp01_Toggle").GetComponent<ToggleGroup>();
                    toggleToClick = toggleGroup.transform.GetChild(num).GetComponent<Toggle>();
                    toggleToClick.isOn = true;
                }
                if (value != null && value.ContainsKey("Temp02_Toggle"))
                {
                    num = value["Temp02_Toggle"];

                    toggleGroup = GameObject.Find("Temp02_Toggle").GetComponent<ToggleGroup>();
                    toggleToClick = toggleGroup.transform.GetChild(num).GetComponent<Toggle>();
                    toggleToClick.isOn = true;
                }

                break;
        }

        //ui ���� ������Ʈ
        Canvas.ForceUpdateCanvases();
    }
    
    //â�� ���� ���� �ٸ� �޴��� �Ѿ ��, ����
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
                            value["sound_effect"] = (int)slider.value;
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
                            value["sound_bgm"] = (int)slider.value;
                        }
                    }
                }
                SaveDictionaryData("gameSetting_01", value);

                break;
            case 1:
                Dictionary<string, int> settingValue = new Dictionary<string, int>();

                // �� ��� �׷��� ������ ������
                ToggleGroup[] toggleGroups = new ToggleGroup[3];
                toggleGroups[0] = GameObject.Find("Performance_Toggle").GetComponent<ToggleGroup>();
                toggleGroups[1] = GameObject.Find("Temp01_Toggle").GetComponent<ToggleGroup>();
                toggleGroups[2] = GameObject.Find("Temp02_Toggle").GetComponent<ToggleGroup>();

                // �� ��� �׷쿡 ���� �ݺ�
                for (int i = 0; i < toggleGroups.Length; i++)
                {
                    ToggleGroup toggleGroup = toggleGroups[i];
                    int temp = 0;

                    // ��� �׷� ���� ��۵��� �ݺ��ϸ鼭 üũ�� ����� ã��
                    foreach (Toggle toggle in toggleGroup.GetComponentsInChildren<Toggle>())
                    {
                        if (toggle.isOn)
                        {
                            // üũ�� ����� ã���� �� �ش� �ε����� settingValue ��ųʸ��� �߰�
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


                            settingValue[name] = temp;
                            break;
                        }
                        else
                        {
                            temp++;
                        }
                    }
                }

                SaveDictionaryData("gameSetting_02", value);

                break;
        }
    }


    //����ȭ, �� ����ȭ
    private static void SaveDictionaryData(string key, Dictionary<string, int> dictionary)
    {
        string json = JsonUtility.ToJson(dictionary);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    private static void LoadDictionaryData(string key, Dictionary<string, int> dictionary)
    {
        string json = PlayerPrefs.GetString(key);
        dictionary = JsonUtility.FromJson<Dictionary<string, int>>(json);
    }
}