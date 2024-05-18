using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Enemy
{
    public override void InitSetting()
    {
        enemyData.name = "Golem";
        enemyData.hp = 150;
        enemyData.maxHp = 4000;//enemyData.hp;
        enemyData.attackDamage = 40;
        enemyData.attackDelayTime = 2.5f;
        enemyData.moveSpeed = 2f;
        enemyData.evasionRate = 5;
        enemyData.enemyType = EnemyType.BOSS;
    }
}
