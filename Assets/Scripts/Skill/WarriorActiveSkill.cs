using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class WarriorActiveSkill : ActiveSkill
{
    Collider2D[] colliders;
    float setCharSkillCoolTime = 60.0f;
    float setCharSkillDurationTime = 10.0f;
    float setCharSkillRange = 8.0f;
    public Sprite warriorSkillSprite;

    
    float setWeaponSkillCoolTime;
    public Sprite[] weaponSkillSprite = new Sprite[5];
    string swordEffectDir = "Sword/";

    // Start is called before the first frame update
    void Start()
    {
        isInDungeon = false;
        if (GameObject.Find("LocalHUD"))
        {
            Hud = GameObject.Find("LocalHUD").GetComponent<HUD>();
            isInDungeon = true;
        }
        if (isInDungeon)
        {
            charSkillCoolTime = 0.0f;
            weaponSkillCoolTime = 0.0f;
            Hud.charSkillImage.sprite = warriorSkillSprite;
        }
        colliders = null;
    }

    // Update is called once per frame
    void Update()
    {

        if (isInDungeon) // 스킬 쿨타임 UI
        {
            //직업 쿨타임
            if (charSkillCoolTime > 0.0f)
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
            Hud.charSkillImage.fillAmount = (setCharSkillCoolTime - charSkillCoolTime) / setCharSkillCoolTime;

            //직업 스킬
            charSkillCoolTime -= Time.deltaTime;
            durationTime -= Time.deltaTime;
            if (Input.GetKeyUp(KeyCode.E) && charSkillCoolTime < 0.0f)
            {
                charEffect = PhotonNetwork.Instantiate(charEffectDir + "WarriorSkillEffect", new Vector2(this.transform.position.x, this.transform.position.y + 1.0f), Quaternion.identity);
                colliders = Physics2D.OverlapCircleAll(transform.position, setCharSkillRange);
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
                        durationTime = setCharSkillDurationTime;
                    }
                }
                charSkillCoolTime = setCharSkillCoolTime;
            }

            weaponSkillCoolTime -= Time.deltaTime;
            if (playerCtrl.GetEquipItem().itemType == ItemType.LEGENDARY)
            {
                //무기 쿨타임
                Hud.weaponSkillImage.transform.parent.gameObject.SetActive(true);
                if (weaponSkillCoolTime > 0.0f)
                {
                    Hud.weaponSkillCoolTime.canvasRenderer.SetAlpha(1f);
                    Hud.weaponSkillCoolTime.raycastTarget = true;
                    Hud.weaponSkillCoolTime.text = weaponSkillCoolTime.ToString("F1");
                }
                else
                {
                    Hud.weaponSkillCoolTime.canvasRenderer.SetAlpha(0f);
                    Hud.weaponSkillCoolTime.raycastTarget = false;
                }
                Hud.weaponSkillImage.fillAmount = (setWeaponSkillCoolTime - weaponSkillCoolTime) / setWeaponSkillCoolTime;

                //무기 스킬
                if (playerCtrl.GetEquipItem().itemID == 160) // Legendary_Demon_Sword
                {
                    Hud.weaponSkillImage.sprite = weaponSkillSprite[0];
                    setWeaponSkillCoolTime = 50.0f;
                    if (Input.GetKeyUp(KeyCode.R) && weaponSkillCoolTime < 0.0f)
                    {
                        DemonSowrdSkill();
                        weaponSkillCoolTime = setWeaponSkillCoolTime;
                    }
                }
                else if (playerCtrl.GetEquipItem().itemID == 161) // Great_Sword
                {
                    Hud.weaponSkillImage.sprite = weaponSkillSprite[1];
                    setWeaponSkillCoolTime = 60.0f;

                    if (Input.GetKeyUp(KeyCode.R) && weaponSkillCoolTime < 0.0f)
                    {
                        GreatSwordSkill();
                        weaponSkillCoolTime = setWeaponSkillCoolTime;
                    }
                }
                else if (playerCtrl.GetEquipItem().itemID == 162) //DarkGalaxy_Dagger
                {
                    Hud.weaponSkillImage.sprite = weaponSkillSprite[2];
                    setWeaponSkillCoolTime = 30.0f;
                    if (Input.GetKeyUp(KeyCode.R) && weaponSkillCoolTime < 0.0f)
                    {
                        DarkGalaxyDaggerSkill();
                        weaponSkillCoolTime = setWeaponSkillCoolTime;
                    }
                }
                else if (playerCtrl.GetEquipItem().itemID == 163) // Icycle_Sword
                {
                    Hud.weaponSkillImage.sprite = weaponSkillSprite[3];
                    setWeaponSkillCoolTime = 40.0f;
                    if (Input.GetKeyUp(KeyCode.R) && weaponSkillCoolTime < 0.0f)
                    {
                        IcycleSwordSkill();
                        weaponSkillCoolTime = setWeaponSkillCoolTime;
                    }
                }
                else if (playerCtrl.GetEquipItem().itemID == 164) // King_Maker
                {
                    Hud.weaponSkillImage.sprite = weaponSkillSprite[4];
                    setWeaponSkillCoolTime = 60.0f;
                    if (Input.GetKeyUp(KeyCode.R) && weaponSkillCoolTime < 0.0f)
                    {
                        KingMakerSkill();
                        weaponSkillCoolTime = setWeaponSkillCoolTime;
                    }
                }
            }
            else
            {
                Hud.weaponSkillImage.transform.parent.gameObject.SetActive(false);
            }
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

    void DemonSowrdSkill()
    {
        PhotonNetwork.Instantiate(WeaponEffectDir + swordEffectDir + "DemonSwordSkillEffect", this.transform.position, Quaternion.identity);
    }
    void GreatSwordSkill()
    {
        PhotonNetwork.Instantiate(WeaponEffectDir + swordEffectDir + "GreatSwordSkillEffect", this.transform.position, Quaternion.identity);
    }
    void DarkGalaxyDaggerSkill()
    {
        PhotonNetwork.Instantiate(WeaponEffectDir + swordEffectDir + "DarkGalaxySwordSkillEffect", this.transform.position, Quaternion.identity);
    }
    void IcycleSwordSkill()
    {
        PhotonNetwork.Instantiate(WeaponEffectDir + swordEffectDir + "IcycleSwordSkillEffect", this.transform.position, Quaternion.identity);
    }
    void KingMakerSkill()
    {
        PhotonNetwork.Instantiate(WeaponEffectDir + swordEffectDir + "KingMakerSwordBuffEffect", this.transform.position, Quaternion.identity);
        PhotonNetwork.Instantiate(WeaponEffectDir + swordEffectDir + "KingMakerSwordSkillEffect", this.transform.position, Quaternion.identity);
    }
}
