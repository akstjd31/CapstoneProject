using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class PassiveSkill : Skill
{
    Task<List<int>> skillLevelAllTask;
    List<int> skillLevelAll;
    public int attackCount = 0;

    public float[] pridePercentage = new float[11] {0.0f, 3.0f, 5.0f, 7.0f, 10.0f, 12.5f, 15.0f, 18.0f, 20.0f, 23.0f, 30.0f}; //교만 공격력%
    public float[] greedPercentage = new float[11] {0.0f, 2.0f, 4.0f, 6.0f, 8.0f, 10.0f, 12.0f, 14.0f, 16.0f, 18.0f, 25.0f}; //탐욕 골드획득량%
    public float[,] lustPercentage = new float[11, 2] {{0.0f, 0.0f}, {3.0f, 3.0f}, {5.0f, 5.0f}, {7.0f, 5.0f}, {10.0f, 7.0f}, {12.5f, 7.0f}, //색욕 쿨타임, 공격력% 
    {15.0f, 10.0f}, {18.0f, 10.0f}, {20.0f, 12.5f}, {23.0f, 12.5f}, {30.0f, 15.0f}};
    public float[] envyPercentage = new float[11] {0.0f, 5.0f, 10.0f, 15.0f, 20.0f, 25.0f, 30.0f, 35.0f, 40.0f, 50.0f, 60.0f}; //질투 회피율%
    public float[] glutnyPercentage = new float[11] {0.0f, 2.0f, 4.0f, 5.0f, 7.0f, 8.0f, 9.0f, 10.0f, 12.0f, 13.0f, 15.0f}; //식욕 현재 체력의 피해량 흡혈%
    public float[] wrathPercentage = new float[11] {0.0f, 0.1f, 0.5f, 1.0f, 1.5f, 1.7f, 2.0f, 2.5f, 3.0f, 3.5f, 5.0f}; //분노 초
    public float[] slothPercentage = new float[11] {150.0f, 180.0f, 200.0f, 250.0f, 220.0f, 250.0f, 275.0f, 300.0f, 330.0f, 370.0f, 400.0f}; //나태 %
    // Start is called before the first frame update
    protected void Start()
    {
        skillLevelAllTask = CharSkill.GetSkillLevelAll(0);
        skillLevelAll = skillLevelAllTask.Result;
    }

    void Update()
    {

    }

    public float PrideAttack(EnemyCtrl enemyCtrl, float damage)
    {
        if(skillLevelAll[0] > 0)
        {
            if(enemyCtrl.enemy.enemyData.hp / enemyCtrl.enemy.enemyData.maxHp < 0.3f)
            {
                damage *= 1 + (pridePercentage[skillLevelAll[0]] / 100);
                return damage;
            }
            else
            {
                return damage;
            }
        }
        else
        {
            return damage;
        }
    }

    public float PrideDamaged(float damage)
    {
        if(skillLevelAll[0] > 0)
        {
            return damage * 1.2f;
        }
        else
        {
            return damage;
        }
    }

    public void Greed()
    {
        if(skillLevelAll[1] > 0)
        {
            
        }
    }

    public void Lust()
    {
        if(skillLevelAll[2] > 0)
        {
            float buffedDamage = status.attackDamage * lustPercentage[skillLevelAll[2],1] / 100;
            attackCount++;
            if(attackCount > 5)
            {
                status.attackDamage += buffedDamage;
            }
            else
            {
                status.attackDamage -= buffedDamage;
            }
        }
    } 

    public void Envy()
    {
        if(skillLevelAll[3] > 0)
        {
            status.evasionRate *= 1 + envyPercentage[skillLevelAll[3]] / 100;
        }
    }
    public void Glutny()
    {
        if(skillLevelAll[4] > 0)
        {
            
        }
    }
    public void Wrath()
    {
        if(skillLevelAll[5] > 0)
        {
            
        }
    }
    public void Sloth()
    {
        if(skillLevelAll[6] > 0)
        {
            
        }
    }
}
