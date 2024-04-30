using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : Enemy
{
    public override void InitSetting()
    {
        enemyData.name = "Orc";
        enemyData.hp = 100;
        enemyData.attackDamage = 10;
        enemyData.attackDelayTime = 2f;
    }
}
