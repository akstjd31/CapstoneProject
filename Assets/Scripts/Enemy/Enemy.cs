using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public struct EnemyData
{
    public string name;
    public float maxHp;
    public float hp;
    public float attackDamage;
    public float attackDelayTime;
    public float moveSpeed;
    public float evasionRate; // 회피율
    public EnemyType enemyType;
}
public enum EnemyType
{
    // 근거리, 원거리, 보스
    SHORT_DISTANCE,
    LONG_DISTANCE,
    BOSS
}

public abstract class Enemy : MonoBehaviour
{
    public EnemyData enemyData;

    public abstract void InitSetting();
}
