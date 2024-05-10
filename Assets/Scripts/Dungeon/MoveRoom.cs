using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MoveRoom : MonoBehaviour
{
    Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
        dir = this.transform.localRotation * Vector3.up;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            other.transform.Translate(dir * 5.0f);
        }
    }
}