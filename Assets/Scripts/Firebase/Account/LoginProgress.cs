using Firebase;
using Firebase.Auth;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginProgress : MonoBehaviour
{
    FirebaseAuth auth;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance;
            }
            else
            {
                Debug.LogError("Could not resolve Firebase dependencies: " + task.Result);
                return;
            }
        });
    }

    public void SigninWithEmail()
    {
        auth = FirebaseAuth.DefaultInstance;

        string email = GameObject.Find("InputEmaiil").GetComponentInChildren<Text>().text.Trim();
        string password = GameObject.Find("InputPassword").GetComponentInChildren<Text>().text.Trim();

        auth.SignInWithEmailAndPasswordAsync(email, password);
    }

    private async void SigninWithEmailAsync()
    {
        auth = FirebaseAuth.DefaultInstance;

        string email = GameObject.Find("InputEmaiil").GetComponentInChildren<Text>().text.Trim();
        string password = GameObject.Find("InputPassword").GetComponentInChildren<Text>().text.Trim();

        AuthResult result;

        //로그인 시도
        try
        {
            Debug.Log($"auth2 : {auth.CurrentUser}\n{email}\t{password}");
            //await auth.SignInWithEmailAndPasswordAsync(email, password);

            // SignInWithEmailAndPasswordAsync 메서드를 비동기적으로 호출하고 결과를 기다림
            result = await auth.SignInWithEmailAndPasswordAsync(email, password);

            Debug.Log("end of sign in");
        }
        catch (Exception ex)
        {
            Debug.Log($"auth2 failed : {ex.StackTrace}");
            Debug.Log($"auth2 failed : {ex.Message}");
            return;
        }
    }

    public void BreakLogin()
    {
        //SceneManager.LoadScene("SelectCharacter");
        PhotonManager.ConnectWithRegister();
    }
}
