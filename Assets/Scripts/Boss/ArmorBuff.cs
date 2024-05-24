using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorBuff : MonoBehaviour
{
    public GameObject warriorDebuff, archerDebuff;
    private Transform target;
    [SerializeField] private float durationTime;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetDurationTime(float time)
    {
        this.durationTime = time;
    }

    private void Update()
    {
        if (target != null)
        {
            if (durationTime >= 0.0f)
            {
                this.transform.position = target.position;
                durationTime -= Time.deltaTime;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }
}
