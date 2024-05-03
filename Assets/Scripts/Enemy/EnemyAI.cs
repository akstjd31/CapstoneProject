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

    [SerializeField] private Transform target1, target2; // �÷��̾�

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

        // �÷��̾ ������ ȥ�� ������ ��� ���
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
            // Ư�� Ÿ������ ������ ����� Ư�� ����(���� ����)�� null ���°� �Ǿ��� ��
            if (agent.destination == null)
            {
                focusTarget = GameObject.FindGameObjectWithTag("Player").transform;
            }

            // Ÿ��1�� ��Ŀ�� ������ ��
            if (focusTarget == target1)
            {
                agent.SetDestination(target1.position);
                FlipHorizontalRelativeToTarget(target1.position);
            }
            // Ÿ��2�� ��Ŀ�� ������ ��
            else
            {
                agent.SetDestination(target2.position);
                FlipHorizontalRelativeToTarget(target2.position);
            }

            // ���� �Ÿ��� �������� ����
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

    // ���� Ÿ���� ���� ��
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

    // ���� �÷��̾ ������ �� �ִ� ��ġ�ΰ�?
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

    // �÷��̾ �߰�
    IEnumerator GetTimeToFacePlayer()
    {
        yield return new WaitForSeconds(waitForSec);
        FollowClosestPlayer();

        spriteRenderer.color = Color.red;
    }

    // Ÿ�� ��ġ�� ���� �¿� ����
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

    // ó�� ������ ������ �� �� �÷��̾��� �Ÿ��� ���Ͽ� ��Ŀ�� ��.
    private void FollowClosestPlayer()
    {
        if (target1 != null && target2 != null)
        {
            float dist1 = Vector2.Distance(this.transform.position, target1.position);
            float dist2 = Vector2.Distance(this.transform.position, target2.position);

            // dist���� ũ�� = �Ÿ��� �ִ�
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
