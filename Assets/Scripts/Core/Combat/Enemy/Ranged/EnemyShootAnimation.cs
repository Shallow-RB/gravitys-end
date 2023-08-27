using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootAnimation : MonoBehaviour
{
    public EnemyRangeWeapon enemyRangeWeapon; 

    public void AllowShot()
    {
        enemyRangeWeapon.allowShot = true;
    }

    public void DisAllowShot()
    {
        enemyRangeWeapon.allowShot = false;
    }
}
