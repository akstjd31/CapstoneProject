using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemySO", menuName = "ScriptableObject/EnemySO")]
public class EnemySO : ScriptableObject
{
    public Enemy[] enemyList;
}
