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

    UIManager uiManager;

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
        uiManager = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIManager>();

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
                Instantiate(player.attackedEffect, new Vector2(0.293f, 0.637f), Quaternion.identity, player.transform);
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

                    if (rand > enemy.enemyData.evasionRate && bossCtrl.GetState() != BossCtrl.State.ATTACKED && bossCtrl.GetState() != BossCtrl.State.INVINCIBILITY)
                    {
                        bossCtrl.GetComponent<PhotonView>().RPC("BossKnockbackRPC", RpcTarget.All, targetPos - this.transform.position);
                        bossCtrl.GetComponent<PhotonView>().RPC("DamagePlayerOnHitRPC", RpcTarget.All, viewID, 1.0f);
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

                if (enemyCtrl != null && !enemyCtrl.onHit)
                {
                    enemyCtrl.GetComponent<PhotonView>().RPC("EnemyKnockbackRPC", RpcTarget.All, targetPos - this.transform.position);
                    enemyCtrl.GetComponent<PhotonView>().RPC("DamagePlayerOnHitRPC", RpcTarget.All, viewID, 1.0f);
                    enemyCtrl.GetComponent<EnemySound>().PlayAttackedSound();
                }
            }

            Destroy(this.gameObject);
        }

        else if (other.CompareTag("Jewel"))
        {
            other.GetComponent<BejeweledPillar>().ChangeJewelColor();
            uiManager.PlayJewelSound();
            Destroy(this.gameObject);
        }

        else if(other.CompareTag("Chest") && !owner.Equals("Enemy"))
        {
            ChestController chestController = other.GetComponent<ChestController>();

            uiManager.PlayChestSound();
            chestController.ChestBreak();
            Destroy(this.gameObject);
        }

        else if (other.CompareTag("Obstacle"))
        {
            uiManager.PlayArrowHitObstacleSound();
            Destroy(this.gameObject);
        }
    }
}
