using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_Skeleton : Enemy
{
    public override void InitSetting()
    {
        enemyData.name = "Skeleton";
        enemyData.hp = 90;
        enemyData.attackDamage = 25;
        enemyData.attackDelayTime = 1.5f;
        enemyData.moveSpeed = 3.5f;
        enemyData.evasionRate = 0;
        enemyData.enemyType = EnemyType.LONG_DISTANCE;
    }
}
