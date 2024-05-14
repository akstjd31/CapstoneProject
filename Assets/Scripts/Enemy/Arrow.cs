using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 targetPos;
    [SerializeField] string owner = "";  // 이 화살을 쏜 사람?

    [SerializeField] private int playerViewID = -1;

    [SerializeField] private float damage;

    private Rigidbody2D rigid;

    //private float elapsedTime = 2.0f;

    [PunRPC]
    public void InitializeArrow(Vector3 dir, float spd, string tag, int viewID)
    {
        targetPos = dir;
        speed = spd;
        owner = tag;
        playerViewID = viewID;
    }

    public void SetViewID(int viewID)
    {
        this.playerViewID = viewID;
    }

    public void SetOwner(string owner)
    {
        this.owner = owner;
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void SetTarget(Vector3 targetPos)
    {
        this.targetPos = targetPos;
    }
    
    public Vector3 GetTarget()
    {
        return targetPos;
    }

    private void Start()
    {
        rigid = this.GetComponent<Rigidbody2D>();

        Destroy(this.gameObject, 2f);
    }

    private void Update()
    {
        //elapsedTime -= Time.deltaTime;

        //if (elapsedTime <= 0.0f)
        //{
        //    PhotonNetwork.Destroy(this.gameObject);
        //}

        if (targetPos != null)
        {
            Vector2 shootDir;
            if (owner.Equals("Player"))
            {
                shootDir = (targetPos - this.transform.position);
            }
            else
            {
                shootDir = (targetPos - this.transform.position).normalized;
            }

            rigid.velocity = shootDir * speed;

            float angle = Mathf.Atan2(rigid.velocity.y, rigid.velocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            if (owner.Equals("Enemy"))
            {
                if (rigid.velocity == Vector2.zero)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !owner.Equals(other.tag))
        {
            PlayerCtrl player = other.GetComponent<PlayerCtrl>();

            if (player != null && !player.onHit && rigid.velocity != Vector2.zero)
            {
                //player.GetComponent<PhotonView>().RPC("DamageEnemyOnHitRPC", RpcTarget.All, player.passiveSkill.PrideDamaged(enemy.enemyData.attackDamage));
                player.GetComponent<PhotonView>().RPC("PlayerKnockbackRPC", RpcTarget.All, targetPos - this.transform.position);
                player.GetComponent<PhotonView>().RPC("DamageEnemyOnHitRPC", RpcTarget.All, damage);
            }

            Destroy(this.gameObject);
        }

        else if (other.CompareTag("Enemy") && !owner.Equals(other.tag))
        {
            EnemyCtrl enemy = other.GetComponent<EnemyCtrl>();

            if (enemy != null && !enemy.onHit && rigid.velocity != Vector2.zero)
            {
                //enemy.GetComponent<PhotonView>().RPC("DamagePlayerOnHitRPC", RpcTarget.All, playerViewID, passiveSkill.PrideAttack(enemyCtrl, status.attackDamage));
                enemy.GetComponent<PhotonView>().RPC("EnemyKnockbackRPC", RpcTarget.All, targetPos - this.transform.position);
                enemy.GetComponent<PhotonView>().RPC("DamagePlayerOnHitRPC", RpcTarget.All, playerViewID);
            }

            Destroy(this.gameObject);
        }

        else if (other.CompareTag("Obstacle"))
        {
            Destroy(this.gameObject);
        }
    }
}
