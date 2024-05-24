using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

/////////////////////////////////
//// 던전에 들어간 이후에 사용될 스크립트

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance = null;
    public GameObject localPlayer; // 로컬 플레이어
    public GameObject remotePlayer; // 원격 플레이어

    PhotonView[] photonViews;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (localPlayer == null || remotePlayer == null)
        {
            PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
            foreach (var view in photonViews)
            {
                if (view.IsMine)
                {
                    localPlayer = view.gameObject;
                }
                else
                {
                    remotePlayer = view.gameObject;
                }
            }
        }
    }

    // 플레이어 생성 완료 시 호출되는 콜백
    // public override void OnJoinedRoom()
    // {
    //     // 로컬 플레이어를 찾아서 변수에 할당
    //     localPlayer = GameObject.FindGameObjectWithTag("Player");

    //     // 다른 플레이어를 찾아서 변수에 할당
    //     PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
    //     foreach (var view in photonViews)
    //     {
    //         if (view.IsMine == false)
    //         {
    //             remotePlayer = view.gameObject;
    //             break;
    //         }
    //     }
    // }
}
