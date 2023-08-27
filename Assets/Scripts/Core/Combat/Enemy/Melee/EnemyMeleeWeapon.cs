using System.Collections;
using System.Collections.Generic;
using Controllers.Player;
using Core.Enemy;
using UnityEngine;

public class EnemyMeleeWeapon : MonoBehaviour
{    
    [SerializeField]
    private int minDamage = 5;
    [SerializeField]
    private int maxDamage = 10;

    public int GetMinDamage()
    {
        return minDamage;
    }

    public int GetMaxDamage()
    {
        return maxDamage;
    }
}
