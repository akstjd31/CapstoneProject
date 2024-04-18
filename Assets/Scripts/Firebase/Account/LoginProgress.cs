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
    Credential credential;

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
        /*
        auth = FirebaseAuth.DefaultInstance;

        string email = GameObject.Find("InputEmaiil").GetComponentInChildren<Text>().text.Trim();
        string password = GameObject.Find("InputPassword").GetComponentInChildren<Text>().text.Trim();

        auth.SignInWithEmailAndPasswordAsync(email, password);
        */

        SigninWithEmailAsync2();
    }

    private async void ForceLoginWithCred()
    {
        auth = FirebaseAuth.DefaultInstance;
        credential = EmailAuthProvider.GetCredential("ggem@gmail.com", "qweqwe123@@qwe");
        Debug.Log($"credential : {credential.IsValid()}");

        await auth.SignInWithCredentialAsync(credential);
        Debug.Log("complete login");
    }

    private async void SigninWithEmailAsync2()
    {
        auth = FirebaseAuth.DefaultInstance;

        string email = GameObject.Find("InputEmaiil").GetComponentInChildren<Text>().text;
        string password = GameObject.Find("InputPassword").GetComponentInChildren<Text>().text;

        var authTask = await auth.SignInWithEmailAndPasswordAsync(email, password);
        Debug.Log($"authTask : {authTask}");
    }

    private async void SigninWithEmailAsync()
    {
        auth = FirebaseAuth.DefaultInstance;

        string email = GameObject.Find("InputEmaiil").GetComponentInChildren<Text>().text.Trim();
        string password = GameObject.Find("InputPassword").GetComponentInChildren<Text>().text.Trim();

        AuthResult result;

        //�α��� �õ�
        try
        {
            Debug.Log($"auth2 : {auth.CurrentUser}\n{email}\t{password}");
            //await auth.SignInWithEmailAndPasswordAsync(email, password);

            // SignInWithEmailAndPasswordAsync �޼��带 �񵿱������� ȣ���ϰ� ����� ��ٸ�
            await auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
                if (task.IsCanceled)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                    return;
                }
                if (task.IsFaulted)
                {
                    Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                    return;
                }

                AuthResult result = task.Result;
                Debug.LogFormat("User signed in successfully: {0} ({1})",
                    result.User.DisplayName, result.User.UserId);
            });

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
