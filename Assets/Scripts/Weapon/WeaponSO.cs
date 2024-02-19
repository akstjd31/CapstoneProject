using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "ScriptableObject/WeaponSO")]
public class WeaponSO : ScriptableObject
{
    public Weapon[] weaponList;
}
