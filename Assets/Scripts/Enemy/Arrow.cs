using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Arrow : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 targetPos;
    [SerializeField] string owner = "";  // 이 화살을 쏜 사람?

    [SerializeField] private int viewID = -1;

    [SerializeField] private float damage;

    private Rigidbody2D rigid;

    //private float elapsedTime = 2.0f;

    [PunRPC]
    public void InitializeArrow(Vector3 dir, float spd, float dam, string tag, int viewID)
    {
        targetPos = dir;
        speed = spd;
        damage = dam;
        owner = tag;
        this.viewID = viewID;
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
                player.GetComponent<PhotonView>().RPC("PlayerKnockbackRPC", RpcTarget.All, viewID, targetPos - this.transform.position);
                player.GetComponent<PhotonView>().RPC("DamageEnemyOnHitRPC", RpcTarget.All, damage);

                player.GetComponent<PlayerSound>().PlayAttackedSound();
            }

            Destroy(this.gameObject);
        }

        else if (other.CompareTag("Enemy") && !owner.Equals(other.tag))
        {
            Enemy enemy = other.GetComponent<Enemy>();

            if (enemy.enemyData.enemyType == EnemyType.BOSS)
            {
                BossCtrl bossCtrl = other.GetComponent<BossCtrl>();

                if (bossCtrl != null &&
                    !bossCtrl.onHit &&
                    rigid.velocity != Vector2.zero)
                {
                    float rand = Random.Range(0, 100f);

                    if (rand > enemy.enemyData.evasionRate)
                    {
                        //enemy.GetComponent<PhotonView>().RPC("DamagePlayerOnHitRPC", RpcTarget.All, playerViewID, passiveSkill.PrideAttack(enemyCtrl, status.attackDamage));
                        bossCtrl.GetComponent<PhotonView>().RPC("BossKnockbackRPC", RpcTarget.All, targetPos - this.transform.position);
                        bossCtrl.GetComponent<PhotonView>().RPC("DamagePlayerOnHitRPC", RpcTarget.All, viewID);

                        bossCtrl.GetComponent<BossSound>().PlayAttackedSound();
                    }
                    else
                    {
                        Debug.Log("회피!");
                    }
                }
            }
            else
            {
                EnemyCtrl enemyCtrl = other.GetComponent<EnemyCtrl>();

                if (enemyCtrl != null && !enemyCtrl.onHit && rigid.velocity != Vector2.zero)
                {
                    //enemy.GetComponent<PhotonView>().RPC("DamagePlayerOnHitRPC", RpcTarget.All, playerViewID, passiveSkill.PrideAttack(enemyCtrl, status.attackDamage));
                    enemyCtrl.GetComponent<PhotonView>().RPC("EnemyKnockbackRPC", RpcTarget.All, targetPos - this.transform.position);
                    enemyCtrl.GetComponent<PhotonView>().RPC("DamagePlayerOnHitRPC", RpcTarget.All, viewID);

                    enemyCtrl.GetComponent<EnemySound>().PlayAttackedSound();
                }
            }

            Destroy(this.gameObject);
        }

        else if (other.CompareTag("Jewel"))
        {
            other.GetComponent<BejeweledPillar>().ChangeJewelColor();
            Destroy(this.gameObject);
        }

        else if(other.CompareTag("Chest") && !owner.Equals("Enemy"))
        {
            ChestController chestController = other.GetComponent<ChestController>();
            chestController.ChestBreak();
            Destroy(this.gameObject);
        }

        else if (other.CompareTag("Obstacle"))
        {
            Destroy(this.gameObject);
        }
    }
}
