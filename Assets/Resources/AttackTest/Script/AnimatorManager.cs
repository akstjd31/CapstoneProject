using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [RequireComponent(typeof(SpriteRenderer))]
 [RequireComponent(typeof(Rigidbody2D))]
 [RequireComponent(typeof(Animator))]
public class AnimatorManager : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2d;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if(rigidbody2d.velocity.x > 0.01f) spriteRenderer.flipX = false;
        if(rigidbody2d.velocity.x < -0.01f) spriteRenderer.flipX = true;
        
        if(rigidbody2d.velocity.sqrMagnitude > 0.01) animator.SetBool("isWalk", true);
        else animator.SetBool("isWalk", false);
    }
}
