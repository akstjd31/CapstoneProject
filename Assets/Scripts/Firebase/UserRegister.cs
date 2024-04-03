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
    public GameObject Error_Textbox;
    private int errCode;    //�Է� �� �߻��� ������ ����
    //1 : �� �׸� ����, 2 : �̸��� ���� ����, 3 : ��й�ȣ ����, ��ġ ����, 4: �����ϴ� ������ �����Ϸ� ��

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
        //�̹� �����ϴ� ������ ���
        bool SubmitRegister = await IsValidNewAccountAsync(email, password);
        if (!SubmitRegister)
        {
            errCode = 4;
            Debug.Log("Account is already exist");
        }

        return SubmitRegister;
    }

    public async void InnerSubmitRegister()
    {
        //���� �˾ƾ� �ϴ� �±��� �θ�
        string[] tagList = { "Register_Email_Input", "Register_Password_Input", "Register_Password_Confirm_Input" };
        GameObject parent;
        TextMeshProUGUI textComponent;
        string email = "", password = "", passwordConfirm = "", nickname = "";

        //�� �׸��� �ִ����� ���� Ȯ��
        for (int i = 0; i < tagList.Length; i++)
        {
            parent = GameObject.Find(tagList[i]);

            if (parent == null)
                continue;

            textComponent = parent.GetComponentInChildren<TextMeshProUGUI>();
            textComponent.text = textComponent.text.Trim();
            //Debug.Log($"textComponent{i} : {textComponent.text.CompareTo("")} {textComponent.text.Length}");

            //1 : �������, 2 : char 1�� �Է�
            if (textComponent.text.Length == 1)
            {
                //�� �׸��� ������ �� ����
                Debug.Log("empty field is exist");
                errCode = 1;
                SetErrorMsg(errCode);
                return;
            }
        }

        //�� �׸��� ��ȿ�� ���������� Ȯ��
        for (int i = 0; i < tagList.Length; i++)
        {
            parent = GameObject.Find(tagList[i]);

            if (parent == null)
                continue;

            textComponent = parent.GetComponentInChildren<TextMeshProUGUI>();
            //Debug.Log($"input text of {tagList[i]} : {textComponent.text}");

            if (i == 0 && !IsValidEmail(textComponent.text))
            {   //�̸��� ���� Ȯ��
                Debug.Log("The email address is badly formatted");
                errCode = 2;
                SetErrorMsg(errCode);
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

        //��й�ȣ�� ��й�ȣ Ȯ���� ���� �����ؾ� ��
        if(!password.Equals(passwordConfirm))
        {
            errCode = 3;
            SetErrorMsg(errCode);
            Debug.Log("password exception");
            return;
        }

        //��й�ȣ ���� ����(���̾�̽� ���� 6�̻�)
        if (!(password.Length > 5))
        {
            errCode = 4;
            SetErrorMsg(errCode);
            return;
        }

        var continueRegister = await IsExistAccount(email, password);

        if (!continueRegister)
        {
            errCode = 5;
            SetErrorMsg(errCode);
            Debug.Log("Account is already exist");
            return;
        }

        //���� �޽����� ���õ� ���� ���� ������ �ϴ� ��� ���� �޽��� ����
        SetErrorMsg(-1);

        //�߰��� ����� ������ ����        //�г��� ��
        Dictionary<string, object> additionalData = new();
        additionalData.Add("nickname", nickname);


        UserData.RegisterWithEmail_Password(email, password, additionalData);

        EmailRegisterPopup.SetActive(false);

        PhotonManager.ConnectWithRegister();
    }

    private void SetErrorMsg(int type)
    {
        string[] msgList = 
        { "�� �׸��� ������ �� �����ϴ�",
        "�߸��� �̸��� �����Դϴ�",
        "��й�ȣ�� ��ġ���� �ʽ��ϴ�",
        "��й�ȣ�� 6�ڸ� �̻����� �����Ǿ�� �մϴ�",
        "�̹� ���Ե� �̸����Դϴ�"};

        Error_Textbox.GetComponent<Text>().text = type != -1 ? msgList[type - 1] : "";
    }

    static bool IsValidEmail(string email)
    {
        // ������ �̸��� ������ Ȯ���ϴ� ���� ǥ����
        string pattern = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";

        // �̸��� ���� Ȯ��
        return Regex.IsMatch(email, pattern);
    }

    //���� ������ �������� Ȯ��
    public static async Task<bool> IsValidNewAccountAsync(string email, string password)
    {
        var auth = FirebaseAuth.DefaultInstance;

        try
        {
            var createUserResult = await auth.CreateUserWithEmailAndPasswordAsync(email, password);

            // ����� ������ ���������� �Ϸ�Ǿ�����
            if (createUserResult != null && createUserResult.User != null)
            {
                // ����� ����
                var user = createUserResult.User;
                await user.DeleteAsync();

                //Debug.Log("User deleted successfully!");
                return true;
            }
            else
            {
                // ����� ������ ������ ���
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
    
    //���� ������ �������� Ȯ��
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

            //Ȯ���� ���� ������ ������ ��� �ٷ� ����
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

                // ����� ������ ������ ���
                Debug.Log("User deleted successfully!");
            });

        });

        return true;
    }
}
