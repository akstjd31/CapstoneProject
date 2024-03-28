using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserRegister : MonoBehaviour
{
    public GameObject EmailRegisterPopup;
    private int errCode;    //�Է� �� �߻��� ������ ����

    void Start()
    {
        EmailRegisterPopup.SetActive(false);
    }

    public void ShowPopup()
    {
        EmailRegisterPopup.SetActive(true);
    }

    public void ClosePopup()
    {
        EmailRegisterPopup.SetActive(false);
    }

    public void SubmitRegister()
    {
        //���� �˾ƾ� �ϴ� �±��� �θ�
        string[] tagList = { "Register_Email_Input", "Register_Password_Input", "Register_Password_Confirm_Input", "Register_Nickname_Input" };
        GameObject parent;
        TextMeshProUGUI textComponent;
        string email = "", password = "", passwordConfirm = "", nickname = "";


        for (int i = 0; i < tagList.Length; i++)
        {
            parent = GameObject.Find(tagList[i]);

            if (parent == null)
                continue;

            textComponent = parent.GetComponentInChildren<TextMeshProUGUI>();
            //Debug.Log($"input text of {tagList[i]} : {textComponent.text}");

            if(textComponent.text == "")
            {
                //�� �׸��� ������ �� ����
                Debug.Log("empty field is exist");
                errCode = 1;
                return;
            }
            else if(i == 0 && !IsValidEmail(textComponent.text))
            {   //�̸��� ���� Ȯ��
                Debug.Log("The email address is badly formatted");
                errCode = 2;
                return;
            }

            switch(tagList[i])
            {
                case "Register_Email_Input":
                    email = textComponent != null ? textComponent.text : "null";
                    break;
                case "Register_Password_Input":
                    password = textComponent != null ? textComponent.text : "null";
                    break;
                case "Register_Password_Confirm_Input":
                    passwordConfirm = textComponent != null ? textComponent.text : "null";
                    break;
                case "Register_Nickname_Input":
                    nickname = textComponent != null ? textComponent.text : "null";
                    break;
            }
        }

        //��й�ȣ�� ��й�ȣ Ȯ���� ���� �����ؾ� �� & ���� ����(���̾�̽� ���� 6�̻�)
        if(!password.Equals(passwordConfirm) && password.Length > 7)
        {
            Debug.Log("password exception");
            return;
        }
        
        //�߰��� ����� ������ ����        //�г��� ��
        Dictionary<string, object> additionalData = new Dictionary<string, object>();
        additionalData.Add("nickname", nickname);


        UserData.SignInWithEmail_Password(email, password, additionalData);

        EmailRegisterPopup.SetActive(false);
    }

    static bool IsValidEmail(string email)
    {
        // ������ �̸��� ������ Ȯ���ϴ� ���� ǥ����
        string pattern = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";

        // �̸��� ���� Ȯ��
        return Regex.IsMatch(email, pattern);
    }
}
