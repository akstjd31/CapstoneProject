using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class EnemyCtrl : MonoBehaviour
{
    private enum State 
	{
		NORMAL, MOVE, ATTACK, ATTACKIDLE
	}
    
    [SerializeField] private State state;
    
    Transform target1, target2;
    NavMeshAgent agent;
    Animator anim;
    PhotonView pv, hitPlayerPV;

    [SerializeField] private EnemySO enemySO;
    [SerializeField] private float attackWaitTime;

    bool playerHit = false;
    bool focusTheFirstPlayer = true;

    private GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        pv = this.GetComponent<PhotonView>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        state = State.NORMAL;

        anim = GetComponent<Animator>();

        attackWaitTime = AttackDelayTime();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        hitPlayerPV = null;
        
    }

    void FixedUpdate()
    {   

        if (target1 == null && gameManager.localPlayer != null)
        {
            target1 = gameManager.localPlayer.transform;
        }
        else if (target2 == null & gameManager.remotePlayer != null)
        {
            target2 = gameManager.remotePlayer.transform;
        }

        if (target1 != null)
        {
            if (target2 == null)
            {
                float dist = Vector2.Distance(target1.position, this.transform.position);

                //pv.RPC("StateAccordingToDist", RpcTarget.All, dist);
                StateAccordingToDist(dist);
            }
            else
            {
                float dist1 = Vector2.Distance(target1.position, this.transform.position);
                float dist2 = Vector2.Distance(target2.position, this.transform.position);
            
                if (dist1 < dist2)
                {
                    focusTheFirstPlayer = true;
                    //pv.RPC("StateAccordingToDist", RpcTarget.All, dist1);
                    StateAccordingToDist(dist1);
                }
                else
                {
                    focusTheFirstPlayer = false;
                    //pv.RPC("StateAccordingToDist", RpcTarget.All, dist2);
                    StateAccordingToDist(dist2);
                }
            }
        }
    }

    //[PunRPC]
    private void StateAccordingToDist(float dist)
    {
        if (dist > 5f)
        {
            agent.isStopped = true;
            state = State.NORMAL;
        }
        else if (dist > 1.6f)
        {
            agent.isStopped = false;
            state = State.MOVE;
        }
        else
        {
            agent.isStopped = true;

            if (attackWaitTime < AttackDelayTime())
            {
                state = State.ATTACKIDLE;
                attackWaitTime += Time.deltaTime;
            }
            else
            {
                if (hitPlayerPV)
                {
                    PlayerCtrl playerctrl = hitPlayerPV.GetComponent<PlayerCtrl>();
                    if (playerctrl.GetState() != PlayerCtrl.State.DIE)
                    {
                        state = State.ATTACK;
                        hitPlayerPV.GetComponent<Status>().HP -= AttackDamage();
                    
                        playerctrl.ChangeState(PlayerCtrl.State.HIT);
                    }
                    else
                    {
                        state = State.NORMAL;
                        agent.isStopped = false;
                    }
                }

                attackWaitTime = 0.0f;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target1 != null || target2 != null)
        {
            switch (state)
            {
                case State.NORMAL:
                    anim.SetBool("AttackIdle", false);
                    anim.SetBool("isMove", false);
                    break;
                case State.MOVE:
                    anim.SetBool("isAttack", false);
                    anim.SetBool("AttackIdle", false);
                    anim.SetBool("isMove", true);
                    break;
                case State.ATTACK:
                    anim.SetBool("isMove", false);
                    anim.SetBool("AttackIdle", false);
                    anim.SetBool("isAttack", true);
                    break;
                case State.ATTACKIDLE:
                    anim.SetBool("isAttack", false);
                    anim.SetBool("AttackIdle", true);
                    break;
            }
        }
    }

    void LateUpdate()
    {
        if (target1 != null || target2 != null)
        {
            switch (state)
            {
                case State.NORMAL:
                    break;
                case State.MOVE:
                    if (focusTheFirstPlayer)
                        agent.SetDestination(target1.position);
                        //pv.RPC("RPC_SetDestination", RpcTarget.All, target1.position);
                    else
                        agent.SetDestination(target2.position);
                        //pv.RPC("RPC_SetDestination", RpcTarget.All, target2.position);

                    EnemyDirection();
                    break;
                case State.ATTACK:
                    break;
                case State.ATTACKIDLE:
                    break;
            }
        }
    }

    [PunRPC]
    private void RPC_SetDestination(Vector3 pos)
    {
        agent.SetDestination(pos);
    }

    private float AttackDelayTime()
    {
        foreach(Enemy enemy in enemySO.enemyList)
        {
            string cloneName = this.name.Substring(0, this.name.Length - 7);
            if (cloneName.Equals(enemy.name))
            {
                return enemy.attackDelayTime;
            }
        }
        return -1;
    }

    private int AttackDamage()
    {
        foreach(Enemy enemy in enemySO.enemyList)
        {
            string cloneName = this.name.Substring(0, this.name.Length - 7);
            if (cloneName.Equals(enemy.name))
            {
                return enemy.attackDamage;
            }
        }
        return -1;
    }

    private void EnemyDirection()
    {
        float playerXPos = (focusTheFirstPlayer == true) ? target1.position.x : target2.position.x;

        if (playerXPos - this.transform.position.x >= 0.0f)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hitPlayerPV = other.GetComponent<PhotonView>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            hitPlayerPV = null;
        }
    }
}
