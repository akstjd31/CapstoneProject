using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObject/Weapon")]

public class Weapon : ScriptableObject
{
    public string name; // 이름
    public int damage; // 무기 공격력
    public float speed; // 공격 속도
    public enum WeaponType
    {
        // 근거리, 원거리
        SHORT_DISTANCE,
        LONG_DISTANCE
    }
    
    public WeaponType weaponType; // 속성(?)

    public GameObject Prefab; // 해당 프리팹
}
