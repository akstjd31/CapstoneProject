using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ArcherActiveSkill : ActiveSkill
{
    float archerActiveSkillCoolTime = 60.0f;
    float archerActiveSkillDrationTime = 10.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        coolTime -= Time.deltaTime;
        durationTime -= Time.deltaTime;
        if (Input.GetKeyUp(KeyCode.E) && coolTime < 0.0f)
        {
            status.moveSpeed += status.GetDefaultMoveSpeed() * 0.3f;
            durationTime = archerActiveSkillDrationTime;
            coolTime = archerActiveSkillCoolTime;
        }
        
        if(durationTime < 0.0f)
        {
            status.moveSpeed = status.GetDefaultMoveSpeed();
        }
    }
}
