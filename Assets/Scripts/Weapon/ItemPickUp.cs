using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    [SerializeField] private Weapon weapon;

    public int GetDamage()
    {
        return weapon.damage;
    }

    public float GetSpeed()
    {
        return weapon.speed;
    }

    
}
