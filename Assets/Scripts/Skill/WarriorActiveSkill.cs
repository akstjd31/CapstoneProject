using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WarriorActiveSkill : ActiveSkill
{
    Collider2D[] colliders;
    float warriorActiveSkillCoolTime = 60.0f;
    float warriorActiveSkillDrationTime = 10.0f;
    float warriorActiveSkillRange = 8.0f;
    // Start is called before the first frame update
    void Start()
    {
        colliders = null;
    }

    // Update is called once per frame
    void Update()
    {
        coolTime -= Time.deltaTime;
        durationTime -= Time.deltaTime;
        if (Input.GetKeyUp(KeyCode.E) && coolTime < 0.0f)
        {
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
                    durationTime = warriorActiveSkillDrationTime;
                }
            }
            coolTime = warriorActiveSkillCoolTime;
        }

        if (durationTime < 0.0f && colliders != null)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.CompareTag("Enemy"))
                {
                    if (playerPV.ViewID == colliders[i].gameObject.GetComponent<EnemyCtrl>().GetComponent<EnemyAI>().GetFirstTarget().GetComponent<PhotonView>().ViewID)
                    {
                        colliders[i].gameObject.GetComponent<EnemyCtrl>().GetComponent<EnemyAI>().aggroMeter1 -= 100;
                    }
                    else
                    {
                        colliders[i].gameObject.GetComponent<EnemyCtrl>().GetComponent<EnemyAI>().aggroMeter2 -= 100;
                    }
                }
            }
        }

    }
}
