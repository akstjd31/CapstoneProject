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

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger");
        if (other.tag == "Player")
        {
            Debug.Log("Player");
            other.transform.Translate(dir * 5.0f);
            Debug.Log("Translate");
        }
    }
}
