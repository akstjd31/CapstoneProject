using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserRegister : MonoBehaviour
{
    public GameObject EmailRegisterPopup;
    private int errCode;    //입력 중 발생한 오류의 종류
    //1 : 빈 항목 존재, 2 : 이메일 형식 오류, 3 : 비밀번호 길이, 일치 오류, 4: 존재하는 계정을 생성하려 함

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
        InnerSubmitRegister();
    }

    public async Task<bool> IsExistAccount(string email, string password)
    {
        //이미 존재하는 계정인 경우
        bool SubmitRegister = await IsValidAccountAsync(email, password);
        if (!SubmitRegister)
        {
            errCode = 4;
            Debug.Log("Account is already exist");
        }

        return SubmitRegister;
    }

    public async void InnerSubmitRegister()
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
            errCode = 3;
            Debug.Log("password exception");
            return;
        }

        var continueRegister = await IsExistAccount(email, password);

        if (!continueRegister)
        {
            errCode = 4;
            Debug.Log("Account is already exist");
            return;
        }

        //추가로 등록할 데이터 설정        //닉네임 등
        Dictionary<string, object> additionalData = new Dictionary<string, object>();
        additionalData.Add("nickname", nickname);


        UserData.RegisterWithEmail_Password(email, password, additionalData);

        EmailRegisterPopup.SetActive(false);

        PhotonManager.ConnectWithRegister();
    }

    static bool IsValidEmail(string email)
    {
        // 간단한 이메일 형식을 확인하는 정규 표현식
        string pattern = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";

        // 이메일 형식 확인
        return Regex.IsMatch(email, pattern);
    }

    //생성 가능한 계정인지 확인
    public static async Task<bool> IsValidAccountAsync(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;

        try
        {
            var createUserResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);

            // 사용자 생성이 성공적으로 완료되었으면
            if (createUserResult != null && createUserResult.User != null)
            {
                // 사용자 삭제
                var user = createUserResult.User;
                await user.DeleteAsync();

                //Debug.Log("User deleted successfully!");
                return true;
            }
            else
            {
                // 사용자 생성에 실패한 경우
                Debug.LogError("Failed to create user");
                return false;
            }
        }
        catch (Exception e)
        {
            Debug.Log($"IsValidAccountAsync Exception : ${e}");
            return false;
        }
    }
    
    //생성 가능한 계정인지 확인
    public static bool IsValidAccount(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

            //확인을 위한 계정이 생성된 경우 바로 삭제
            FirebaseUser user = task.Result.User;
            user.DeleteAsync().ContinueWith(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.LogError("DeleteAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("DeleteAsync encountered an error: " + task.Exception);
                    return;
                }

                // 사용자 삭제가 성공한 경우
                Debug.Log("User deleted successfully!");
            });

        });

        return true;
    }
}
