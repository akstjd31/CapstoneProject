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
    private EnemyCtrl enemyCtrl;

    [SerializeField] private Transform target1, target2; // 2인 멀티 플레이어

    [SerializeField] private float waitForSec = 1f;

    private PhotonView enemyPV;

    [SerializeField] private Transform focusTarget = null;

    public bool isLookingAtPlayer = false;

    private bool focusTargetSetting = false;

    [SerializeField] private GameObject onTriggerCheckObj;
    [SerializeField] private TriggerCheck triggerCheck;

    public float aggroMeter1, aggroMeter2;  // 어그로미터기

    // Start is called before the first frame update
    void Start()
    {
        enemyCtrl = this.GetComponent<EnemyCtrl>();
        agent = this.GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        enemyPV = this.GetComponent<PhotonView>();

        aggroMeter1 = 0; aggroMeter2 = 0;
    }

    // Update is called once per frame
    void Update()
    {
        // 현재 플레이어가 해당 방에 존재할 때
        if (triggerCheck != null)
        {
            if (triggerCheck.isPlayerInRoom)
            {
                // 아직 해당 던전에 플레이어가 타겟에 할당되지 않았다면
                if (target1 == null && target2 == null)
                {
                    if (GameObject.FindGameObjectsWithTag("Player").Length == 2)
                    {
                        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");

                        target1 = targets[0].transform;
                        target2 = targets[1].transform;

                        StartCoroutine(GetTimeToFacePlayer());
                    }
                    else if (GameObject.FindGameObjectsWithTag("Player").Length == 1)
                    {
                        GameObject target = GameObject.FindGameObjectWithTag("Player");

                        target1 = target.transform;

                        StartCoroutine(GetTimeToFacePlayer());
                    }
                    else
                    {
                        agent.isStopped = true;
                    }
                }

                if (focusTarget != null)
                {
                    agent.SetDestination(focusTarget.position);

                    CheckAggroMeterAndChangeFocus();
                }
                else
                {
                    // 포커스했던 플레이어가 죽을 시 다시 체크
                    if (isLookingAtPlayer && focusTargetSetting)
                    {
                        FollowClosetPlayer();
                    }
                }
            }
            else
            {
                agent.isStopped = true;
            }
        }
    }

    public TriggerCheck GetTriggerCheck()
    {
        return triggerCheck;
    }

    private void CheckAggroMeterAndChangeFocus()
    {
        if (target1 != null && target2 != null)
        {
            // 어그로 미터에 따른 타겟 변경
            if (aggroMeter1 > aggroMeter2)
            {

                focusTarget = target1;
            }
            else
            {
                focusTarget = target2;
            }
        }
    }

    public Transform GetFirstTarget()
    {
        return target1;
    }

    public Transform GetSecondTarget()
    {
        return target2;
    }

    public Transform GetFocusTarget()
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

    // 포커싱 대상 정하기 (적과 플레이어와 가까우면 포커싱)
    public void FollowClosetPlayer()
    {
        // 남아있는 플레이어가 존재하지 않은 경우
        if (target1 == null && target2 == null)
        {
            focusTargetSetting = false;
            isLookingAtPlayer = false;
            return;
        }

        if (target1 != null && target2 != null)
        {
            // float dist1 = Vector2.Distance(this.transform.position, target1.position);
            // float dist2 = Vector2.Distance(this.transform.position, target2.position);


            if (aggroMeter1 > aggroMeter2)
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
            focusTarget = target1 == null ? target2 : target1;
        }

        isLookingAtPlayer = true;
        focusTargetSetting = true;
        agent.isStopped = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("TriggerObj"))
        {
            onTriggerCheckObj = other.gameObject;
            triggerCheck = onTriggerCheckObj.GetComponent<TriggerCheck>();
        }
    }
}
