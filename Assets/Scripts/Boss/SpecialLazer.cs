using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialLazer : MonoBehaviour
{
    private void Start()
    {
        Destroy(this.gameObject, this.GetComponent<ParticleSystem>().main.duration);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Status status = other.GetComponent<Status>();

            status.HP = 0;
        }
    }
}
