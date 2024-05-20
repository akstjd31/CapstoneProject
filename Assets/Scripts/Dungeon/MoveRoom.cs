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
    // Start is called before the first frame update
    void Start()
    {
        dir = this.transform.localRotation * Vector3.up;
        audioSource = this.GetComponent<AudioSource>();
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
        if(playerCount == 2)
        {
            other.transform.Translate(dir * 6.0f);
            audioSource.PlayOneShot(openSound);
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