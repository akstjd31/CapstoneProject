using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public enum State
{ 
    NORMAL, MOVE, ATTACK, ATTACKED, DIE
}

public class EnemyCtrl : MonoBehaviour
{
    private PhotonView enemyPV;
    private Animator anim;
    private NavMeshAgent agent;
    private EnemyAI enemyAIScript;
    private Rigidbody2D rigid;
    
    private EnemyManager enemyManagerScript;
    [SerializeField] private Enemy enemy;

    public Slider HPBar;
    [SerializeField] private Slider hpBar;

    private GameObject canvas;

    [SerializeField] private State state;

    public bool onHit = false;

    private float attackDistanceSpeed = 10f;
    private float attackedDistanceSpeed = 3f;

    private bool isEnemyDead = false;

    private Status status;

    private Vector3 targetPos;

    public Vector3 playerAttackDirection;

    public Transform detectionPoint;
    public float detectionRange = 3f;
    public LayerMask playerLayers;

    [SerializeField] private float restTime = 0.0f;

    // 공격 포인트와 범위
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;

    Collider2D hitPlayers;

    // Start is called before the first frame update
    private void Start()
    {
        enemy.InitSetting();

        enemyPV = this.GetComponent<PhotonView>();
        anim = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
        enemyAIScript = this.GetComponent<EnemyAI>();
        rigid = this.GetComponent<Rigidbody2D>();

        //enemyManagerScript = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();

        canvas = GameObject.FindGameObjectWithTag("Canvas");

        HPInitSetting();

        state = State.NORMAL;

        status = null;
    }

    // HP UI 생성 및 세팅
    public void HPInitSetting()
    {
        hpBar = Instantiate(HPBar, Vector2.zero, Quaternion.identity, canvas.transform);
        hpBar.maxValue = enemy.enemyData.hp;
        hpBar.value = enemy.enemyData.hp;
    }

    public void SetState(State state)
    {
        this.state = state;
    }

    public void SetEnemy(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public Slider GetHPSlider()
    {
        return hpBar;
    }

    private void FixedUpdate()
    {
        if (!isEnemyDead)
        {
            // 죽음
            if (hpBar.value <= 0)
            {
                StartCoroutine(Death());
            }

            // 속도가 0이 아니면 이동상태
            if (agent.velocity != Vector3.zero && 
                state != State.ATTACK && 
                state != State.ATTACKED &&
                restTime >= 1.0f)
            {
                state = State.MOVE;
            }

            // 적의 타겟이 존재할 때
            if (enemyAIScript.GetTarget() != null)
            {
                // 적이 플레이어와 어느정도 가까이 있으면 공격
                if (IsEnemyClosetPlayer() && state != State.ATTACK && state != State.NORMAL && !onHit)
                {
                    state = State.ATTACK;
                    targetPos = enemyAIScript.GetTarget().position;
                    agent.isStopped = true;
                    enemyAIScript.isLookingAtPlayer = false;    // 공격할 때 플레이어가 움직여도 그 방향 유지
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
                break;
        }

        if (hpBar != null)
            FollowEnemyHPBar();

        // 넉백
        
    }

    // 플레이어가 소유한 범위포인트에 따른 반환
    private bool IsEnemyClosetPlayer()
    {
        // 만약 근거리 몹이라면
        if (enemy.enemyData.enemyType == EnemyType.SHORT_DISTANCE && enemyAIScript.GetTarget() != null && state != State.ATTACK)
        {
            // 공격 범위에 들어간 적
            Collider2D players = Physics2D.OverlapCircle(detectionPoint.position, detectionRange, playerLayers);

            if (players != null)
            {
                return true;
            }

        }

        // 원거리 몹일 경우 ..
        else
        {

        }
        return false;
    }

    IEnumerator Death()
    {
        // 죽기전에 처리해야할 것들
        isEnemyDead = true;

        enemyAIScript.enabled = false;
        agent.isStopped = true;
        anim.SetTrigger("Death");
        yield return new WaitForSeconds(1.0f);

        DestroyHPBar();
        //enemyManagerScript.RemoveEnemy(this.gameObject);
        Destroy(this.gameObject);
    }

    // 피격
    [PunRPC]
    public void EnemyAttackedPlayer(int damagedHP)
    {
        enemy.enemyData.hp -= damagedHP;
        onHit = true;
    }


    private void Attack()
    {
        // 공격 범위에 포함된 플레이어
        hitPlayers = Physics2D.OverlapCircle(attackPoint.position, attackRange, playerLayers);
    }


    // 공격 방향
    private void AttackDirection()
    {
        rigid.velocity = (targetPos - this.transform.position) * attackedDistanceSpeed;

        float attackSpeedDropMultiplier = 6f;

        attackDistanceSpeed -= attackDistanceSpeed * attackSpeedDropMultiplier * Time.deltaTime;

        if (attackDistanceSpeed < 0.3f)
        {
            if (hitPlayers != null)
            {
                hitPlayers.GetComponent<Status>().HP -= enemy.enemyData.attackDamage;
                Debug.Log(hitPlayers.GetComponent<Status>().HP);
            }

            rigid.velocity = Vector2.zero;
            attackDistanceSpeed = 10f;
            state = State.NORMAL;
            restTime = 0.0f;

        }
    }

    // 공격이나 피격당할 때 1초 쉬기
    private void attackAndRest()
    {
        if (restTime <= 1.0f)
        {
            restTime += Time.deltaTime;
        }
        else
        {
            agent.isStopped = false;
            enemyAIScript.isLookingAtPlayer = true;
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
                state = State.NORMAL;
                restTime = 0.0f;
            }
        }
    }

    public void DestroyHPBar()
    {
        Destroy(hpBar.gameObject);
    }

    // HP UI가 적을 따라다님.
    private void FollowEnemyHPBar()
    {
        Vector3 enemyPos = Camera.main.WorldToScreenPoint(this.transform.position);
        hpBar.transform.position = new Vector2(enemyPos.x, enemyPos.y - 30);
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
