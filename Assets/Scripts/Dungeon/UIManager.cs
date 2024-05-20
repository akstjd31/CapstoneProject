using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class UIManager : MonoBehaviour
{
    public Status localPlayerStatus, remotePlayerStatus;
    // 원격 플레이어
    public HUD remoteHUD;

    // 로컬 플레이어
    public HUD localHUD;

    [SerializeField] private Inventory inventory;
    [SerializeField] private int activePlayers = -1;
    [SerializeField] private float removeItemInterval = 0.0f;
    private float startTime = 10.0f;

    public AudioSource audioSource;
    public AudioClip audioClip;
    public AudioClip chestSound;
    public AudioClip healSound;
    public AudioClip goldSound;
    public AudioClip[] arrowHitObstacle;
    public AudioClip jewelSound;

    public float rand;
    private bool flag = false;

    private void Start()
    {
        inventory = this.transform.Find("Inventory").GetComponent<Inventory>();
    }

    private void Update()
    {
        UpdateHUD();

        if (startTime > 0.0f)
        {
            startTime -= Time.deltaTime;
        }
        else if (localPlayerStatus == null && remotePlayerStatus == null && !flag)
        {
            flag = true;
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            
            if (players[0].GetComponent<PhotonView>().IsMine)
            {
                localPlayerStatus = players[0].GetComponent<Status>();
                remotePlayerStatus = players[1].GetComponent<Status>();


            }
            else
            {
                localPlayerStatus = players[1].GetComponent<Status>();
                remotePlayerStatus = players[0].GetComponent<Status>();
            }

            localHUD.hpBar.maxValue = this.localPlayerStatus.MAXHP;
            remoteHUD.hpBar.maxValue = this.remotePlayerStatus.MAXHP;

            // if (localPlayerStatus.gameObject.Equals(players[0]))
            // {
            //     localPlayerStatus = players[0].GetComponent<Status>();
            //     remotePlayerStatus = players[1].GetComponent<Status>();
            // }
            // else
            // {
            //     remotePlayerStatus = players[0].GetComponent<Status>();
            // }
        }
        else
        {
            if (GameObject.FindGameObjectsWithTag("Player").Length == 0)
            {
                if (!inventory.gameObject.activeSelf)
                {
                    inventory.gameObject.SetActive(true);
                }
                else
                {
                    ItemRecovery();
                }
            }
        }
    }

    [PunRPC]
    public void RandomFloat()
    {
        rand = Random.Range(0, 100f);
    }

    public void PlayChestSound()
    {
        audioSource.PlayOneShot(chestSound);
    }

    public void PlayHealSound()
    {
        audioSource.PlayOneShot(healSound);
    }

    public void PlayGoldSound()
    {
        audioSource.PlayOneShot(goldSound);
    }

    public void PlayJewelSound()
    {
        audioSource.PlayOneShot(jewelSound);
    }

    public void PlayArrowHitObstacleSound()
    {
        int rand = Random.Range(0, arrowHitObstacle.Length);

        audioSource.PlayOneShot(arrowHitObstacle[rand]);
    }

    // 아이템 회수
    private void ItemRecovery()
    {
        if (inventory.items.Count > 0)
        {
            if (removeItemInterval <= 0.0f)
            {
                inventory.items.Remove(inventory.items[inventory.items.Count - 1]);
                inventory.FreshSlot();
                audioSource.PlayOneShot(audioClip);
                removeItemInterval = 1.0f;
            }
            else
            {
                removeItemInterval -= Time.deltaTime;
            }
        }
    }

    private void UpdateHUD()
    {
        // 둘다 생존하고 있는 경우
        if (localPlayerStatus != null && remotePlayerStatus != null)
        {
            localHUD.hpBarText.text = string.Format("{0} / {1}", localPlayerStatus.HP, localPlayerStatus.MAXHP);
            
            localHUD.hpBar.value = this.localPlayerStatus.HP;

            remoteHUD.nickName.text = this.remotePlayerStatus.GetComponent<PhotonView>().Controller.NickName;
            remoteHUD.hpBarText.text = string.Format("{0} / {1}", remotePlayerStatus.HP, remotePlayerStatus.MAXHP);

            remoteHUD.hpBar.value = this.remotePlayerStatus.HP;
        }
        // 원격 플레이어 생존 시
        else if (remotePlayerStatus != null)
        {
            remoteHUD.nickName.text = this.remotePlayerStatus.GetComponent<PhotonView>().Controller.NickName;
            remoteHUD.hpBarText.text = string.Format("{0} / {1}", remotePlayerStatus.HP, remotePlayerStatus.MAXHP);

            remoteHUD.hpBar.value = this.remotePlayerStatus.HP;
        }

        // 로컬 플레이어 생존 시
        else if (localPlayerStatus != null)
        {
            localHUD.hpBarText.text = string.Format("{0} / {1}", localPlayerStatus.HP, localPlayerStatus.MAXHP);

            localHUD.hpBar.value = this.localPlayerStatus.HP;
        }
        else
        {

        }
    }
}
