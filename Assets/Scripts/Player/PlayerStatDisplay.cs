using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerStatDisplay : MonoBehaviour
{
    public Text hpText, weaponText, attackDamageText, attackSpeedText;
    PlayerCtrl playetCtrl;
    Status status;

    void Start()
    {

    }
    
    void LateUpdate()
    {
        if (playetCtrl == null)
        {
            if (GameObject.FindGameObjectWithTag("Player"))
            {
                PhotonView photonView = GameObject.FindGameObjectWithTag("Player").GetComponent<PhotonView>();

                if (photonView.IsMine)
                {
                    playetCtrl = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerCtrl>();
                    status = GameObject.FindGameObjectWithTag("Playe;").GetComponent<Status>();
                }
                    
            }
        }
        else
        {
            UpdateText(status.HP, playetCtrl.weaponName, status.attackDamage, status.attackSpeed);
        }
    }

    public void UpdateText(int HP, string weaponName, int attackDamage, float attackSpeed)
    {
        hpText.text = "HP: " + HP;
        weaponText.text = "Weapon: " + weaponName;
        attackDamageText.text = "AttackDamage: " + attackDamage;
        attackSpeedText.text = "AttackSpeed: " + attackSpeed.ToString("F1");
    }
}
