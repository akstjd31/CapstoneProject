using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class HealItem : MonoBehaviour
{
    public int healRate = 50;

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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Status status = other.GetComponent<Status>();
            
            status.HP += healRate;
            if(status.HP > status.MAXHP)
            {
                status.HP = status.MAXHP;
            }

            uiManager.PlayHealSound();
            this.GetComponent<PhotonView>().RPC("DestroyObj", RpcTarget.All);
        }
    }
}
