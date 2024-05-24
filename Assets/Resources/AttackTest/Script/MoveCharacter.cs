using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MoveCharacter : MonoBehaviour
{
    public KeyCode moveUp = KeyCode.UpArrow;
    public KeyCode moveDown = KeyCode.DownArrow;
    public KeyCode moveLeft = KeyCode.LeftArrow;
    public KeyCode moveRight = KeyCode.RightArrow;
    public float speed = 1;
    private Rigidbody2D rigidbody2d;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();    
    }

    // Update is called once per frame
    void Update()
    {
        float moveX = 0;
        float moveY = 0;
        if(Input.GetKey(moveUp)) moveY += speed;
        if(Input.GetKey(moveDown)) moveY -= speed;
        if(Input.GetKey(moveLeft)) moveX -= speed;
        if(Input.GetKey(moveRight)) moveX += speed;
        Vector2 moveVector = new Vector2(moveX, moveY);
        rigidbody2d.AddForce(moveVector);
    }
}
