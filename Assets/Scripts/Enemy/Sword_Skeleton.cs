using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword_Skeleton : Enemy
{
    public override void InitSetting()
    {
        enemyData.name = "Skeleton";
        enemyData.hp = 100;
        enemyData.attackDamage = 10;
        enemyData.attackDelayTime = 1f;
        enemyData.moveSpeed = 5;
        enemyData.evasionRate = 0;
        enemyData.enemyType = EnemyType.SHORT_DISTANCE;
    }
}
