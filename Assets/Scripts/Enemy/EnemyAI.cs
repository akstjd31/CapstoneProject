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

    private Status status1, status2;

    [SerializeField] private float waitForSec = 1f;

    private PhotonView enemyPV;

    private Transform focusTarget = null;

    public bool isLookingAtPlayer = false;

    private bool isTargetPathMissing = false;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        enemyPV = this.GetComponent<PhotonView>();
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

                status1 = target1.gameObject.GetComponent<Status>();
                status2 = target2.gameObject.GetComponent<Status>();

                StartCoroutine(GetTimeToFacePlayer());
            }
            //else if (GameObject.FindGameObjectsWithTag("Player").Length == 1)
            //{
            //    GameObject target = GameObject.FindGameObjectWithTag("Player");

            //    target1 = target.transform;

            //    status1 = target1.gameObject.GetComponent<Status>();
            //}
        }

        if (focusTarget != null)
        {
            // 만약 타겟으로 향하는 path가 존재하지 않을 경우
            if (agent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                agent.isStopped = true;
                isTargetPathMissing = true;

            }
            else
            {
                // path가 존재하지 않다가 생기면 다시 타겟을 설정한다.
                if (agent.isStopped && isTargetPathMissing)
                {
                    StartCoroutine(GetTimeToFacePlayer());
                    agent.isStopped = false;
                    isTargetPathMissing = false;
                }

                // 포커싱 == 타겟1
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

            CheckAggroMeterAndChangeFocus();
        }
    }

    private void CheckAggroMeterAndChangeFocus()
    {
        if (target1 != null && target2 != null)
        {
            // 어그로 미터에 따른 타겟 변경
            if (status1.agroMeter > status2.agroMeter)
            {

                focusTarget = target1;
            }
            else
            {
                focusTarget = target2;
            }
        }
    }

    public Transform GetTarget()
    {
        return focusTarget;
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
