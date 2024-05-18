using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class WarriorActiveSkill : ActiveSkill
{
    Collider2D[] colliders;
    float warriorActiveSkillCoolTime = 60.0f;
    float warriorActiveSkillDurationTime = 10.0f;
    float warriorActiveSkillRange = 8.0f;
    public GameObject effect;
    public Sprite warriorSkillSprite;
    string effectDir = "Effects/CharSkill/";

    // Start is called before the first frame update
    void Start()
    {
        Hud = GameObject.Find("LocalHUD").GetComponent<HUD>();
        if(Hud != null)
        {
            isInDungeon = true;
        }
        if(isInDungeon)
        {
            Hud.charSkillImage.sprite = warriorSkillSprite;
        }
        colliders = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(isInDungeon)
        {
            if(charSkillCoolTime > 0.0f)
            {
                Hud.charSkillCoolTime.canvasRenderer.SetAlpha(1f);
                Hud.charSkillCoolTime.raycastTarget = true;
                Hud.charSkillCoolTime.text = charSkillCoolTime.ToString("F1");
            }
            else
            {
                Hud.charSkillCoolTime.canvasRenderer.SetAlpha(0f);
                Hud.charSkillCoolTime.raycastTarget = false;
            }
            Hud.charSkillImage.fillAmount = (warriorActiveSkillCoolTime - charSkillCoolTime) / warriorActiveSkillCoolTime;
        }
        charSkillCoolTime -= Time.deltaTime;
        durationTime -= Time.deltaTime;
        if (Input.GetKeyUp(KeyCode.E) && charSkillCoolTime < 0.0f)
        {
            effect = PhotonNetwork.Instantiate(effectDir + "WarriorSkillEffect", new Vector2(this.transform.position.x, this.transform.position.y + 1.0f), Quaternion.identity);
            colliders = Physics2D.OverlapCircleAll(transform.position, warriorActiveSkillRange);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.CompareTag("Enemy"))
                {
                    if (playerPV.ViewID == colliders[i].gameObject.GetComponent<EnemyCtrl>().GetComponent<EnemyAI>().GetFirstTarget().GetComponent<PhotonView>().ViewID)
                    {
                        colliders[i].gameObject.GetComponent<EnemyCtrl>().GetComponent<EnemyAI>().aggroMeter1 += 100;
                    }
                    else
                    {
                        colliders[i].gameObject.GetComponent<EnemyCtrl>().GetComponent<EnemyAI>().aggroMeter2 += 100;
                    }
                    durationTime = warriorActiveSkillDurationTime;
                }
            }
            charSkillCoolTime = warriorActiveSkillCoolTime;
        }

        // if (durationTime < 0.0f && Enemys != null)
        // {
        //     for (int i = 0; i < Enemys.Count; i++)
        //     {
        //         if (playerPV.ViewID == colliders[i].gameObject.GetComponent<EnemyCtrl>().GetComponent<EnemyAI>().GetFirstTarget().GetComponent<PhotonView>().ViewID)
        //         {
        //             Enemys[0].GetComponent<EnemyCtrl>().GetComponent<EnemyAI>().aggroMeter1 -= 100;
        //             Enemys.Remove(Enemys[0]);
        //         }
        //         else
        //         {
        //             colliders[i].gameObject.GetComponent<EnemyCtrl>().GetComponent<EnemyAI>().aggroMeter2 -= 100;
        //             Enemys.Remove(Enemys[0]);
        //         }
        //     }
        // }
    }
}
