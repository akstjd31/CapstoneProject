using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Enemy
{
    public override void InitSetting()
    {
        enemyData.name = "Golem";
        enemyData.hp = 1000;
        enemyData.maxHp = enemyData.hp;
        enemyData.attackDamage = 20;
        enemyData.attackDelayTime = 1f;
        enemyData.moveSpeed = 2f;
        enemyData.evasionRate = 5;
        enemyData.enemyType = EnemyType.BOSS;
    }
}
