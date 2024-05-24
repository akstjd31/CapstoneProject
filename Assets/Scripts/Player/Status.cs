using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
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
    private Dictionary<int, int> expByLevel = new Dictionary<int, int>  // <현재 레벨, 다음 레벨에 필요한 경험치량>
    {
        {0, 10},
        {1, 25},
        {2, 40},
        {3, 60},
        {4, 80},
        {5, 110},
        {6, 150},
        {7, 180},
        {8, 240},
        {9, 350}
    };
    public Transform levelUpEffect;

    public int curExp = 0;  // 현재 경험치량

    public float attackDamage; // 공격력
    [SerializeField] private float defaultAttackDamage; // 디폴트 공격력 (무기 포함 X)

    public int attackSpeed; // 공격 속도

    public float moveSpeed = 5; // 이동 속도
    [SerializeField] private float defaultMoveSpeed; // 디폴트 이동 속도

    public float evasionRate = 5f; // 회피율
    public int goldEarnRate = 0; // 골드 획득량
    private int goldEarnMinRate, goldEarnMaxRate;
    public int money = 0; // 현재 잔고
    public float superArmorDuration = 1.0f; // 슈퍼아머 지속시간 초
    public float damageTakenRate = 1.0f; // 받는 데미지 퍼센트
    public float coolTimeRate = 1.0f; // 쿨타임 감소
    public bool isEnvy = false; // 질투 활성화 bool
    [SerializeField] private float defaultEvasionRate; // 디폴트 회피율

    public string charType; // 직업

    private GameObject canvas;
    [SerializeField] private Transform statInfo; // 플레이어의 스탯 정보
    public Text[] stats; // 스탯 정보가 담긴 텍스트
    private UIManager uiManager;

    PlayerCtrl playerCtrl; // 플레이어 스크립트
    PhotonView pv; // 플레이어 pv
    PassiveSkill passiveSkill;
    string nickName; // 플레이어 닉네임

    private float randNum;

    private PlayerSound playerSound;

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
        nickName = this.GetComponent<PhotonView>().Owner.NickName;
        pv = this.GetComponent<PhotonView>();
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        uiManager = canvas.GetComponent<UIManager>();
        passiveSkill = playerCtrl.GetComponent<PassiveSkill>();
        playerSound = this.GetComponent<PlayerSound>();

        if (SceneManager.GetActiveScene().name == "DungeonScene")
        {
            HP = MAXHP;

            //uiManager.localPlayerStatus = this;
        }

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
            //Vector3 playerPos = Camera.main.WorldToScreenPoint(playerCtrl.transform.position);

            //UpdateText(playerPos);
        }
    }

    public void GoldPerLevel()
    {
        switch (level)
        {
            case 0:
                goldEarnMinRate = 1; goldEarnMaxRate = 2;
                break;
            case 1:
                goldEarnMaxRate = 3;
                break;
            case 2:
                goldEarnMinRate = 2; goldEarnMaxRate = 5;
                break;
            case 3:
                goldEarnMinRate = 3;
                break;
            case 4:
                goldEarnMaxRate = 7;
                break;
            case 5:
                goldEarnMinRate = 4;
                break;
            case 6:
                goldEarnMinRate = 5;
                break;
            case 7:
                goldEarnMaxRate = 9;
                break;
            case 8:
                goldEarnMaxRate = 10;
                break;
            case 9:
                goldEarnMinRate = 6;
                break;
            case 10:
                goldEarnMinRate = 7;
                break;
        }
    }

    [PunRPC]
    private void RandomGoldAmount()
    {
        goldEarnRate = Random.Range(goldEarnMinRate, goldEarnMaxRate+1);
    }

    public void CheckLevelUp()
    {
        if (level != maxLevel)
        {
            if (curExp >= expByLevel[level])
            {
                playerSound.PlayLevelUpSound();
                Instantiate(levelUpEffect.gameObject, new Vector2(this.transform.position.x, this.transform.position.y + 1.5f), Quaternion.identity, this.transform);
                level += 1;
                curExp = 0;
            }
        }
    }

    // 피격 RPC
    [PunRPC]
    public void DamageEnemyOnHitRPC(float damage)
    {
        randNum = Random.Range(0, 100f);

        if (randNum > evasionRate)
        {
            if(isEnvy)
            {
                playerCtrl.GetPartyMember(playerCtrl).GetComponent<Status>().HP -= damage * playerCtrl.GetPartyMember(playerCtrl).GetComponent<Status>().damageTakenRate;
            }
            //passiveSkill.attackCount = 0;
            HP -= damage * damageTakenRate;
        }
        else
        {
            Debug.Log("회피!");
        }
    }

    [PunRPC]
    public void PlayerKnockbackRPC(int enemyViewID, Vector3 attackDirection)
    {
        if (randNum > evasionRate)
        {
            PhotonView targetPV = PhotonView.Find(enemyViewID);

            if (targetPV.GetComponent<Enemy>().enemyData.enemyType == EnemyType.BOSS)
            {
                if (targetPV.GetComponent<BossCtrl>().GetState() == BossCtrl.State.LAZERCAST)
                {
                    playerCtrl.knockBackDistanceSpeed = 12f;
                    return;
                }

                playerCtrl.knockBackDistanceSpeed = 9f;
            }
            else
            {
                playerCtrl.knockBackDistanceSpeed = 5f;
            }

            playerCtrl.originalKnockBackDistanceSpeed = playerCtrl.knockBackDistanceSpeed;
            playerCtrl.onHit = true;
            pv.RPC("ChangeStateRPC", RpcTarget.All, (int)PlayerCtrl.State.ATTACKED);
            playerCtrl.enemyAttackDirection = attackDirection;
        }
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
