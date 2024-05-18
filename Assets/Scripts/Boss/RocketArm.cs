using UnityEngine;
using Photon.Pun;

public class RocketArm : MonoBehaviour
{
    [SerializeField] private Transform startPos, target;
    [SerializeField] private float speed = 5f;
    private int ownerViewID;
    private float damage;

    private Rigidbody2D rigid;

    private void Start()
    {
        rigid = this.GetComponent<Rigidbody2D>();

        // 초기 위치를 startPos로 설정
        transform.position = startPos.position;

        Destroy(this.gameObject, 5f);
    }

    private void Update()
    {
        if (target != null)
        {
            MoveTowardsTarget();
            FlipHorizontalRelativeToTarget();
        }
    }

    // 적의 위치에 따른 스케일 뒤집기
    private void FlipHorizontalRelativeToTarget()
    {
        if (target.position.x - this.transform.position.x > 0)
        {
            this.transform.localScale = new Vector3(7, 7, 7);
        }
        else
        {
            this.transform.localScale = new Vector3(-7, 7, 7);
        }
    }

    public void SetViewID(int viewID)
    {
        this.ownerViewID = viewID;
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    public void SetTargetPos(Transform target)
    {
        this.target = target;
    }

    private void MoveTowardsTarget()
    {
        Vector2 shootDir = (target.position - this.transform.position).normalized;

        rigid.velocity = shootDir * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 플레이어와 접촉했을 때
        if (other.CompareTag("Player"))
        {
            PlayerCtrl playerCtrl = other.GetComponent<PlayerCtrl>();

            if (playerCtrl != null && !playerCtrl.onHit)
            {
                PhotonView playerPV = playerCtrl.GetComponent<PhotonView>();

                playerPV.RPC("DamageEnemyOnHitRPC", RpcTarget.All, damage);
                playerPV.RPC("PlayerKnockbackRPC", RpcTarget.All, ownerViewID, target.position - startPos.position);

                Destroy(this.gameObject);
            }
        }
    }
}