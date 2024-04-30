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
    NORMAL, MOVE, ATTACK, DIE
}

public class EnemyCtrl : MonoBehaviour
{
    private BoxCollider2D collider;

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
    // Start is called before the first frame update
    private void Start()
    {
        enemy.InitSetting();

        collider = GetComponent<BoxCollider2D>();

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
        hpBar.value = hpBar.maxValue;
    }

    public void SetEnemy(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public Enemy GetEnemy()
    {
        return enemy;
    }

    private void FixedUpdate()
    {
        if (!isEnemyDead)
        {
            // 죽음
            if (enemy.enemyData.hp <= 0)
            {
                StartCoroutine(Death());
            }

            // 속도가 0이 아니면 이동상태
            if (agent.velocity != Vector3.zero && state != State.ATTACK)
            {
                state = State.MOVE;
            }

            // 적의 타겟이 존재할 때
            if (!enemyAIScript.IsFocusTargetNull())
            {
                // 적이 플레이어와 어느정도 가까이 있으면 공격
                if (enemyAIScript.IsEnemyClosetPlayer() && state != State.ATTACK)
                {
                    state = State.ATTACK;
                    targetPos = enemyAIScript.GetTarget().position;
                    agent.isStopped = true;
                    enemyAIScript.isLookingAtPlayer = false;
                }
            }
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.NORMAL:
                IdleAnimation();
                break;
            case State.MOVE:
                MoveAnimation();
                break;
            case State.ATTACK:
                Attack();
                AttackAnimation();
                break;
            case State.DIE:
                break;
        }

        //if (hpBar != null)
        //    FollowEnemyHPBar();

        // 넉백
        KnockBack();
    }

    IEnumerator Death()
    {
        // 죽기전에 처리해야할 것들
        isEnemyDead = true;

        enemyAIScript.enabled = false;
        //anim.SetTrigger("Die");

        yield return new WaitForSeconds(3.0f);

        DestroyHPBar();
        enemyManagerScript.RemoveEnemy(this.gameObject);
        PhotonNetwork.Destroy(this.gameObject);
    }

    // 피격
    [PunRPC]
    public void EnemyAttackedPlayer(int damagedHP)
    {
        enemy.enemyData.hp -= damagedHP;
        onHit = true;
    }

    // 공격
    private void Attack()
    {
        rigid.velocity = (targetPos - this.transform.position).normalized * attackedDistanceSpeed;

        float attackSpeedDropMultiplier = 6f;

        attackDistanceSpeed -= attackDistanceSpeed * attackSpeedDropMultiplier * Time.deltaTime;

        if (attackDistanceSpeed < 0.05f)
        {
            rigid.velocity = Vector2.zero;
            attackDistanceSpeed = 10f;
            state = State.NORMAL;
            agent.isStopped = false;
            enemyAIScript.isLookingAtPlayer = true;
        }

    }

    private void KnockBack()
    {
        if (onHit && enemy.enemyData.hp > 0)
        {
            anim.speed = 0f;

            // 바라보는 방향 뒤로
            if (this.transform.localScale.x > 0)
            {
                rigid.velocity = -transform.right * attackedDistanceSpeed;
            }
            else
            {
                rigid.velocity = transform.right * attackedDistanceSpeed;
            }

            float attackedSpeedDropMultiplier = 6f;
            attackedDistanceSpeed -= attackedDistanceSpeed * attackedSpeedDropMultiplier * Time.deltaTime;

            if (attackedDistanceSpeed < 0.6f)
            {
                rigid.velocity = Vector2.zero;
                onHit = false;
                attackedDistanceSpeed = 3f;
                anim.speed = 1f;
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
        hpBar.transform.position = new Vector2(enemyPos.x, enemyPos.y - 100);
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && state == State.ATTACK)
        {
            status = other.GetComponent<Status>();
            //BoxCollider2D boxCol = other.GetComponent<BoxCollider2D>();

            status.HP -= enemy.enemyData.attackDamage;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        status = null;
    }
}
