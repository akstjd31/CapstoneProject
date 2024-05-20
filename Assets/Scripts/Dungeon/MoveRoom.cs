using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveRoom : MonoBehaviour
{
    Vector3 dir;
    int playerCount = 0;

    public AudioSource audioSource;
    public AudioClip openSound;
    float timer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        dir = this.transform.localRotation * Vector3.up;
        audioSource = this.GetComponent<AudioSource>();
    }

    void Update()
    {
        timer += Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerCount++;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if(playerCount == 2 && timer > 1.0f)
        {
            other.transform.Translate(dir * 6.0f);
            audioSource.PlayOneShot(openSound);
            timer = 0.0f;
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerCount--;
        }
    }
}