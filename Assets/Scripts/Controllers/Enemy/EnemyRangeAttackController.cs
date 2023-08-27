using System.Collections;
using System.Collections.Generic;
using Controllers;
using Controllers.Player;
using Core.Enemy;
using UnityEngine;

public class EnemyRangeAttackController : MonoBehaviour
{
    public float attackRange;
    public GameObject rangeWeaponObject;

    [HideInInspector]
    public EnemyRangeWeapon rangeWeapon;

    [HideInInspector]
    public float playerDistance;

    private EnemyBase enemyBase;
    private EnemyController enemyController;
    private GameObject player;

    private void Start()
    {
        enemyBase = gameObject.GetComponent<EnemyBase>();
        enemyController = gameObject.GetComponent<EnemyController>();
        player = PlayerManager.Instance.player;
        rangeWeapon = rangeWeaponObject.GetComponent<EnemyRangeWeapon>();

        rangeWeapon.SetEnemy(transform);
    }
}
