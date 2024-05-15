using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningLazer : MonoBehaviour
{
    [SerializeField] private float speed = 4f;
    [SerializeField] private float warningTime = 4f;
    private Transform target;
    private Vector3 lastTargetPos;

    private Rigidbody2D rigid;

    public Transform specialLazerPrefab;
    private ParticleSystem particleSystem;
    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    private void Start()
    {
        rigid = this.GetComponent<Rigidbody2D>();
        particleSystem = specialLazerPrefab.gameObject.GetComponent<ParticleSystem>();
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
        lastTargetPos = target.position;
        Vector2 shootDir = (target.position - this.transform.position).normalized;

        rigid.velocity = shootDir * speed;
    }
}
