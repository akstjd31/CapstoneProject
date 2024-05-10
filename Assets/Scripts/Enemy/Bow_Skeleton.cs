using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow_Skeleton : Enemy
{
    public override void InitSetting()
    {
        enemyData.name = "Skeleton";
        enemyData.hp = 80;
        enemyData.attackDamage = 7;
        enemyData.attackDelayTime = 1.5f;
        enemyData.enemyType = EnemyType.LONG_DISTANCE;
    }
}
