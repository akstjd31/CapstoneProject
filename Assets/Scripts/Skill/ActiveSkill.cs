using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun.Demo.Cockpit;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ActiveSkill : Skill
{
    public float charSkillCoolTime;
    public float durationTime;
    string charType;
    public bool isInDungeon = true;
    public HUD Hud = null;

    // Start is called before the first frame update
    void Start()
    {
        charType = GameObject.Find("PhotonManager").GetComponent<PhotonManager>().GetCharType();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}