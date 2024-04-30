using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class EnemyAI : MonoBehaviour
{
    //public SpriteRenderer spriteRenderer;
    public EnemySO enemySO;

    private NavMeshAgent agent;

    [SerializeField] private Transform target1, target2; // 2인 멀티 플레이어

    [SerializeField] private float waitForSec = 1f;

    private PhotonView enemyPV;

    private Transform focusTarget = null;

    public bool isLookingAtPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        enemyPV = this.GetComponent<PhotonView>();

        StartCoroutine(GetTimeToFacePlayer());
    }

    // Update is called once per frame
    void Update()
    {
        // 아직 해당 던전에 플레이어가 타겟에 할당되지 않았다면
        if (target1 == null && target2 == null)
        {
            if (GameObject.FindGameObjectsWithTag("Player").Length == 2)
            {
                GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

                target1 = targets[0].transform;
                target2 = targets[1].transform;
            }
            else if (GameObject.FindGameObjectsWithTag("Player").Length == 1)
            {
                GameObject target = GameObject.FindGameObjectWithTag("Player");

                target1 = target.transform;
            }
        }

        if (focusTarget != null)
        {
            // 만약 focus로 설정해놨던 플레이어가 사망 시 남은 플레이어를 포커싱
            if (agent.destination == null)
            {
                focusTarget = GameObject.FindGameObjectWithTag("Player").transform;
            }

            // 포커싱 == 타겟1일때
            if (focusTarget == target1)
            {
                agent.SetDestination(target1.position);

                if (isLookingAtPlayer)
                    FlipHorizontalRelativeToTarget(target1.position);
            }
            // 포커싱 == 타겟2
            else
            {
                agent.SetDestination(target2.position);

                if (isLookingAtPlayer)
                    FlipHorizontalRelativeToTarget(target2.position);
            }
        }
    }

    // 타겟이 없을 때
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

    // 적과 플레이어의 거리가 어느정도 좁혀졌는가?
    public bool IsEnemyClosetPlayer()
    {
        //float dist = Vector2.Distance(this.transform.position, focusTarget.position);
        float xAbs = Mathf.Abs(this.transform.position.x - focusTarget.position.x);
        float yAbs = Mathf.Abs(this.transform.position.y - focusTarget.position.y);
        if (xAbs < 2.5f && yAbs < 1f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 처음에 적이 일정시간을 기다렸다가 플레이어를 쫒아감.
    IEnumerator GetTimeToFacePlayer()
    {
        yield return new WaitForSeconds(waitForSec);
        FollowClosetPlayer();

        //spriteRenderer.color = Color.red;
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

    // 포커싱 대상 정하기 (적과 플레이어와 가까우면 포커싱)
    public void FollowClosetPlayer()
    {
        if (target1 != null && target2 != null)
        {
            float dist1 = Vector2.Distance(this.transform.position, target1.position);
            float dist2 = Vector2.Distance(this.transform.position, target2.position);

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

        isLookingAtPlayer = true;
    }
}
