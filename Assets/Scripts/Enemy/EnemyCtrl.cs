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
    NORMAL, MOVE, ATTACK, ATTACKIDLE, DIE
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

    [SerializeField] private float attackDelay;

    public bool onHit = false;

    private float attackDistanceSpeed = 5f;
    private float attackedDistanceSpeed = 3f;

    private bool isEnemyDead = false;

    private Status status;
    // Start is called before the first frame update
    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();

        enemyPV = this.GetComponent<PhotonView>();
        anim = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
        enemyAIScript = this.GetComponent<EnemyAI>();
        rigid = this.GetComponent<Rigidbody2D>();

        enemyManagerScript = GameObject.FindGameObjectWithTag("EnemyManager").GetComponent<EnemyManager>();

        canvas = GameObject.FindGameObjectWithTag("Canvas");

        HPInitSetting();

        state = State.NORMAL;

        attackDelay = 999;

        status = null;

    }

    // 몬스터 HP바 세팅
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

    // 적의 상태(state)를 처리하는 부분
    private void FixedUpdate()
    {
        if (!isEnemyDead)
        {
            // 죽음 
            if (enemy.enemyData.hp <= 0)
            {
                StartCoroutine(Death());
            }

            // 이동
            if (agent.velocity != Vector3.zero)
            {
                attackDelay = 999;
                state = State.MOVE;
            }
            // 공격 or 공격준비
            else
            {
                // 설정해놓은 타깃이 존재할 때
                if (!enemyAIScript.IsFocusTargetNull())
                {
                    // 몬스터마다 주어진 공격딜레이 적용
                    if (attackDelay > enemy.enemyData.attackDelayTime)
                    {
                        // 타깃과 적이 가깝다고 판단하면 공격
                        if (enemyAIScript.IsEnemyClosetPlayer())
                        {
                            state = State.ATTACK;
                            attackDelay = 0.0f;
                        }
                    }
                    else
                    {
                        state = State.ATTACKIDLE;

                        attackDelay += Time.deltaTime;
                    }
                }
            }
        }
    }

    // 애니메이션 및 기타함수 실행
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
                //Attack();
                AttackAnimation();
                break;
            case State.ATTACKIDLE:
                AttackIdleAnimation();
                break;
            case State.DIE:
                break;
        }

        // 체력바가 존재할때만 적 따라다님.
        if (hpBar != null)
            FollowEnemyHPBar();

        // 넉백
        KnockBack();
    }

    IEnumerator Death()
    {
        // 적이 쓰러지기 직전 해야하는 것들
        isEnemyDead = true;

        enemyAIScript.enabled = false;
        anim.SetTrigger("Die");

        yield return new WaitForSeconds(3.0f);

        // 이후 처리
        DestroyHPBar();
        enemyManagerScript.RemoveEnemy(this.gameObject);
        PhotonNetwork.Destroy(this.gameObject);
    }

    // 플레이어에게 맞았을 떄
    [PunRPC]
    public void EnemyAttackedPlayer(int damagedHP)
    {
        enemy.enemyData.hp -= damagedHP;
        onHit = true;
    }

    // 이동 공격
    private void Attack()
    {
        // 공격 방향 결정
        //if (this.transform.localScale.x > 0)
        //{
        //    rigid.velocity = transform.right * attackedDistanceSpeed;
        //}
        //else
        //{
        //    rigid.velocity = -transform.right * attackedDistanceSpeed;
        //}

        rigid.velocity = (enemyAIScript.GetTarget().position - this.transform.position).normalized * attackedDistanceSpeed;

        float attackSpeedDropMultiplier = 6f; // 감소되는 정도

        attackDistanceSpeed -= attackDistanceSpeed * attackSpeedDropMultiplier * Time.deltaTime; // 실제 나아가는 거리 계산

        if (attackDistanceSpeed < 0.3f)
        {
            rigid.velocity = Vector2.zero;
            attackDistanceSpeed = 5f;

            state = State.ATTACKIDLE;
        }

    }

    // 넉백
    private void KnockBack()
    {
        if (onHit && enemy.enemyData.hp > 0)
        {
            anim.speed = 0f;

            // 오른쪽 / 왼쪽을 바라보는 기준으로 뒤로 넉백
            if (this.transform.localScale.x > 0)
            {
                rigid.velocity = -transform.right * attackedDistanceSpeed;
            }
            else
            {
                rigid.velocity = transform.right * attackedDistanceSpeed;
            }

            float attackedSpeedDropMultiplier = 6f; // 감소되는 정도
            attackedDistanceSpeed -= attackedDistanceSpeed * attackedSpeedDropMultiplier * Time.deltaTime; // 실제 나아가는 거리 계산

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

    // HP바가 적을 따라다님.
    private void FollowEnemyHPBar()
    {
        Vector3 enemyPos = Camera.main.WorldToScreenPoint(this.transform.position);
        hpBar.transform.position = new Vector2(enemyPos.x, enemyPos.y - 100);
        hpBar.value = enemy.enemyData.hp;
    }

    private void IdleAnimation()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("AttackIdle", false);
        anim.SetBool("isMove", false);
    }

    private void MoveAnimation()
    {
        anim.SetBool("isAttack", false);
        anim.SetBool("AttackIdle", false);
        anim.SetBool("isMove", true);
    }

    private void AttackAnimation()
    {
        anim.SetBool("isMove", false);
        anim.SetBool("AttackIdle", false);
        anim.SetBool("isAttack", true);
    }

    private void AttackIdleAnimation()
    {
        anim.SetBool("isMove", false);
        anim.SetBool("isAttack", false);
        anim.SetBool("AttackIdle", true);
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
