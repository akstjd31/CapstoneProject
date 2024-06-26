using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class ChestController : MonoBehaviourPunCallbacks
{
    string mapDir = "Dungeon/";

    UIManager uiManager;
    // Start is called before the first frame update
    void Start()
    {
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    public void DestroyObj()
    {
        Destroy(this.gameObject);
    }

    public void ChestBreak()
    {
        int randInt = Random.Range(0, 11);
        if (randInt < 11)
        {
            PhotonNetwork.InstantiateRoomObject(mapDir + "Heart", this.transform.position, Quaternion.identity, 0);
        }

        uiManager.PlayChestSound();
        this.GetComponent<PhotonView>().RPC("DestroyObj", RpcTarget.All);
    }
}