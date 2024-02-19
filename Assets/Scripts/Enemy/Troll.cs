using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Troll : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    public float GetAttackDelaytime()
    {
        return enemy.attackDelayTime;
    }
}
