using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class ArcherActiveSkill : ActiveSkill
{
    PhotonView pv;
    Vector3 mouseWorldPosition;


    float setCharSkillCoolTime = 60.0f;
    float setCharSkillDrationTime = 10.0f;
    public Sprite archerSkillSprite;

    float setWeaponSkillCoolTime;
    public Sprite[] weaponSkillSprite = new Sprite[5];
    string bowEffectDir = "Bow/";

    float angle;
    Vector3 direction;

    // Start is called before the first frame update
    void Start()
    {
        if (pv.IsMine)
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
                Hud.charSkillImage.sprite = archerSkillSprite;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (pv.IsMine)
        {
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseWorldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            mouseWorldPosition = new Vector3(mouseWorldPosition.x, mouseWorldPosition.y, 0.0f);

            direction = mouseWorldPosition - this.transform.position;
            direction.z = 0; // 2D 게임의 경우 z 축 방향을 무시

            // 방향 벡터를 기준으로 회전 각도 계산
            angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            if (isInDungeon)
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
            }

            //직업 스킬
            charSkillCoolTime -= Time.deltaTime;
            durationTime -= Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.E) && charSkillCoolTime < 0.0f)
            {
                charEffect = PhotonNetwork.Instantiate(charEffectDir + "ArcherSkillEffect", this.transform.position, Quaternion.identity);
                status.moveSpeed += status.GetDefaultMoveSpeed() * 0.3f;
                durationTime = setCharSkillDrationTime;
                charSkillCoolTime = setCharSkillCoolTime;
            }
            if (durationTime > 0.0f)
            {
                charEffect.transform.position = new Vector2(this.transform.position.x + (0.5f * this.transform.localScale.x), this.transform.position.y + 0.3f);
                charEffect.transform.localScale = new Vector2(-this.transform.localScale.x, this.transform.localScale.y);
            }

            if (durationTime < 0.0f && charEffect != null)
            {
                PhotonNetwork.Destroy(charEffect);
                status.moveSpeed = status.GetDefaultMoveSpeed();
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
                if (playerCtrl.GetEquipItem().itemID == 260) // Dark_Long_Bow
                {
                    Hud.weaponSkillImage.sprite = weaponSkillSprite[0];
                    setWeaponSkillCoolTime = 60.0f;
                    if (Input.GetKeyDown(KeyCode.R) && weaponSkillCoolTime < 0.0f)
                    {
                        DarkLongBowSkill();
                        weaponSkillCoolTime = setWeaponSkillCoolTime;
                    }
                }
                else if (playerCtrl.GetEquipItem().itemID == 261) // Fire_Phoenix_Bow
                {
                    Hud.weaponSkillImage.sprite = weaponSkillSprite[1];
                    setWeaponSkillCoolTime = 50.0f;

                    if (Input.GetKeyDown(KeyCode.R) && weaponSkillCoolTime < 0.0f)
                    {
                        FirePhoenixBow();
                        weaponSkillCoolTime = setWeaponSkillCoolTime;
                    }
                }
                else if (playerCtrl.GetEquipItem().itemID == 262) //Icycle_Wind_Bow
                {
                    Hud.weaponSkillImage.sprite = weaponSkillSprite[2];
                    setWeaponSkillCoolTime = 0.1f;
                    if (Input.GetKey(KeyCode.R) && weaponSkillCoolTime <= 0.0f)
                    {
                        IcycleWindBowSkill();
                        weaponSkillCoolTime = setWeaponSkillCoolTime;
                    }
                }
                else if (playerCtrl.GetEquipItem().itemID == 263) // Shining_Compound_Bow
                {
                    Hud.weaponSkillImage.sprite = weaponSkillSprite[3];
                    setWeaponSkillCoolTime = 40.0f;
                    if (Input.GetKeyDown(KeyCode.R) && weaponSkillCoolTime < 0.0f)
                    {
                        ShiningCompoundBowSkill();
                        weaponSkillCoolTime = setWeaponSkillCoolTime;
                    }
                }
                else if (playerCtrl.GetEquipItem().itemID == 264) // Pathfinder
                {
                    Hud.weaponSkillImage.sprite = weaponSkillSprite[4];
                    setWeaponSkillCoolTime = 60.0f;
                    if (Input.GetKeyDown(KeyCode.R) && weaponSkillCoolTime < 0.0f)
                    {
                        PathfinderSkill();
                        weaponSkillCoolTime = setWeaponSkillCoolTime;
                    }
                }
            }
            else
            {
                if (Hud != null)
                    Hud.weaponSkillImage.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    void DarkLongBowSkill()
    {
        GameObject DarkLongBowSkillPrefab = PhotonNetwork.Instantiate(WeaponEffectDir + bowEffectDir + "DarkLongBowSkill_", mouseWorldPosition, Quaternion.identity);
        PhotonView DarkLongBowSkillPv = DarkLongBowSkillPrefab.GetComponent<PhotonView>();
        DarkLongBowSkill DarkLongBowSkill = DarkLongBowSkillPv.GetComponent<DarkLongBowSkill>();
        DarkLongBowSkillPv.RPC("InitializeDarkLongBowSkill", RpcTarget.AllBuffered, pv.ViewID);
        weaponSkillCoolTime = setWeaponSkillCoolTime;
    }
    void FirePhoenixBow()
    {
        PhotonNetwork.Instantiate(WeaponEffectDir + bowEffectDir + "GreatSwordSkillEffect", this.transform.position, Quaternion.identity);
    }
    void IcycleWindBowSkill()
    {
        playerCtrl.Fire();
        if(mouseWorldPosition.x < playerCtrl.transform.position.x)
        {
            PhotonNetwork.Instantiate(WeaponEffectDir + bowEffectDir + "IcycleWindBowSkill_L", new Vector2(this.transform.position.x - 0.7f, this.transform.position.y + 0.5f), Quaternion.identity);
        }
        else if(mouseWorldPosition.x > playerCtrl.transform.position.x)
        {
            PhotonNetwork.Instantiate(WeaponEffectDir + bowEffectDir + "IcycleWindBowSkill_R", new Vector2(this.transform.position.x + 0.7f, this.transform.position.y + 0.5f), Quaternion.identity);
        }
    }
    void ShiningCompoundBowSkill()
    {
        GameObject ShiningCompoundBowSkillPrefab = PhotonNetwork.Instantiate(WeaponEffectDir + bowEffectDir + "ShiningCompoundBowSkill_", new Vector2(mouseWorldPosition.x, mouseWorldPosition.y + 5.0f), Quaternion.identity);
        PhotonView ShiningCompoundBowSkillPv = ShiningCompoundBowSkillPrefab.GetComponent<PhotonView>();
        ShiningCompoundBowSkill DarkLongBowSkill = ShiningCompoundBowSkillPv.GetComponent<ShiningCompoundBowSkill>();
        ShiningCompoundBowSkillPv.RPC("InitializeShiningCompoundBowSkill", RpcTarget.AllBuffered, pv.ViewID, direction);
        weaponSkillCoolTime = setWeaponSkillCoolTime;
    }
    void PathfinderSkill()
    {
        GameObject PathFinderSkillPrefab = PhotonNetwork.Instantiate(WeaponEffectDir + bowEffectDir + "PathFinderSkill_", this.transform.position, Quaternion.identity);
        PhotonView PathFinderSkillPv = PathFinderSkillPrefab.GetComponent<PhotonView>();
        PathFinderSkill PathFinderSkill = PathFinderSkillPv.GetComponent<PathFinderSkill>();
        PathFinderSkillPv.RPC("InitializePathFinderSkill", RpcTarget.AllBuffered, pv.ViewID);
        PathFinderSkill.setPlayerCtrl(playerCtrl);
        PathFinderSkill.setStatus(status);
        weaponSkillCoolTime = setWeaponSkillCoolTime;
    }
}
