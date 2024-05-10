using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed = 6f;
    private Vector3 targetPos;

    private int damage;

    private Rigidbody2D rigid;

    public void SetDamage(int damage)
    {
        this.damage = damage;
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
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerCtrl player = other.GetComponent<PlayerCtrl>();

            if (player != null && !player.onHit && rigid.velocity != Vector2.zero)
            {
                player.GetComponent<PhotonView>().RPC("DamageEnemyOnHitRPC", RpcTarget.All, damage, targetPos - this.transform.position);
            }

            Destroy(this.gameObject);
        }

        else if (other.CompareTag("Enemy") && !owner.Equals(other.tag))
        {
            EnemyCtrl enemy = other.GetComponent<EnemyCtrl>();

            if (enemy != null && !enemy.onHit && rigid.velocity != Vector2.zero)
            {
                enemy.GetComponent<PhotonView>().RPC("DamagePlayerOnHitRPC", RpcTarget.All, playerViewID, targetPos - this.transform.position);
            }

            Destroy(this.gameObject);
        }
    }
}
