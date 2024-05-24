using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl4Test : MonoBehaviour
{
    public float movePower = 5f; // 이동에 필요한 힘
    Vector3 moveDir, rollDir, attackDir; // 이동 방향, 구르기 방향, 공격 방향
    Animator anim; // 플레이어 애니메이터
    Rigidbody2D rigid; // 플레이어 리지드 바디

    // Start is called before the first frame update
    void Start()
    {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    
    void Move()
    {
        if (moveDir.x != 0 || moveDir.y != 0 || anim.GetBool("AttackIdle"))
        {
            anim.SetBool("AttackIdle", false);
            anim.SetBool("isMove", true);
        }
        else
        {
            anim.SetBool("isMove", false);
        }

        // 플레이어 좌, 우 스케일 값 변경 (뒤집기)
        if (moveDir.x > 0.0f)
        {
            this.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (moveDir.x < 0.0f)
        {
            this.transform.localScale = new Vector3(-1, 1, 1);
        }

        rigid.velocity = moveDir * movePower;
    }

}
