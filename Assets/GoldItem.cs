using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GoldItem : MonoBehaviour
{
    private Status status1, status2;
    private float rand1, rand2;

    public AudioSource audioSource;
    public AudioClip goldSound;

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
    }

    public void SetStatus(Status status1, Status status2)
    {
        this.status1 = status1;
        this.status2 = status2;
    }

    [PunRPC]
    private void GoldRandom()
    {
        rand1 = Random.Range(0, 100f);
        rand2 = Random.Range(0, 100f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            this.GetComponent<PhotonView>().RPC("GoldRandom", RpcTarget.All);

            if (status1 != null && status2 != null)
            {
                status1.GoldPerLevel();
                status2.GoldPerLevel();

                status1.GetComponent<PhotonView>().RPC("RandomGoldAmount", RpcTarget.All);
                status2.GetComponent<PhotonView>().RPC("RandomGoldAmount", RpcTarget.All);


                if (rand1 >= 50.0f)
                {
                    status1.money += status1.goldEarnRate;
                }
                
                if (rand2 >= 50.0f)
                {
                    status2.money += status2.goldEarnRate;
                }
                
            }
            else
            {
                if (status1 != null)
                {
                    status1.GoldPerLevel();

                    status1.GetComponent<PhotonView>().RPC("RandomGoldAmount", RpcTarget.All);

                    if (rand1 >= 50.0f)
                    {
                        status1.money += status1.goldEarnRate;
                    }
                }
                else
                {
                    status2.GoldPerLevel();

                    status2.GetComponent<PhotonView>().RPC("RandomGoldAmount", RpcTarget.All);

                    if (rand2 >= 50.0f)
                    {
                        status2.money += status2.goldEarnRate;
                    }
                }
            }

            audioSource.PlayOneShot(goldSound);
            PhotonNetwork.Destroy(this.gameObject);
        }
    }
}
