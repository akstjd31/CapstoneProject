using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Laser : MonoBehaviour
{
    [SerializeField] private int ownerViewID;
    [SerializeField] private float damage;
    [SerializeField] private bool isTrigger = false;
    [SerializeField] private Transform startPos;
    private GameObject triggerObj = null;

    private Animator anim;

    public LayerMask playerMask;
    public Transform laserPoint;
    public Vector2 rectangleSize = new Vector2(10.5f, 1.0f); // 직사각형의 가로와 세로 길이

    private void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    private void Update()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 1.0f)
        {
            Destroy(this.gameObject);
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

    public void SetStartPos(Transform startPos)
    {
        this.startPos = startPos;
    }

    // 레이저 발사 시 호출되는 이벤트 함수
    private void CheckPlayerLaserHit()
    {
        Collider2D players = Physics2D.OverlapBox(laserPoint.position, rectangleSize, playerMask);

        if (players != null)
        {
            PlayerCtrl playerCtrl = players.GetComponent<PlayerCtrl>();

            if (playerCtrl != null && !playerCtrl.onHit)
            {
                PhotonView playerPV = playerCtrl.GetComponent<PhotonView>();

                playerPV.RPC("DamageEnemyOnHitRPC", RpcTarget.All, damage);
                playerPV.RPC("PlayerknockbackRPC", RpcTarget.All, ownerViewID, startPos.position - players.transform.position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (laserPoint != null)
        {
            Gizmos.color = Color.red; // 기즈모의 색상 설정
            // 직사각형 중심 위치
            Vector3 center = laserPoint.position;
            // 직사각형의 크기 설정
            Vector3 size = new Vector3(rectangleSize.x, rectangleSize.y, 0.0f);
            // 직사각형 그리기
            Gizmos.DrawWireCube(center, size);
        }
    }
}
