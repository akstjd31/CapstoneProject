using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slug : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    private float moveIntervalTime = 0.0f;
    private float jumpPower = 5f;

    Animator anim;
    Rigidbody2D rigid;
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        rigid = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveIntervalTime += Time.deltaTime;

        if (moveIntervalTime >= 1.5f)
        {
            anim.SetTrigger("Moving");
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            moveIntervalTime = 0.0f;
        }
    }

    void Move()
    {

    }

    // IEnumerator Move()
    // {
    //     yield return new WaitForSeconds(1.5f);
    //     anim.SetTrigger("Moving");
    // }
}
