using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLazer : MonoBehaviour
{
    [SerializeField] private float warningTime = 4f;
    private Transform target;
    private Vector3 lastTargetPos;
    private Vector3 targetVelocity = Vector3.zero;
    private float lerpTime = 0.5f;
    private float startDelay = 1f;

    public Transform specialLazerPrefab;

    public AudioSource audioSource;
    public AudioClip lazerBuildSound;
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void Start()
    {
        audioSource = this.GetComponent<AudioSource>();

        audioSource.PlayOneShot(lazerBuildSound);
    }

    private void Update()
    {
        if (warningTime >= 0.0f)
        {
            warningTime -= Time.deltaTime;

            if (target != null)
            {
                MoveTowardsTarget();
            }
        }
        else
        {
            Instantiate(specialLazerPrefab, lastTargetPos, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

    private void MoveTowardsTarget()
    {
        
        //Vector2 shootDir = (target.position - this.transform.position).normalized;

        // 부드럽게 이동
        transform.position = Vector3.SmoothDamp(transform.position, target.position, ref targetVelocity, lerpTime);
        lastTargetPos = transform.position;
        //rigid.velocity = shootDir * speed;
    }
}
