using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class EnemyAI : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public EnemySO enemySO;

    private NavMeshAgent agent;

    [SerializeField] private Transform target1, target2; // 플레이어

    [SerializeField] private float waitForSec = 1f;

    private PhotonView enemyPV;

    private Transform focusTarget = null;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        enemyPV = this.GetComponent<PhotonView>();

        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

        // 플레이어가 던전에 혼자 들어오는 경우 고려
        if (targets.Length > 1)
        {
            target1 = targets[0].transform;
            target2 = targets[1].transform;
        }
        else
        {
            target1 = targets[0].transform;
        }

        StartCoroutine(GetTimeToFacePlayer());
    }

    // Update is called once per frame
    void Update()
    {
        if (focusTarget != null)
        {
            // 특정 타겟으로 지정한 대상이 특정 이유(게임 종료)로 null 상태가 되었을 때
            if (agent.destination == null)
            {
                focusTarget = GameObject.FindGameObjectWithTag("Player").transform;
            }

            // 타겟1이 포커싱 상태일 때
            if (focusTarget == target1)
            {
                agent.SetDestination(target1.position);
                FlipHorizontalRelativeToTarget(target1.position);
            }
            // 타겟2가 포커싱 상태일 때
            else
            {
                agent.SetDestination(target2.position);
                FlipHorizontalRelativeToTarget(target2.position);
            }

            // 일정 거리가 좁혀지면 멈춤
            if (IsEnemyClosetPlayer())
            {
                agent.SetDestination(this.transform.position);
            }
            else
            {
                agent.SetDestination(focusTarget.position);
            }
        }
    }

    // 현재 타겟이 없을 때
    public bool IsFocusTargetNull()
    {
        if (focusTarget == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Transform GetTarget()
    {
        return focusTarget;
    }

    // 적이 플레이어를 공격할 수 있는 위치인가?
    public bool IsEnemyClosetPlayer()
    {
        //float dist = Vector2.Distance(this.transform.position, focusTarget.position);
        float xAbs = Mathf.Abs(this.transform.position.x - focusTarget.position.x);
        float yAbs = Mathf.Abs(this.transform.position.y - focusTarget.position.y);
        if (xAbs < 2.5f && yAbs < 0.7f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 플레이어를 발견
    IEnumerator GetTimeToFacePlayer()
    {
        yield return new WaitForSeconds(waitForSec);
        FollowClosestPlayer();

        spriteRenderer.color = Color.red;
    }

    // 타겟 위치에 따른 좌우 반전
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

    // 처음 던전에 입장할 때 두 플레이어의 거리를 비교하여 포커스 함.
    private void FollowClosestPlayer()
    {
        if (target1 != null && target2 != null)
        {
            float dist1 = Vector2.Distance(this.transform.position, target1.position);
            float dist2 = Vector2.Distance(this.transform.position, target2.position);

            // dist값이 크다 = 거리가 멀다
            if (dist1 > dist2)
            {
                focusTarget = target2;
            }
            else
            {
                focusTarget = target1;
            }
        }
        else
        {
            focusTarget = target1;
        }
    }
}
