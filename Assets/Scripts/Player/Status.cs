using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Random = UnityEngine.Random;

public class Status : MonoBehaviourPunCallbacks
{
    // 텍스트 이름 == 인덱스
    private enum TextList
    {
        NickName = 0, HP, Weapon, RollCoolTime
    }

    public float HP; // 체력
    public float MAXHP; // 최대 체력
    [SerializeField] private float defaultHP; // 디폴트 체력

    public int level = 0; // 플레이어 레벨
    public int maxLevel = 10; // 플레이어의 최대 레벨

    public float attackDamage; // 공격력
    [SerializeField] private float defaultAttackDamage; // 디폴트 공격력 (무기 포함 X)

    public int attackSpeed; // 공격 속도

    public float moveSpeed = 5; // 이동 속도
    [SerializeField] private float defaultMoveSpeed; // 디폴트 이동 속도

    public float evasionRate = 5f; // 회피율
    [SerializeField] private float defaultEvasionRate; // 디폴트 회피율

    public string charType; // 직업

    [SerializeField] private Transform statInfo; // 플레이어의 스탯 정보
    public Text[] stats; // 스탯 정보가 담긴 텍스트

    PlayerCtrl playerCtrl; // 플레이어 스크립트
    PhotonView pv; // 플레이어 pv
    string nickName; // 플레이어 닉네임

    private Inventory inventory;

    private void Awake()
    {
        if (charType.Equals("Warrior"))
        {
            InitSetting(200, 34);
        }
        else if (charType.Equals("Archer"))
        {
            InitSetting(125, 38);
        }
    }

    private void Start()
    {
        playerCtrl = this.GetComponent<PlayerCtrl>();
        nickName = playerCtrl.GetComponent<PhotonView>().Owner.NickName;
        pv = this.GetComponent<PhotonView>();
        inventory = GameObject.FindGameObjectWithTag("Canvas").transform.Find("Inventory").GetComponent<Inventory>();

        // 태그로 찾은 후에 텍스트 집어넣기
        //statInfo = GameObject.FindGameObjectWithTag("StatInfo").transform;
        //stats = statInfo.GetChild(0).GetComponentsInChildren<Text>();
    }

    public float GetDefaultHP()
    {
        return defaultHP;
    }

    public float GetDefaultAttackDamage()
    {
        return defaultAttackDamage;
    }

    public float GetDefaultEvasionRate()
    {
        return defaultEvasionRate;
    }

    public float GetDefaultMoveSpeed()
    {
        return defaultMoveSpeed;
    }

    private void InitSetting(int hp, int attackDamage)
    {
        this.HP = hp;
        this.attackDamage = attackDamage;

        MAXHP = hp;
        defaultHP = hp;
        defaultAttackDamage = attackDamage;
        defaultMoveSpeed = moveSpeed;
        defaultEvasionRate = evasionRate;
    }

    private void Update()
    {

        // 본인한테만 스탯 정보가 보임.
        if (pv.IsMine)
        {
            Vector3 playerPos = Camera.main.WorldToScreenPoint(playerCtrl.transform.position);

            //UpdateText(playerPos);
        }
    }

    // 피격 RPC
    [PunRPC]
    public void DamageEnemyOnHitRPC(float damage)
    {
        float rand = Random.Range(0f, 100f);

        // 회피
        if (rand > evasionRate)
        {
            HP -= damage;
        }
    }

    [PunRPC]
    public void PlayerKnockbackRPC(Vector3 attackDirection)
    {
        playerCtrl.onHit = true;
        playerCtrl.SetState(PlayerCtrl.State.ATTACKED);
        playerCtrl.enemyAttackDirection = attackDirection;
    }


    // 플레이어 위치에 따른 텍스트 위치 조절
    void UpdateText(Vector3 playerPos)
    {
        stats[(int)TextList.NickName].transform.position = new Vector2(playerPos.x, playerPos.y + 60);
        stats[(int)TextList.HP].transform.position = new Vector2(playerPos.x + 80, playerPos.y + 40);
        stats[(int)TextList.Weapon].transform.position = new Vector2(playerPos.x + 80, playerPos.y + 20);

        stats[(int)TextList.NickName].text = nickName;
        stats[(int)TextList.HP].text = "HP: " + HP;
        stats[(int)TextList.Weapon].text = "Weapon: " + playerCtrl.weaponName;
        //PrintRollCoolTimeText();
    }
}
