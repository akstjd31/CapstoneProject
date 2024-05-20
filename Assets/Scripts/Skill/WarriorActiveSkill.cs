using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Voice.PUN;

public class WarriorActiveSkill : ActiveSkill
{
    PhotonView pv;
    Vector3 mouseWorldPosition;

    Collider2D[] colliders;
    float setCharSkillCoolTime = 60.0f;
    float setCharSkillDurationTime = 10.0f;
    float setCharSkillRange = 8.0f;
    public Sprite warriorSkillSprite;

    
    float setWeaponSkillCoolTime;
    public Sprite[] weaponSkillSprite = new Sprite[5];
    string swordEffectDir = "Sword/";

    float angle;
    Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        pv = playerCtrl.GetComponent<PhotonView>();
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
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        mouseWorldPosition = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0.0f);

        direction = mouseWorldPosition - this.transform.position;
        direction.z = 0; // 2D 게임의 경우 z 축 방향을 무시

        // 방향 벡터를 기준으로 회전 각도 계산
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

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
            if (Input.GetKeyDown(KeyCode.E) && charSkillCoolTime < 0.0f)
            {
                charEffect = PhotonNetwork.Instantiate(charEffectDir + "WarriorSkillEffect", new Vector2(this.transform.position.x, this.transform.position.y + 1.0f), Quaternion.identity);
                colliders = Physics2D.OverlapCircleAll(transform.position, setCharSkillRange);
                for (int i = 0; i < colliders.Length; i++)
                {
                    if (colliders[i].gameObject.CompareTag("Enemy"))
                    {
                        if (playerCtrl.pv.ViewID == colliders[i].gameObject.GetComponent<EnemyCtrl>().GetComponent<EnemyAI>().GetFirstTarget().GetComponent<PhotonView>().ViewID)
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
                    if (Input.GetKeyDown(KeyCode.R) && weaponSkillCoolTime < 0.0f)
                    {
                        DemonSwordSkill();
                    }
                }
                else if (playerCtrl.GetEquipItem().itemID == 161) // Great_Sword
                {
                    Hud.weaponSkillImage.sprite = weaponSkillSprite[1];
                    setWeaponSkillCoolTime = 60.0f;

                    if (Input.GetKeyDown(KeyCode.R) && weaponSkillCoolTime < 0.0f)
                    {
                        GreatSwordSkill();
                    }
                }
                else if (playerCtrl.GetEquipItem().itemID == 162) //DarkGalaxy_Dagger
                {
                    Hud.weaponSkillImage.sprite = weaponSkillSprite[2];
                    setWeaponSkillCoolTime = 30.0f;
                    if (Input.GetKeyDown(KeyCode.R) && weaponSkillCoolTime < 0.0f)
                    {
                        DarkGalaxyDaggerSkill();
                    }
                }
                else if (playerCtrl.GetEquipItem().itemID == 163) // Icycle_Sword
                {
                    Hud.weaponSkillImage.sprite = weaponSkillSprite[3];
                    setWeaponSkillCoolTime = 40.0f;
                    if (Input.GetKeyDown(KeyCode.R) && weaponSkillCoolTime < 0.0f)
                    {
                        IcycleSwordSkill();
                    }
                }
                else if (playerCtrl.GetEquipItem().itemID == 164) // King_Maker
                {
                    Hud.weaponSkillImage.sprite = weaponSkillSprite[4];
                    setWeaponSkillCoolTime = 60.0f;
                    if (Input.GetKeyDown(KeyCode.R) && weaponSkillCoolTime < 0.0f)
                    {
                        KingMakerSkill();
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

    void DemonSwordSkill()
    {
        GameObject demonSwordSkillPrefab = PhotonNetwork.Instantiate(WeaponEffectDir + swordEffectDir + "DemonSwordSkill_", this.transform.position, Quaternion.identity);
        PhotonView demonSwordSkillPv = demonSwordSkillPrefab.GetComponent<PhotonView>();
        DemonSwordSkill demonSwordSkill = demonSwordSkillPv.GetComponent<DemonSwordSkill>();
        Debug.Log(pv.ViewID);
        demonSwordSkillPv.RPC("InitializeDemonSwordSkill", RpcTarget.AllBuffered, pv.ViewID);
        weaponSkillCoolTime = setWeaponSkillCoolTime;
    }
    void GreatSwordSkill()
    {

        GameObject GreatSwordSkillPrefab = PhotonNetwork.Instantiate(WeaponEffectDir + swordEffectDir + "GreatSwordSkill_", this.transform.position,  Quaternion.Euler(0, 0, angle));
        PhotonView GreatSwordSkillPv = GreatSwordSkillPrefab.GetComponent<PhotonView>();
        GreatSwordSkill GreatSwordSkill = GreatSwordSkillPv.GetComponent<GreatSwordSkill>();
        Debug.Log(pv.ViewID);
        GreatSwordSkillPv.RPC("InitializeGreatSwordSkill", RpcTarget.AllBuffered, pv.ViewID, direction);
        weaponSkillCoolTime = setWeaponSkillCoolTime;
    }
    void DarkGalaxyDaggerSkill()
    {
        Collider2D[] collider = Physics2D.OverlapCircleAll(transform.position, 8.0f);
        for(int i = 0; i < collider.Length; i++)
        {
            if(collider[i].CompareTag("Enemy"))
            {
                playerCtrl.transform.position = collider[i].gameObject.transform.position;
                GameObject DarkGalaxyDaggerSkillPrefab = PhotonNetwork.Instantiate(WeaponEffectDir + swordEffectDir + "DarkGalaxySwordSkill_", this.transform.position, Quaternion.identity);
                PhotonView DarkGalaxyDaggerSkillPv = DarkGalaxyDaggerSkillPrefab.GetComponent<PhotonView>();
                DarkGalaxySwordSkill DarkGalaxyDaggerSkill = DarkGalaxyDaggerSkillPv.GetComponent<DarkGalaxySwordSkill>();
                Debug.Log(pv.ViewID);
                DarkGalaxyDaggerSkillPv.RPC("InitializeDarkGalaxyDaggerSkill", RpcTarget.AllBuffered, pv.ViewID);
                weaponSkillCoolTime = setWeaponSkillCoolTime;
                break;
            }
        }
    }
    void IcycleSwordSkill()
    {
        GameObject IcycleSwordSkillPrefab = PhotonNetwork.Instantiate(WeaponEffectDir + swordEffectDir + "IcycleSwordSkill_", this.transform.position, Quaternion.identity);
        PhotonView IcycleSwordSkillPv = IcycleSwordSkillPrefab.GetComponent<PhotonView>();
        IcycleSwordSkill IcycleSwordSkill = IcycleSwordSkillPv.GetComponent<IcycleSwordSkill>();
        Debug.Log(pv.ViewID);
        IcycleSwordSkillPv.RPC("InitializeIcycleSwordSkill", RpcTarget.AllBuffered, pv.ViewID);
        weaponSkillCoolTime = setWeaponSkillCoolTime;
    }
    void KingMakerSkill()
    {
        GameObject KingMakerSkillPrefab = PhotonNetwork.Instantiate(WeaponEffectDir + swordEffectDir + "KingMakerSwordSkill_", this.transform.position, Quaternion.identity);
        PhotonView KingMakerSkillPv = KingMakerSkillPrefab.GetComponent<PhotonView>();
        KingMakerSkill KingMakerSkill = KingMakerSkillPv.GetComponent<KingMakerSkill>();
        KingMakerSkillPv.RPC("InitializeKingMakerSwordSkill", RpcTarget.AllBuffered, pv.ViewID);
        KingMakerSkill.setPlayerCtrl(playerCtrl);
        KingMakerSkill.setStatus(status);
        weaponSkillCoolTime = setWeaponSkillCoolTime;
    }
}
