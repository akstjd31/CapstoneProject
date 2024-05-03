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

    // ���� HP�� ����
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

    // ���� ����(state)�� ó���ϴ� �κ�
    private void FixedUpdate()
    {
        if (!isEnemyDead)
        {
            // ���� 
            if (enemy.enemyData.hp <= 0)
            {
                StartCoroutine(Death());
            }

            // �̵�
            if (agent.velocity != Vector3.zero)
            {
                attackDelay = 999;
                state = State.MOVE;
            }
            // ���� or �����غ�
            else
            {
                // �����س��� Ÿ���� ������ ��
                if (!enemyAIScript.IsFocusTargetNull())
                {
                    // ���͸��� �־��� ���ݵ���� ����
                    if (attackDelay > enemy.enemyData.attackDelayTime)
                    {
                        // Ÿ��� ���� �����ٰ� �Ǵ��ϸ� ����
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

    // �ִϸ��̼� �� ��Ÿ�Լ� ����
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

        // ü�¹ٰ� �����Ҷ��� �� ����ٴ�.
        if (hpBar != null)
            FollowEnemyHPBar();

        // �˹�
        KnockBack();
    }

    IEnumerator Death()
    {
        // ���� �������� ���� �ؾ��ϴ� �͵�
        isEnemyDead = true;

        enemyAIScript.enabled = false;
        anim.SetTrigger("Die");

        yield return new WaitForSeconds(3.0f);

        // ���� ó��
        DestroyHPBar();
        enemyManagerScript.RemoveEnemy(this.gameObject);
        PhotonNetwork.Destroy(this.gameObject);
    }

    // �÷��̾�� �¾��� ��
    [PunRPC]
    public void EnemyAttackedPlayer(int damagedHP)
    {
        enemy.enemyData.hp -= damagedHP;
        onHit = true;
    }

    // �̵� ����
    private void Attack()
    {
        // ���� ���� ����
        //if (this.transform.localScale.x > 0)
        //{
        //    rigid.velocity = transform.right * attackedDistanceSpeed;
        //}
        //else
        //{
        //    rigid.velocity = -transform.right * attackedDistanceSpeed;
        //}

        rigid.velocity = (enemyAIScript.GetTarget().position - this.transform.position).normalized * attackedDistanceSpeed;

        float attackSpeedDropMultiplier = 6f; // ���ҵǴ� ����

        attackDistanceSpeed -= attackDistanceSpeed * attackSpeedDropMultiplier * Time.deltaTime; // ���� ���ư��� �Ÿ� ���

        if (attackDistanceSpeed < 0.3f)
        {
            rigid.velocity = Vector2.zero;
            attackDistanceSpeed = 5f;

            state = State.ATTACKIDLE;
        }

    }

    // �˹�
    private void KnockBack()
    {
        if (onHit && enemy.enemyData.hp > 0)
        {
            anim.speed = 0f;

            // ������ / ������ �ٶ󺸴� �������� �ڷ� �˹�
            if (this.transform.localScale.x > 0)
            {
                rigid.velocity = -transform.right * attackedDistanceSpeed;
            }
            else
            {
                rigid.velocity = transform.right * attackedDistanceSpeed;
            }

            float attackedSpeedDropMultiplier = 6f; // ���ҵǴ� ����
            attackedDistanceSpeed -= attackedDistanceSpeed * attackedSpeedDropMultiplier * Time.deltaTime; // ���� ���ư��� �Ÿ� ���

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

    // HP�ٰ� ���� ����ٴ�.
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
