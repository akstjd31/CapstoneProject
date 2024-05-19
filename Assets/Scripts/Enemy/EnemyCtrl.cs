using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class EnemyCtrl : MonoBehaviour
{
    public enum State
    {
        NORMAL, MOVE, ATTACK, ATTACKED, DIE
    }

    private PhotonView pv;
    private Animator anim;
    private NavMeshAgent agent;
    private EnemyAI enemyAI;
    private DropChanceCalculator dropCalc;
    private DropItem dropItem;
    private Rigidbody2D rigid;
    
    [SerializeField] public Enemy enemy;

    public Slider HPBar;
    [SerializeField] private Slider hpBar;

    private GameObject canvas;

    [SerializeField] private State state;

    public bool onHit = false;

    private float attackDistanceSpeed = 10f;
    private float attackedDistanceSpeed = 3f;

    private bool isDeath = false;
    private float deathTime = 1.0f;

    private Status status;

    private Vector3 targetPos;

    public Vector3 playerAttackDirection;

    public Transform detectionPoint;
    public float detectionRange;
    public LayerMask playerLayers;

    [SerializeField] private float restTime = 0.0f;

    // 근접 공격 포인트와 범위
    public Transform attackPoint;
    public float attackRange = 0.5f;

    // 원거리 발사 위치와 발사체
    public Transform firePoint;
    public Transform projectile;

    public Transform goldPrefab;

    private EnemySound enemySound;

    // Start is called before the first frame update
    private void Start()
    {
        pv = this.GetComponent<PhotonView>();
        anim = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
        enemyAI = this.GetComponent<EnemyAI>();
        dropItem = this.GetComponent<DropItem>();
        dropCalc = this.GetComponent<DropChanceCalculator>();
        rigid = this.GetComponent<Rigidbody2D>();
        enemySound = this.GetComponent<EnemySound>();
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        

        enemy.InitSetting();

        detectionRange = enemy.enemyData.enemyType == EnemyType.LONG_DISTANCE ? 7 : 3;  // 원거리, 근거리몹에 따라 범위가 다름.

        HPInitSetting();
        ChangeState(State.NORMAL);

        status = null;
    }

    // HP UI 생성 및 세팅
    public void HPInitSetting()
    {
        hpBar = Instantiate(HPBar, Vector2.zero, Quaternion.identity, canvas.transform);
        hpBar.maxValue = enemy.enemyData.hp;
        enemy.enemyData.maxHp = enemy.enemyData.hp;
        hpBar.value = enemy.enemyData.hp;
    }

    public void ChangeState(State state)
    {
        pv.RPC("ChangeStateRPC", RpcTarget.All, (int)state);
    }

    [PunRPC]
    private void ChangeStateRPC(int state)
    {
        this.state = (State)state;
    }

    public Enemy GetEnemy()
    {
        return enemy;
    }

    // 플레이어한테 피격당했을 떄 RPC
    [PunRPC]
    public void DamagePlayerOnHitRPC(int playerViewID)
    {
        PhotonView playerPV = PhotonView.Find(playerViewID);
        //PassiveSkill passiveSkill = playerPV.GetComponent<PassiveSkill>();
        Status status = playerPV.GetComponent<Status>();

        //passiveSkill.attackCount++;
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
                dropCalc.SetLevel(status.level);    // 죽기 전에 본인을 죽인 플레이어의 레벨정보를 넘겨준다.
                dropItem.SetCharType(status.charType);
                
                status.curExp += 1;
                status.CheckLevelUp();

                Status targetStatus = null;
                // 적을 잡으면 모든 플레이어한테 골드, 경험치를 부여
                if (playerPV.transform.Equals(enemyAI.GetFirstTarget()))
                {
                    targetStatus = enemyAI.GetSecondTarget() != null ? enemyAI.GetSecondTarget().GetComponent<Status>() : null;

                    if (targetStatus != null)
                    {
                        targetStatus.curExp += 1;
                        targetStatus.CheckLevelUp();
                    }
                }
                else
                {
                    targetStatus = enemyAI.GetFirstTarget() != null ? enemyAI.GetFirstTarget().GetComponent<Status>() : null;

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
    public void EnemyKnockbackRPC(Vector3 attackDirection)
    {
        playerAttackDirection = attackDirection;
        onHit = true;
        ChangeState(State.ATTACKED);
    }

    private void FixedUpdate()
    {
        if (!isDeath)
        {
            // 속도가 0이 아니면 이동상태
            if (agent.velocity != Vector3.zero && 
                state != State.ATTACK && 
                !onHit &&
                restTime >= enemy.enemyData.attackDelayTime)
            {
                ChangeState(State.MOVE);
            }

            // 적의 타겟이 존재할 때
            if (enemyAI.GetFocusTarget() != null)
            {
                // 적이 플레이어와 어느정도 가까이 있으면 공격
                if (IsEnemyClosetPlayer() && state != State.ATTACK && state != State.NORMAL && !onHit)
                {
                    ChangeState(State.ATTACK);
                    targetPos = enemyAI.GetFocusTarget().position;
                    agent.isStopped = true;
                    enemyAI.isLookingAtPlayer = false;    // 공격할 때 플레이어가 움직여도 그 방향 유지

                }
            }
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.NORMAL:
                attackAndRest();
                IdleAnimation();
                break;
            case State.MOVE:
                MoveAnimation();
                break;
            case State.ATTACK:
                Attack();
                AttackDirection();
                AttackAnimation();
                break;
            case State.ATTACKED:
                KnockBack();
                break;
            case State.DIE:
                Death();
                break;
        }

        if (hpBar != null)
        {
            FollowEnemyHPBar();
        }

        if (enemyAI.GetFocusTarget() != null && enemyAI.isLookingAtPlayer)
        {
            FlipHorizontalRelativeToTarget(enemyAI.GetFocusTarget().position);
        }
    }

    // 플레이어가 소유한 범위포인트에 따른 반환
    private bool IsEnemyClosetPlayer()
    {
        if (enemyAI.GetFocusTarget() != null && state != State.ATTACK)
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

    public void DeathAnimEvent()
    {
        enemySound.PlayDeathSound();
        ChangeState(State.DIE);
        anim.speed = 0f;
    }

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
            PhotonNetwork.Destroy(this.gameObject);
        }
    }


    private void Attack()
    {
        // 근접
        if (enemy.enemyData.enemyType == EnemyType.SHORT_DISTANCE)
        {
            // 공격 범위에 포함된 플레이어
            Collider2D hitPlayers = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayers);

            if (hitPlayers != null)
            {
                PlayerCtrl player = hitPlayers.GetComponent<PlayerCtrl>();
                if (player != null && !player.onHit)
                {
                    // 몹의 데미지만큼 플레이어에게 피해를 입힘.
                    //player.GetComponent<PhotonView>().RPC("DamageEnemyOnHitRPC", RpcTarget.All, player.passiveSkill.PrideDamaged(enemy.enemyData.attackDamage));
                    player.GetComponent<PhotonView>().RPC("DamageEnemyOnHitRPC", RpcTarget.All, enemy.enemyData.attackDamage);
                    player.GetComponent<PhotonView>().RPC("PlayerKnockbackRPC", RpcTarget.All, pv.ViewID, targetPos - this.transform.position);

                    hitPlayers.GetComponent<PlayerSound>().PlayAttackedSound();
                }
            }
        }
        else
        {
        }
    }

    // 적의 위치에 따른 스케일 뒤집기
    private void FlipHorizontalRelativeToTarget(Vector2 target)
    {
        if (target.x - this.transform.position.x > 0)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    // 공격 방향
    private void AttackDirection()
    {
        // 근접
        if (enemy.enemyData.enemyType == EnemyType.SHORT_DISTANCE)
        {
            rigid.velocity = (targetPos - this.transform.position) * attackedDistanceSpeed;

            float attackSpeedDropMultiplier = 6f;

            attackDistanceSpeed -= attackDistanceSpeed * attackSpeedDropMultiplier * Time.deltaTime;

            if (attackDistanceSpeed < 0.3f)
            {
                rigid.velocity = Vector2.zero;
                attackDistanceSpeed = 10f;
                ChangeState(State.NORMAL);
                restTime = 0.0f;
            }
        }
        // 원거리
        else
        {
        }
    }

    // 애니메이션 이벤트 호출 함수
    public void Fire()
    {
        // 총알 프리팹을 firePoint 위치에 인스턴스화
        GameObject arrowPrefab = PhotonNetwork.Instantiate(projectile.name, firePoint.position, firePoint.rotation);

        // PhotonView와 Arrow 컴포넌트를 가져옴
        PhotonView arrowPV = arrowPrefab.GetComponent<PhotonView>();
        if (arrowPV == null)
        {
            Debug.LogError("PhotonView component is missing on the instantiated arrow prefab.");
            return;
        }

        Arrow arrow = arrowPV.GetComponent<Arrow>();
        if (arrow == null)
        {
            Debug.LogError("Arrow component is missing on the instantiated arrow prefab.");
            return;
        }

        // RPC를 사용하여 다른 클라이언트에 총알의 초기 설정 전송
        arrowPV.RPC("InitializeArrow", RpcTarget.AllBuffered, targetPos, 3.5f, enemy.enemyData.attackDamage, this.tag, pv.ViewID);

        // 상태 변경
        ChangeState(State.NORMAL);
        restTime = 0.0f;
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

    private void KnockBack()
    {
        if (onHit && enemy.enemyData.hp > 0)
        {
            anim.Play("Idle", -1, 0f);
            anim.speed = 0;
            rigid.velocity = Vector2.zero;
            agent.isStopped = true;

            rigid.velocity = playerAttackDirection * attackedDistanceSpeed;

            float attackedSpeedDropMultiplier = 6f;
            attackedDistanceSpeed -= attackedDistanceSpeed * attackedSpeedDropMultiplier * Time.deltaTime;

            if (attackedDistanceSpeed < 0.05f)
            {
                rigid.velocity = Vector2.zero;
                onHit = false;
                attackedDistanceSpeed = 3f;
                anim.speed = 1f;
                ChangeState(State.NORMAL);
                restTime = 0.0f;
            }
        }
    }

    // HP UI가 적을 따라다님.
    private void FollowEnemyHPBar()
    {
        Vector3 enemyPos = Camera.main.WorldToScreenPoint(this.transform.position);
        hpBar.transform.position = new Vector2(enemyPos.x, enemyPos.y - 30);
        hpBar.value = enemy.enemyData.hp;
    }

    private void IdleAnimation()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isMove", false);
    }

    private void MoveAnimation()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("isMove", true);
    }

    private void AttackAnimation()
    {
        anim.SetBool("isMove", false);
        anim.SetBool("isAttack", true);
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

    //private void OnTriggerStay2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player") && state == State.ATTACK)
    //    {
    //        status = other.GetComponent<Status>();
    //        //BoxCollider2D boxCol = other.GetComponent<BoxCollider2D>();

    //        status.HP -= enemy.enemyData.attackDamage;
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D other)
    //{
    //    status = null;
    //}
}
