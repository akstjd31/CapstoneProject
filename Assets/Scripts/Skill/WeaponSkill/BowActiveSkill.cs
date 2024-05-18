using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowActiveSkill : WeaponActiveSkill
{
    // Start is called before the first frame update
    void Start()
    {
        Hud = GameObject.Find("LocalHUD").GetComponent<HUD>();
        if (Hud != null)
        {
            isInDungeon = true;
        }
        if (isInDungeon)
        {
            Hud.weaponSkillImage.transform.parent.gameObject.SetActive(true);
            Hud.weaponSkillImage.sprite = weaponSkillSprite;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
