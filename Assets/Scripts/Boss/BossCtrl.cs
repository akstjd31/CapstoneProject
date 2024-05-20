using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;

public class BossCtrl : MonoBehaviour
{
    // 애니메이션 종류
    // 근접 펀치, 단단해지기, 레이저 빔, 무적?

    public enum State
    {
        NORMAL, MOVE, ATTACKED, INVINCIBILITY, RANGEATTACK, SPECIAL_LAZER, MELEE, LAZERCAST, ARMORBUFF, DEATH
    }

    public Enemy enemy;

    private Animator anim;
    private Rigidbody2D rigid;
    private EnemyAI enemyAI;
    private NavMeshAgent agent;
    private PhotonView pv;
    private GameObject canvas;
    private DropChanceCalculator dropCalc;
    private DropItem dropItem;

    [SerializeField] private State state;

    // 플레이어 발견 범위
    public Transform detectionPoint;
    public float detectionRange;
    public LayerMask playerLayers;

    // 근접 공격 범위
    public Transform attackPoint;
    public float attackRange = 0.5f;

    // 공격
    private float attackDistanceSpeed = 8f;

    private Vector3 targetPos;
    private bool isMelee = false;

    [SerializeField] private float restTime = 0.0f;

    // 체력 바
    public Slider HPBar;
    [SerializeField] private Slider hpBar;

    // 죽음
    private bool isDeath = false;
    private float deathTime = 2.0f;

    // 피격
    [SerializeField] private string attackerCharType;
    private float attackedDistanceSpeed = 3f;
    public Vector3 playerAttackDirection;
    public bool onHit = false;

    // 로켓 손
    public Transform rocketArmPrefab;
    public Transform rocketFirePoint;
    public Transform rocketEndPoint;
    public Transform rocketAndLaserPoint;
    public Vector2 rectangleSize = new Vector2(20.0f, 2.0f); // 직사각형의 가로와 세로 길이
    [SerializeField] private float rocketCoolTime = 0.0f;
    private float rocketDefaultCoolTime = 30.0f;
    [SerializeField] private bool rocketFire = false;

    // 레이저
    public Transform lazerPrefab;
    public Transform lazerCreatePoint;
    [SerializeField] private float lazerCoolTime = 0.0f;
    private float lazerDefaultCoolTime = 30.0f;
    [SerializeField] private bool lazerFire = false;

    // 중간 패턴 1
    public Transform warningLazerPrefab;
    [SerializeField] private bool specialLazerFire = false;
    [SerializeField] private float spcialLazerCoolTime = 0.0f;
    private float spcialLazerDefaultCoolTime = 60.0f;

    // 중간 패턴 2
    public Transform armorBuffPrefab;
    public Transform armorBuffPoint;
    [SerializeField] private GameObject armorBuffObj = null;
    [SerializeField] private string debuffPlayerCharType;
    [SerializeField] private float armorBuffCoolTime = 0.0f;
    private float armorBuffDefaultCoolTime = 60.0f;
    private float armorBuffElapsedTime;
    private float armorBuffDurationTime = 20.0f;
    int rand;

    // 히든 패턴
    public GameObject jewelColorObj;
    public Transform hiddenPatternTimer;
    private Text timerText;
    [SerializeField] private List<BejeweledPillar> bejeweledPillars;
    private bool HiddenPatternStart = false;
    private float remainingTime = 120f;
    [SerializeField] private bool[] isComplete;
    private Color[] colors = { Color.red, Color.blue, Color.green, Color.yellow };
    private int randColorIdx;

    [SerializeField] private GameObject onTriggerCheckObj;
    [SerializeField] private TriggerCheck triggerCheck;

    public Transform goldPrefab;

    private BossSound bossSound;

    private bool hpSetting = false;
    private UIManager uiManager;

    public State GetState()
    {
        return state;
    }

    [PunRPC]
    void ChangeStateRPC(int state)
    {
        this.state = (State)state;
    }

    private void Awake()
    {
        enemy.InitSetting();
    }

    private void Start()
    {
        enemyAI = this.GetComponent<EnemyAI>();
        agent = this.GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();
        rigid = this.GetComponent<Rigidbody2D>();
        pv = this.GetComponent<PhotonView>();
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        dropItem = this.GetComponent<DropItem>();
        dropCalc = this.GetComponent<DropChanceCalculator>();
        bossSound = this.GetComponent<BossSound>();
        uiManager = canvas.GetComponent<UIManager>();

        bejeweledPillars = new List<BejeweledPillar>();

        pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.NORMAL);
    }

    private void FixedUpdate()
    {
        if (!isDeath)
        {

            if (agent.velocity != Vector3.zero &&
           restTime >= enemy.enemyData.attackDelayTime &&
           state == State.NORMAL)
            {
                pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.MOVE);
            }

            // 시간안에 히든패턴 파훼를 못할 시 플레이어 즉사
            if (HiddenPatternStart && remainingTime <= 0.0f)
            {
                if (enemyAI.GetFirstTarget() != null && enemyAI.GetSecondTarget() != null)
                {
                    enemyAI.GetFirstTarget().GetComponent<Status>().HP = 0f;
                    enemyAI.GetSecondTarget().GetComponent<Status>().HP = 0f;
                }
                else
                {
                    if (enemyAI.GetFirstTarget() != null)
                    {
                        enemyAI.GetFirstTarget().GetComponent<Status>().HP = 0f;
                    }

                    if (enemyAI.GetSecondTarget() != null)
                    {
                        enemyAI.GetSecondTarget().GetComponent<Status>().HP = 0f;
                    }
                }

                enemyAI.isLookingAtPlayer = false;
                agent.isStopped = true;
            }

            // 체력이 30퍼 이하가 되면 히든 패턴 시작
            if (enemy.enemyData.hp <= enemy.enemyData.maxHp * 0.3f && !HiddenPatternStart)
            {
                anim.Play("Idle");
                rigid.velocity = Vector2.zero;
                HiddenPatternStart = true;
                agent.isStopped = true;
                enemyAI.isLookingAtPlayer = false;
                pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.INVINCIBILITY);
                GameObject timer = Instantiate(hiddenPatternTimer.gameObject, Vector2.zero, Quaternion.identity);
                timer.transform.parent = canvas.transform;
                timer.GetComponent<RectTransform>().anchoredPosition = new Vector2(160, -60);
                timerText = timer.GetComponentInChildren<Text>();
            }

            // 로켓 손 or 레이저 발사
            if (state == State.MOVE)
            {
                if (IsPlayerInRectangleRange())
                {
                    if (lazerCoolTime <= 0.0f)
                    {
                        lazerCoolTime = lazerDefaultCoolTime;
                        FlipHorizontalRelativeToTarget(enemyAI.GetFocusTarget().position);
                        pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.LAZERCAST);
                        agent.isStopped = true;
                    }
                    if (rocketCoolTime <= 0.0f)
                    {
                        rocketCoolTime = rocketDefaultCoolTime;
                        agent.isStopped = true;
                        FlipHorizontalRelativeToTarget(enemyAI.GetFocusTarget().position);
                        pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.RANGEATTACK);
                    }
                }

                if (IsEnemyClosetPlayer())
                {
                    if (armorBuffCoolTime <= 0.0f || spcialLazerCoolTime <= 0.0f)
                    {
                        uiManager.RandomFloat();


                        if (uiManager.rand <= 10f)
                        {
                            if (armorBuffCoolTime <= 0.0f)
                            {
                                armorBuffCoolTime = armorBuffDefaultCoolTime;
                                agent.isStopped = true;
                                pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.ARMORBUFF);
                                armorBuffElapsedTime = armorBuffDurationTime;
                            }
                        }
                        else if (uiManager.rand <= 20f)
                        {
                            if (spcialLazerCoolTime <= 0.0f)
                            {
                                spcialLazerCoolTime = spcialLazerDefaultCoolTime;
                                agent.isStopped = true;
                                pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.SPECIAL_LAZER);
                            }
                        }
                        else
                        {
                            pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.MELEE);
                        }

                    }
                    else
                    {
                        pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.MELEE);
                    }
                }
            }

            CoolTimeCalculator();
        }
        else
        {
            rigid.velocity = Vector2.zero;
        }
    }

    private void Update()
    {

        switch (state)
        {
            case State.NORMAL:
                attackAndRest();
                IdleAndMoveAnimation();
                break;
            case State.MOVE:
                IdleAndMoveAnimation();
                break;
            case State.MELEE:
                MeleeAnimation();
                MeleeMove();
                break;
            case State.ATTACKED:
                KnockBack();
                break;
            case State.RANGEATTACK:
                RangeAttackAnimation();
                break;
            case State.LAZERCAST:
                LazerAnimation();
                CheckLazerEnd();
                break;
            case State.SPECIAL_LAZER:
                //SecialLazerEndCheck();
                SpecialLazerAnimation();
                break;
            case State.ARMORBUFF:
                ArmorBuffAnimation();
                break;
            case State.INVINCIBILITY:
                InvincibilityAnimation();
                JewelColorCheck();
                HiddenPatternRemainingTime();
                break;
            case State.DEATH:
                Death();
                break;
        }

        if (enemyAI.GetFocusTarget() != null && enemyAI.isLookingAtPlayer &&
            state != State.LAZERCAST)
        {
            FlipHorizontalRelativeToTarget(enemyAI.GetFocusTarget().position);
        }

        if (hpBar != null)
        {
            hpBar.value = enemy.enemyData.hp;
            //enemy.enemyData.hp = enemy.enemyData.maxHp * 0.31f;
        }

        if (armorBuffElapsedTime <= 0.0f && armorBuffObj != null)
        {
            armorBuffObj = null;
            debuffPlayerCharType = "";
        }

        HPInitSetting();
    }

    private void IdleAndMoveAnimation()
    {
        anim.SetBool("isMelee", false);
        anim.SetBool("isRangeAttack", false);
        anim.SetBool("isLazerFire", false);
        anim.SetBool("isSpecialLazerFire", false);
        anim.SetBool("isArmorBuff", false);
        anim.SetBool("isInvincibility", false);
    }

    private void MeleeAnimation()
    {
        anim.SetBool("isMelee", true);
        anim.SetBool("isRangeAttack", false);
        anim.SetBool("isLazerFire", false);
        anim.SetBool("isSpecialLazerFire", false);
        anim.SetBool("isArmorBuff", false);
        anim.SetBool("isInvincibility", false);
    }

    private void RangeAttackAnimation()
    {
        anim.SetBool("isMelee", false);
        anim.SetBool("isRangeAttack", true);
        anim.SetBool("isLazerFire", false);
        anim.SetBool("isSpecialLazerFire", false);
        anim.SetBool("isArmorBuff", false);
        anim.SetBool("isInvincibility", false);
    }

    private void LazerAnimation()
    {
        anim.SetBool("isMelee", false);
        anim.SetBool("isRangeAttack", false);
        anim.SetBool("isLazerFire", true);
        anim.SetBool("isSpecialLazerFire", false);
        anim.SetBool("isArmorBuff", false);
        anim.SetBool("isInvincibility", false);
    }

    private void SpecialLazerAnimation()
    {
        anim.SetBool("isMelee", false);
        anim.SetBool("isRangeAttack", false);
        anim.SetBool("isLazerFire", false);
        anim.SetBool("isSpecialLazerFire", true);
        anim.SetBool("isArmorBuff", false);
        anim.SetBool("isInvincibility", false);
    }

    private void ArmorBuffAnimation()
    {
        anim.SetBool("isMelee", false);
        anim.SetBool("isRangeAttack", false);
        anim.SetBool("isLazerFire", false);
        anim.SetBool("isSpecialLazerFire", false);
        anim.SetBool("isArmorBuff", true);
        anim.SetBool("isInvincibility", false);
    }

    private void InvincibilityAnimation()
    {
        anim.SetBool("isMelee", false);
        anim.SetBool("isRangeAttack", false);
        anim.SetBool("isLazerFire", false);
        anim.SetBool("isSpecialLazerFire", false);
        anim.SetBool("isArmorBuff", false);
        anim.SetBool("isInvincibility", true);
    }

    // 중간 패턴 1 : 즉사 레이저 애니메이션 이벤트 함수
    private void SpecialLazer()
    {
        GameObject lazerPrefab = Instantiate(warningLazerPrefab.gameObject, this.transform.position, Quaternion.identity);
        WarningLazer warningLazer = lazerPrefab.GetComponent<WarningLazer>();

        warningLazer.SetTarget(enemyAI.GetFocusTarget());

        agent.isStopped = false;
        pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.NORMAL);
        restTime = 0.0f;
        onHit = false;
    }

    // 중간 패턴 2 : 아머 강화 애니메이션 이벤트 함수
    private void ArmorBuff()
    {
        armorBuffObj = Instantiate(armorBuffPrefab.gameObject, armorBuffPoint.position, Quaternion.identity);

        ArmorBuff armorBuff = armorBuffObj.GetComponent<ArmorBuff>();
        armorBuff.SetTarget(armorBuffPoint);
        armorBuff.SetDurationTime(armorBuffDurationTime);

        pv.RPC("RandomNumber", RpcTarget.All);

        // 전사 디버프
        if (rand == 1)
        {
            armorBuff.warriorDebuff.SetActive(true);
            debuffPlayerCharType = "Warrior";

        }
        else
        {
            armorBuff.archerDebuff.SetActive(true);
            debuffPlayerCharType = "Archer";
        }

        agent.isStopped = false;
        restTime = 1.0f;
        pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.NORMAL);
    }

    // 죽음
    private void Death()
    {
        if (deathTime >= 0.0f)
        {
            deathTime -= Time.deltaTime;
        }
        else
        {
            dropItem.GetComponent<PhotonView>().RPC("SpawnDroppedItem", RpcTarget.All);

            Destroy(hpBar.gameObject);
            Destroy(this.gameObject);
        }
    }

    // 히든 패턴 : 2분 동안 재단 색 보스 머리에 뜨는 색하고 같은걸로 변경하기
    private void Invincibility()
    {
        anim.speed = 0f;

        GameObject[] jewelObjs = GameObject.FindGameObjectsWithTag("Jewel");

        // 보스방에 있는 기둥의 정보 불러오기
        foreach (GameObject jewelObj in jewelObjs)
        {
            bejeweledPillars.Add(jewelObj.GetComponent<BejeweledPillar>());
        }

        // 랜덤으로 선택된 색을 입힘.
        //int rand = Random.Range(0, colors.Length);
        jewelColorObj.SetActive(true);
        jewelColorObj.GetComponent<SpriteRenderer>().color = colors[1];

        foreach (BejeweledPillar jewelPillar in bejeweledPillars)
        {
            jewelPillar.SetColor(colors[rand]);
        }

        for (var i = 0; i < bejeweledPillars.Count; i++)
        {
            isComplete[i] = false;
        }
    }

    private void JewelColorCheck()
    {
        if (bejeweledPillars.Count > 0)
        {
            bool allTrue = true;
            foreach (bool value in isComplete)
            {
                if (!value)
                {
                    allTrue = false;
                    break;
                }
            }

            if (allTrue)
            {
                anim.speed = 1f;
                restTime = 0.0f;
                agent.isStopped = false;
                enemyAI.isLookingAtPlayer = true;
                onHit = false;
                pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.NORMAL);
                jewelColorObj.SetActive(false);
                return;
            }

            for (var i = 0; i < bejeweledPillars.Count; i++)
            {
                if (!isComplete[i] && bejeweledPillars[i].flag)
                {
                    isComplete[i] = true;
                }
            }
        }
    }

    private void HiddenPatternRemainingTime()
    {
        remainingTime -= Time.deltaTime;

        int min, sec;
        min = (int)remainingTime / 60;
        sec = (int)remainingTime - min * 60;

        if (timerText != null)
        {
            timerText.text = min + " : " + sec;
        }
    }

    // 쿨타임 계산
    private void CoolTimeCalculator()
    {
        if (rocketCoolTime > 0.0f)
        {
            rocketCoolTime -= Time.deltaTime;
        }

        if (lazerCoolTime > 0.0f)
        {
            lazerCoolTime -= Time.deltaTime;
        }

        if (spcialLazerCoolTime > 0.0f)
        {
            spcialLazerCoolTime -= Time.deltaTime;
        }

        if (armorBuffCoolTime > 0.0f)
        {
            armorBuffCoolTime -= Time.deltaTime;
        }

        if (armorBuffElapsedTime > 0.0f)
        {
            armorBuffElapsedTime -= Time.deltaTime;
        }
    }

    private void CheckLazerEnd()
    {
        bool flag = GameObject.Find("Lazer(Clone)");

        if (lazerFire && !flag)
        {
            anim.speed = 1f;
            restTime = 0.0f;
            pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.NORMAL);
            lazerFire = false;
            onHit = false;
        }
    }

    // 레이저 발사 애니메이션 이벤트 함수
    private void LazerFire()
    {
        GameObject layerObj = Instantiate(lazerPrefab.gameObject, lazerCreatePoint.position, Quaternion.identity);

        Lazer lazer = layerObj.GetComponent<Lazer>();
        lazer.SetDamage(enemy.enemyData.attackDamage * 2.0f);
        lazer.SetViewID(pv.ViewID);
        lazer.SetStartPos(this.transform);

        if (this.transform.localScale.x < 0)
        {
            lazer.transform.localScale = new Vector3(-4, 4, 4);
        }

        lazerFire = true;
    }

    // 플레이어가 직사각형 범위에 포함되었는지 확인하여 반환
    private bool IsPlayerInRectangleRange()
    {
        Collider2D hitPlayers = Physics2D.OverlapBox(rocketAndLaserPoint.position, rectangleSize, 0f, playerLayers);

        return hitPlayers != null;
    }

    // 로켓 발사 애니메이션 이벤트 함수
    private void RocketFire()
    {
        anim.speed = 0f;
        GameObject golemArm = Instantiate(rocketArmPrefab.gameObject, rocketFirePoint.position, Quaternion.identity);

        RocketArm rocketArm = golemArm.GetComponent<RocketArm>();
        rocketArm.SetTargetPos(enemyAI.GetFocusTarget());
        rocketArm.SetViewID(pv.ViewID);
        rocketArm.SetDamage(enemy.enemyData.attackDamage * 1.5f);

        bossSound.PlayRangeAttackSound();

        anim.speed = 1f;
        restTime = 0.0f;
        pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.NORMAL);
        onHit = false;
    }

    // 플레이어한테 피격당했을 떄 RPC
    [PunRPC]
    public void DamagePlayerOnHitRPC(int playerViewID, float percentage)
    {
        PhotonView playerPV = PhotonView.Find(playerViewID);
        Status status = playerPV.GetComponent<Status>();

        attackerCharType = status.charType;

        // 골렘 기준 버프효과가 지속되고 있으며, 플레이어 기준 때린 사람의 직업이 디버프 효과를 받고 있는 직업과 동일하다면
        if (armorBuffObj != null && attackerCharType.Equals(debuffPlayerCharType) || state == State.INVINCIBILITY)
        {
            Debug.Log("무적!");
            return;
        }

        // 플레이어의 공격력만큼 체력에서 깎음
        if (hpBar != null)
        {
            enemy.enemyData.hp -= status.attackDamage * percentage;

            // 죽음
            if (enemy.enemyData.hp <= 0)
            {
                pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.DEATH);
                anim.SetTrigger("Death");
                enemyAI.enabled = false;
                isDeath = true;
                agent.isStopped = true;
                rigid.velocity = Vector2.zero;
                dropCalc.SetLevel(status.level);    // 죽기 전에 본인을 죽인 플레이어의 레벨정보를 넘겨준다.
                dropItem.SetCharType(status.charType);

                status.curExp += 1;
                status.CheckLevelUp();

                Status targetStatus = null;
                // 적을 잡으면 모든 플레이어한테 골드, 경험치를 부여
                if (playerPV.transform.Equals(enemyAI.GetFirstTarget()))
                {
                    targetStatus = enemyAI.GetSecondTarget().GetComponent<Status>();

                    if (targetStatus != null)
                    {
                        targetStatus.curExp += 1;
                        targetStatus.CheckLevelUp();
                    }
                }
                else
                {
                    targetStatus = enemyAI.GetFirstTarget().GetComponent<Status>();

                    if (targetStatus != null)
                    {
                        targetStatus.curExp += 1;
                        targetStatus.CheckLevelUp();
                    }
                }

                GameObject gold = PhotonNetwork.Instantiate("Enemy/" + goldPrefab.name, this.transform.position, Quaternion.identity);
                gold.GetComponent<GoldItem>().SetStatus(status, targetStatus);

                return;
            }
        }

        if (enemyAI.GetSecondTarget() == null)
        {
            enemyAI.aggroMeter1 += status.attackDamage * percentage;
        }
        else
        {
            if (playerViewID == enemyAI.GetFirstTarget().GetComponent<PhotonView>().ViewID)
            {
                enemyAI.aggroMeter1 += status.attackDamage * percentage;
            }
            else
            {
                enemyAI.aggroMeter2 += status.attackDamage * percentage;
            }
        }
    }

    [PunRPC]
    public void BossKnockbackRPC(Vector3 attackDirection)
    {
        onHit = true;
        if (state == State.MOVE || state == State.MELEE || state == State.NORMAL)
        {
            playerAttackDirection = attackDirection;
            pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.ATTACKED);
        }
    }


    private void KnockBack()
    {
        if (armorBuffObj != null &&
            attackerCharType.Equals(debuffPlayerCharType))
        {
            attackerCharType = "";
            rigid.velocity = Vector2.zero;
            onHit = false;
            pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.NORMAL);
            restTime = 1.0f;
            return;
        }

        if (onHit && enemy.enemyData.hp > 0)
        {
            rigid.velocity = Vector2.zero;

            if (state == State.MELEE)
            {
                anim.Play("Idle", -1, 0f);
                anim.speed = 0;
            }

            agent.isStopped = true;

            rigid.velocity = playerAttackDirection * attackedDistanceSpeed;

            float attackedSpeedDropMultiplier = 4f;
            attackedDistanceSpeed -= attackedDistanceSpeed * attackedSpeedDropMultiplier * Time.deltaTime;

            if (attackedDistanceSpeed < 0.1f)
            {
                rigid.velocity = Vector2.zero;
                onHit = false;
                attackedDistanceSpeed = 3f;
                anim.speed = 1f;
                pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.NORMAL);
                restTime = 1.5f;
            }
        }
    }

    // Melee 애니메이션 이벤트 함수 (공격포인트에 맞춰서 오버랩 체크)
    private void Melee()
    {
        // 공격 범위에 포함된 플레이어
        Collider2D hitPlayers = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayers);

        if (hitPlayers != null)
        {
            PlayerCtrl player = hitPlayers.GetComponent<PlayerCtrl>();
            if (player != null && !player.onHit)
            {
                PhotonView playerPV = player.GetComponent<PhotonView>();

                // 몹의 데미지만큼 플레이어에게 피해를 입힘.
                //player.GetComponent<PhotonView>().RPC("DamageEnemyOnHitRPC", RpcTarget.All, player.passiveSkill.PrideDamaged(enemy.enemyData.attackDamage));
                playerPV.RPC("DamageEnemyOnHitRPC", RpcTarget.All, enemy.enemyData.attackDamage);
                playerPV.RPC("PlayerKnockbackRPC", RpcTarget.All, pv.ViewID, targetPos - this.transform.position);

                hitPlayers.GetComponent<PlayerSound>().PlayAttackedSound();
            }
        }
    }

    // HP UI 생성 및 세팅
    public void HPInitSetting()
    {
        // 플레이어가 방에 존재할때 HP바 생성
        if (triggerCheck.isPlayerInRoom && !hpSetting)
        {
            hpSetting = true;
            hpBar = Instantiate(HPBar, Vector2.zero, Quaternion.identity, canvas.transform);

            hpBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 400);
            hpBar.maxValue = enemy.enemyData.maxHp;
            hpBar.value = enemy.enemyData.hp;

            hpBar.GetComponentInChildren<Text>().text = enemy.enemyData.name;
        }
    }

    // Melee 애니메이션 이벤트 함수 (공격 애니메이션에 맞춰서 이동)
    private void MeleeAnimEventFunc()
    {
        isMelee = true;
        agent.isStopped = true;
        targetPos = enemyAI.GetFocusTarget().position;
        enemyAI.isLookingAtPlayer = false;
    }

    // 이동 공격
    private void MeleeMove()
    {
        if (agent.isStopped && !enemyAI.isLookingAtPlayer)
        {
            rigid.velocity = (targetPos - this.transform.position) * attackDistanceSpeed;

            float attackSpeedDropMultiplier = 6.5f;

            attackDistanceSpeed -= attackDistanceSpeed * attackSpeedDropMultiplier * Time.deltaTime;

            if (attackDistanceSpeed < 1f)
            {
                isMelee = false;
                rigid.velocity = Vector2.zero;
                attackDistanceSpeed = 8f;
                pv.RPC("ChangeStateRPC", RpcTarget.All, (int)State.NORMAL);
                restTime = 0.0f;
            }
        }
    }

    // 적의 위치에 따른 스케일 뒤집기
    private void FlipHorizontalRelativeToTarget(Vector2 target)
    {
        if (enemy.enemyData.name.Equals("Golem"))
        {
            if (target.x - this.transform.position.x > 0)
            {
                this.transform.localScale = new Vector3(7, 7, 7);
            }
            else
            {
                this.transform.localScale = new Vector3(-7, 7, 7);
            }
        }
    }

    // 플레이어가 소유한 범위포인트에 따른 반환
    private bool IsEnemyClosetPlayer()
    {
        if (enemyAI.GetFocusTarget() != null)
        {
            // 공격 범위에 들어간 적
            Collider2D players = Physics2D.OverlapCircle(detectionPoint.position, detectionRange, playerLayers);

            if (players != null)
            {
                return true;
            }
        }

        return false;
    }

    // 공격이나 피격당할 때 몹마다 가지고 있는 딜레이타임별로 쉬기
    private void attackAndRest()
    {
        if (restTime <= enemy.enemyData.attackDelayTime)
        {
            restTime += Time.deltaTime;
        }
        else
        {
            agent.isStopped = false;
            enemyAI.isLookingAtPlayer = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TriggerObj"))
        {
            onTriggerCheckObj = other.gameObject;
            triggerCheck = onTriggerCheckObj.GetComponent<TriggerCheck>();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint == null)
            return;

        Gizmos.DrawWireSphere(detectionPoint.position, detectionRange);

        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    //private void OnDrawGizmos()
    //{
    //    if (rocketAndLaserPoint != null)
    //    {
    //        Gizmos.color = Color.red; // 기즈모의 색상 설정
    //        // 직사각형 중심 위치
    //        Vector3 center = rocketAndLaserPoint.position;
    //        // 직사각형의 크기 설정
    //        Vector3 size = new Vector3(rectangleSize.x, rectangleSize.y, 0.0f);
    //        // 직사각형 그리기
    //        Gizmos.DrawWireCube(center, size);
    //    }
    //}
}
