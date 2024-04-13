using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInfoManager : MonoBehaviour
{
    public static UserInfoManager instance;
    public static FirebaseUser currentUser; // Firebase 사용자 정보를 저장할 변수

    void Awake()
    {
        // UserInfoManager가 다른 씬으로 전환되어도 파괴되지 않도록 함
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

    // 사용자 정보를 설정하는 메서드
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
