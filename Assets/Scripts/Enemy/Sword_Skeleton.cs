using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Skeleton : Enemy
{
    public override void InitSetting()
    {
        enemyData.name = "Skeleton";
        enemyData.hp = 120;
        enemyData.attackDamage = 10;
        enemyData.attackDelayTime = 1f;
        enemyData.enemyType = EnemyType.SHORT_DISTANCE;
    }
}
