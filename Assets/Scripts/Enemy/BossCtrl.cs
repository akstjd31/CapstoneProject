using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class BossCtrl : MonoBehaviour
{
    // 애니메이션 종류
    // 근접 펀치, 단단해지기, 레이저 빔, 무적?
    
    public enum State
    { 
        NORMAL, MOVE, GLOWING, RANGEATTACK, IMMUNE, MELEE, LASERCAST, ARMORBUFF, DEATH
    }

    public Enemy enemy;

    private Animator anim;
    private EnemyAI enemyAI;
    private NavMeshAgent agent;

    [SerializeField] private State state;

    // 플레이어 발견 범위
    public Transform detectionPoint;
    public float detectionRange;
    public LayerMask playerLayers;

    // 근접 공격 범위
    public Transform attackPoint;
    public float attackRange = 0.5f;

    private void Awake()
    {
        enemy.InitSetting();
    }

    private void Start()
    {
        enemyAI = this.GetComponent<EnemyAI>();
        agent = this.GetComponent<NavMeshAgent>();
        anim = this.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (agent.velocity != Vector3.zero)
        {
            state = State.MOVE;
        }

        if (IsEnemyClosetPlayer())
        {
            agent.isStopped = true;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case State.NORMAL:
                break;
            case State.RANGEATTACK:
                break;
        }

        if (enemyAI.GetFocusTarget() != null && enemyAI.isLookingAtPlayer)
        {
            FlipHorizontalRelativeToTarget(enemyAI.GetFocusTarget().position);
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

    private void OnDrawGizmosSelected()
    {
        if (detectionPoint == null)
            return;

        Gizmos.DrawWireSphere(detectionPoint.position, detectionRange);
    }
}
