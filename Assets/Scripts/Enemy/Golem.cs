using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Enemy
{
    public override void InitSetting()
    {
        enemyData.name = "Golem";
        enemyData.hp = 1000;
        enemyData.attackDamage = 28;
        enemyData.attackDelayTime = 1f;
        enemyData.moveSpeed = 2;
        enemyData.evasionRate = 0;
        enemyData.enemyType = EnemyType.BOSS;
    }
}
