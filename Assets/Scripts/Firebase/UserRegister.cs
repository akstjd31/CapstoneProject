using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserRegister : MonoBehaviour
{
    public GameObject EmailRegisterPopup;
    private int errCode;    //입력 중 발생한 오류의 종류

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
        //값을 알아야 하는 태그의 부모
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
                //빈 항목이 존재할 수 없음
                Debug.Log("empty field is exist");
                errCode = 1;
                return;
            }
            else if(i == 0 && !IsValidEmail(textComponent.text))
            {   //이메일 형식 확인
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

        //비밀번호와 비밀번호 확인의 값은 동일해야 함 & 길이 제한(파이어베이스 기준 6이상)
        if(!password.Equals(passwordConfirm) && password.Length > 7)
        {
            Debug.Log("password exception");
            return;
        }
        
        //추가로 등록할 데이터 설정        //닉네임 등
        Dictionary<string, object> additionalData = new Dictionary<string, object>();
        additionalData.Add("nickname", nickname);


        UserData.SignInWithEmail_Password(email, password, additionalData);

        EmailRegisterPopup.SetActive(false);
    }

    static bool IsValidEmail(string email)
    {
        // 간단한 이메일 형식을 확인하는 정규 표현식
        string pattern = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";

        // 이메일 형식 확인
        return Regex.IsMatch(email, pattern);
    }
}
