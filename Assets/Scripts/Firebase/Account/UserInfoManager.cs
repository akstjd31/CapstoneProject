using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfoManager : MonoBehaviour
{
    public static UserInfoManager instance;
    public static FirebaseUser currentUser; // Firebase ����� ������ ������ ����

    void Awake()
    {
        // UserInfoManager�� �ٸ� ������ ��ȯ�Ǿ �ı����� �ʵ��� ��
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ����� ������ �����ϴ� �޼���
    public static void SetCurrentUser(FirebaseUser user)
    {
        currentUser = user;
        //Debug.Log($"set user {currentUser}");
    }

    public static FirebaseUser GetCurrentUser()
    {
        return currentUser;
    }
}
