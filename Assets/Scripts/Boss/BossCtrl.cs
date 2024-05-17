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
        NORMAL, MOVE, ATTACKED, GLOWING, RANGEATTACK, SPECIAL_LAZER, MELEE, LAZERCAST, ARMORBUFF, DEATH
    }

    public Enemy enemy;

    private Animator anim;
    private Rigidbody2D rigid;
    private EnemyAI enemyAI;
    private NavMeshAgent agent;
    private PhotonView pv;
    private GameObject canvas;

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
    private float deathTime = 1.0f;

    // 피격
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
    [SerializeField] private float armorBuffCoolTime = 0.0f;
    private float armorBuffDefaultCoolTime = 60.0f;
    private float armorBuffDurationTime = 20.0f;

    public State GetState()
    {
        return state;
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

        HPInitSetting();
    }

    private void FixedUpdate()
    {
        if (!isDeath)
        {
            if (agent.velocity != Vector3.zero &&
                restTime >= enemy.enemyData.attackDelayTime &&
                state == State.NORMAL)
            {
                state = State.MOVE;
            }

            // 로켓 손 or 레이저 발사
            if (state == State.MOVE &&
                IsPlayerInRectangleRange())
            {
                int rand = Random.Range(0, 2);

                if (rand == 0)
                {
                    if (lazerCoolTime <= 0.0f)
                    {
                        lazerCoolTime = lazerDefaultCoolTime;
                        FlipHorizontalRelativeToTarget(enemyAI.GetFocusTarget().position);
                        state = State.LAZERCAST;
                        agent.isStopped = true;
                    }
                }
                else
                {
                    if (rocketCoolTime <= 0.0f)
                    {
                        rocketCoolTime = rocketDefaultCoolTime;
                        agent.isStopped = true;
                        FlipHorizontalRelativeToTarget(enemyAI.GetFocusTarget().position);
                        state = State.RANGEATTACK;
                    }
                }

                //int randNum = Random.Range(0, 2);

                //if (randNum == 0)
                //{
                //    state = State.RANGEATTACK;
                //    anim.SetBool("isRangeAttack", true);
                //}
                //else
                //{
                //    state = State.LASERCAST;
                //    // 레이저에 해당되는 애니메이션 세팅
                //}
            }

            if (IsEnemyClosetPlayer() &&
                state == State.MOVE)
            {
                //float rand = Random.Range(0, 100f);
                if (armorBuffCoolTime <= 0.0f)
                {
                    armorBuffCoolTime = armorBuffDefaultCoolTime;
                    agent.isStopped = true;
                    state = State.ARMORBUFF;
                }
                //if (spcialLazerCoolTime <= 0.0f)
                //{
                //    spcialLazerCoolTime = spcialLazerDefaultCoolTime;
                //    agent.isStopped = true;
                //    state = State.SPECIAL_LAZER;
                //}
                //else
                //{
                //    state = State.MELEE;
                //}
            }

            CoolTimeCalculator();
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
                //RangeAttack();
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
                SecialLazerEndCheck();
                SpecialLazerAnimation();
                break;
            case State.ARMORBUFF:
                ArmorBuffAnimation();
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
        }
    }

    private void IdleAndMoveAnimation()
    {
        anim.SetBool("isMelee", false);
        anim.SetBool("isRangeAttack", false);
        anim.SetBool("isLazerFire", false);
        anim.SetBool("isSpecialLazerFire", false);
        anim.SetBool("isArmorBuff", false);
    }

    private void MeleeAnimation()
    {
        anim.SetBool("isMelee", true);
        anim.SetBool("isRangeAttack", false);
        anim.SetBool("isLazerFire", false);
        anim.SetBool("isSpecialLazerFire", false);
        anim.SetBool("isArmorBuff", false);
    }

    private void RangeAttackAnimation()
    {
        anim.SetBool("isMelee", false);
        anim.SetBool("isRangeAttack", true);
        anim.SetBool("isLazerFire", false);
        anim.SetBool("isSpecialLazerFire", false);
        anim.SetBool("isArmorBuff", false);
    }

    private void LazerAnimation()
    {
        anim.SetBool("isMelee", false);
        anim.SetBool("isRangeAttack", false);
        anim.SetBool("isLazerFire", true);
        anim.SetBool("isSpecialLazerFire", false);
        anim.SetBool("isArmorBuff", false);
    }

    private void SpecialLazerAnimation()
    {
        anim.SetBool("isMelee", false);
        anim.SetBool("isRangeAttack", false);
        anim.SetBool("isLazerFire", false);
        anim.SetBool("isSpecialLazerFire", true);
        anim.SetBool("isArmorBuff", false);
    }

    private void ArmorBuffAnimation()
    {
        anim.SetBool("isMelee", false);
        anim.SetBool("isRangeAttack", false);
        anim.SetBool("isLazerFire", false);
        anim.SetBool("isSpecialLazerFire", false);
        anim.SetBool("isArmorBuff", true);
    }

    private void SecialLazerEndCheck()
    {
        if (specialLazerFire)
        {
            agent.isStopped = false;
            state = State.NORMAL;
            restTime = 0.0f;
            specialLazerFire = false;
        }
    }

    // 중간 패턴 1 : 즉사 레이저 애니메이션 이벤트 함수
    private void SpecialLazer()
    {
        GameObject lazerPrefab = Instantiate(warningLazerPrefab.gameObject, this.transform.position, Quaternion.identity);
        WarningLazer warningLazer = lazerPrefab.GetComponent<WarningLazer>();

        warningLazer.SetTarget(enemyAI.GetFocusTarget());
        specialLazerFire = true;
    }

    // 중간 패턴 2 : 아머 강화 애니메이션 이벤트 함수
    private void ArmorBuff()
    {
        
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
    }

    private void CheckLazerEnd()
    {
        bool flag = GameObject.Find("Lazer(Clone)");

        if (lazerFire && !flag)
        {
            anim.speed = 1f;
            restTime = 0.0f;
            state = State.NORMAL;
            lazerFire = false;
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
        GameObject golemArm = PhotonNetwork.Instantiate(rocketArmPrefab.name, rocketFirePoint.position, Quaternion.identity);

        RocketArm rocketArm = golemArm.GetComponent<RocketArm>();
        rocketArm.SetTargetPos(enemyAI.GetFocusTarget());
        rocketArm.SetViewID(pv.ViewID);
        rocketArm.SetDamage(enemy.enemyData.attackDamage * 1.5f);

        anim.speed = 1f;
        restTime = 0.0f;
        state = State.NORMAL;
    }

    // 플레이어한테 피격당했을 떄 RPC
    [PunRPC]
    public void DamagePlayerOnHitRPC(int playerViewID)
    {
        PhotonView playerPV = PhotonView.Find(playerViewID);
        Status status = playerPV.GetComponent<Status>();

        // 플레이어의 공격력만큼 체력에서 깎음
        if (hpBar != null)
        {
            enemy.enemyData.hp -= status.attackDamage;

            // 죽음
            if (enemy.enemyData.hp <= 0)
            {
                anim.SetTrigger("Death");
                enemyAI.enabled = false;
                isDeath = true;
                agent.isStopped = true;
                rigid.velocity = Vector2.zero;
                //dropCalc.SetLevel(status.level);    // 죽기 전에 본인을 죽인 플레이어의 레벨정보를 넘겨준다.
                //dropItem.SetCharType(status.charType);
                return;
            }
        }

        if (enemyAI.GetSecondTarget() == null)
        {
            enemyAI.aggroMeter1 += status.attackDamage;
        }
        else
        {
            if (playerViewID == enemyAI.GetFirstTarget().GetComponent<PhotonView>().ViewID)
            {
                enemyAI.aggroMeter1 += status.attackDamage;
            }
            else
            {
                enemyAI.aggroMeter2 += status.attackDamage;
            }
        }
    }

    [PunRPC]
    public void BossKnockbackRPC(Vector3 attackDirection)
    {
        playerAttackDirection = attackDirection;
        onHit = true;
        state = State.ATTACKED;
    }

    private void KnockBack()
    {
        if (onHit && enemy.enemyData.hp > 0 && state != State.LAZERCAST)
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
                state = State.NORMAL;
                restTime = 2.0f;
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
            }
        }
    }

    // HP UI 생성 및 세팅
    public void HPInitSetting()
    {
        hpBar = Instantiate(HPBar, Vector2.zero, Quaternion.identity, canvas.transform);
        hpBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 400);
        hpBar.maxValue = enemy.enemyData.hp;
        enemy.enemyData.maxHp = enemy.enemyData.hp;
        hpBar.value = enemy.enemyData.hp;

        hpBar.GetComponentInChildren<Text>().text = enemy.enemyData.name;
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
        if (agent.isStopped && isMelee)
        {
            rigid.velocity = (targetPos - this.transform.position) * attackDistanceSpeed;

            float attackSpeedDropMultiplier = 6.5f;

            attackDistanceSpeed -= attackDistanceSpeed * attackSpeedDropMultiplier * Time.deltaTime;

            if (attackDistanceSpeed < 1f)
            {
                isMelee = false;
                rigid.velocity = Vector2.zero;
                attackDistanceSpeed = 8f;
                state = State.NORMAL;
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

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint == null)
            return;

        Gizmos.DrawWireSphere(detectionPoint.position, detectionRange);

        if (attackPoint == null)
            return;

        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

    private void OnDrawGizmos()
    {
        if (rocketAndLaserPoint != null)
        {
            Gizmos.color = Color.red; // 기즈모의 색상 설정
            // 직사각형 중심 위치
            Vector3 center = rocketAndLaserPoint.position;
            // 직사각형의 크기 설정
            Vector3 size = new Vector3(rectangleSize.x, rectangleSize.y, 0.0f);
            // 직사각형 그리기
            Gizmos.DrawWireCube(center, size);
        }
    }
}
