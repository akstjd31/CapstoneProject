using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "ScriptableObject/Enemy")]
public class Enemy : ScriptableObject
{
    public string name;
    public int attackDamage;
    public float attackDelayTime;

    public enum EnemyType
    {
        // 근거리, 원거리
        SHORT_DISTANCE,
        LONG_DISTANCE
    }

    public EnemyType enemyType;
    public GameObject prefab;
}
