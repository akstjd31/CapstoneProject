using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;

public class SwordActiveSkill : WeaponActiveSkill
{
    public float thisWeaponSkillCoolTime = 0.0f;
    string swordEffectDir = "Sword/";
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
        thisWeaponSkillCoolTime -= Time.deltaTime;
        if (playerCtrl.GetEquipItem().itemType == ItemType.LEGENDARY)
        {
            if (isInDungeon) // 스킬 쿨타임 UI
            {
                if (thisWeaponSkillCoolTime > 0.0f)
                {
                    Hud.weaponSkillCoolTime.canvasRenderer.SetAlpha(1f);
                    Hud.weaponSkillCoolTime.raycastTarget = true;
                    Hud.weaponSkillCoolTime.text = thisWeaponSkillCoolTime.ToString("F1");
                }
                else
                {
                    Hud.weaponSkillCoolTime.canvasRenderer.SetAlpha(0f);
                    Hud.weaponSkillCoolTime.raycastTarget = false;
                }
                Hud.weaponSkillImage.fillAmount = (thisWeaponSkillCoolTime - weaponSkillCoolTime) / thisWeaponSkillCoolTime;
            }

            if (playerCtrl.GetEquipItem().itemID == 160) // Legendary_Demon_Sword
            {
                weaponSkillCoolTime = 50.0f;
                if (Input.GetKeyUp(KeyCode.E) && thisWeaponSkillCoolTime < 0.0f)
                {
                    DemonSowrdSkill();
                    thisWeaponSkillCoolTime = weaponSkillCoolTime;
                }
            }
            else if (playerCtrl.GetEquipItem().itemID == 161) // Great_Sword
            {
                weaponSkillCoolTime = 60.0f;

                if (Input.GetKeyUp(KeyCode.E) && thisWeaponSkillCoolTime < 0.0f)
                {
                    GreatSwordSkill();
                    thisWeaponSkillCoolTime = weaponSkillCoolTime;
                }
            }
            else if (playerCtrl.GetEquipItem().itemID == 162) //DarkGalaxy_Dagger
            {
                weaponSkillCoolTime = 30.0f;
                if (Input.GetKeyUp(KeyCode.E) && thisWeaponSkillCoolTime < 0.0f)
                {
                    DarkGalaxyDaggerSkill();
                    thisWeaponSkillCoolTime = weaponSkillCoolTime;
                }
            }
            else if (playerCtrl.GetEquipItem().itemID == 163) // Icycle_Sword
            {
                weaponSkillCoolTime = 40.0f;
                if (Input.GetKeyUp(KeyCode.E) && thisWeaponSkillCoolTime < 0.0f)
                {
                    IcycleSwordSkill();
                    thisWeaponSkillCoolTime = weaponSkillCoolTime;
                }
            }
            else if (playerCtrl.GetEquipItem().itemID == 164) // King_Maker
            {
                weaponSkillCoolTime = 60.0f;
                if (Input.GetKeyUp(KeyCode.E) && thisWeaponSkillCoolTime < 0.0f)
                {
                    KingMakerSkill();
                    thisWeaponSkillCoolTime = weaponSkillCoolTime;
                }
            }
        }
        else
        {
            Hud.weaponSkillImage.transform.parent.gameObject.SetActive(true);
        }
    }

    void DemonSowrdSkill()
    {
        PhotonNetwork.Instantiate(effectDir + swordEffectDir + "DemonSwordSkillEffect", this.transform.position, Quaternion.identity);
    }
    void GreatSwordSkill()
    {
        PhotonNetwork.Instantiate(effectDir + swordEffectDir + "GreatSwordSkillEffect", this.transform.position, Quaternion.identity);
    }
    void DarkGalaxyDaggerSkill()
    {
        PhotonNetwork.Instantiate(effectDir + swordEffectDir + "DarkGalaxySwordSkillEffect", this.transform.position, Quaternion.identity);
    }
    void IcycleSwordSkill()
    {
        PhotonNetwork.Instantiate(effectDir + swordEffectDir + "IcycleSwordSkillEffect", this.transform.position, Quaternion.identity);
    }
    void KingMakerSkill()
    {
        PhotonNetwork.Instantiate(effectDir + swordEffectDir + "KingMakerSwordBuffEffect", this.transform.position, Quaternion.identity);
        PhotonNetwork.Instantiate(effectDir + swordEffectDir + "KingMakerSwordSkillEffect", this.transform.position, Quaternion.identity);
    }
}
