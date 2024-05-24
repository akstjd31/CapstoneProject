using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SkillArrow : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Vector3 targetPos;

    [SerializeField] private int viewID = -1;

    [SerializeField] private float damage;
    private Rigidbody2D rigid;


    [PunRPC]
    public void InitializeArrow(Vector3 dir, float spd, float dam, int viewID)
    {
        targetPos = dir;
        speed = spd;
        damage = dam;
        this.viewID = viewID;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
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

                if (enemyCtrl != null && !enemyCtrl.onHit && rigid.velocity != Vector2.zero)
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
            Destroy(this.gameObject);
        }

        else if(other.CompareTag("Chest"))
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
