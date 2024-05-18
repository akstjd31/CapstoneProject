using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ArcherActiveSkill : ActiveSkill
{
    float archerActiveSkillCoolTime = 60.0f;
    float archerActiveSkillDrationTime = 10.0f;
    public GameObject effect;
    public Sprite archerSkillSprite;
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
            Hud.charSkillImage.sprite = archerSkillSprite;
        }
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
            Hud.charSkillImage.fillAmount = (archerActiveSkillCoolTime - charSkillCoolTime) / archerActiveSkillCoolTime;
        }
        charSkillCoolTime -= Time.deltaTime;
        durationTime -= Time.deltaTime;
        if (Input.GetKeyUp(KeyCode.E) && charSkillCoolTime < 0.0f)
        {
            effect = PhotonNetwork.Instantiate(effectDir + "ArcherSkillEffect", this.transform.position, Quaternion.identity);
            status.moveSpeed += status.GetDefaultMoveSpeed() * 0.3f;
            durationTime = archerActiveSkillDrationTime;
            charSkillCoolTime = archerActiveSkillCoolTime;
        }
        if(durationTime > 0.0f) 
        {
            effect.transform.position = new Vector2(this.transform.position.x + (0.5f * this.transform.localScale.x), this.transform.position.y + 0.3f);
            effect.transform.localScale = new Vector2(-this.transform.localScale.x, this.transform.localScale.y);
        }
        
        if(durationTime < 0.0f && effect != null)
        {
            PhotonNetwork.Destroy(effect);
            status.moveSpeed = status.GetDefaultMoveSpeed();
        }
    }
}
