using System.Collections;
using System.Collections.Generic;
using Controllers;
using Controllers.Player;
using Core.Enemy;
using UnityEngine;


public class BannermanController : MonoBehaviour
{
    [SerializeField]
    [Header("Enemy Settings")]
    private float healEnemyAmountMinimum = 65f;
    [SerializeField]
    private float healEnemyAmountMaximum = 85f;
    [SerializeField]
    [Header("Player Settings")]
    private float healPlayerAmountMinimum = 40f;
    [SerializeField]
    private float healPlayerAmountMaximum = 60f;
    [Header("Healing cooldown")]
    [SerializeField]
    private float healCooldown = 4f;
    [HideInInspector]
    public bool healingAllowed;

    private float coolDownTimer = 0f;

    private List<EnemyBase> enemies = new();
    PlayerStatsController playerStats;

    private void Update()
    {
        if (healingAllowed)
        {
            if (coolDownTimer < healCooldown)
                coolDownTimer += Time.deltaTime;
            else
            {
                coolDownTimer = 0f;
                if (playerStats != null)
                    HealPlayer();
                foreach (EnemyBase enemyBase in enemies)
                    if(enemyBase != null)
                        HealEnemy(enemyBase);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && healingAllowed)
        {
            EnemyBase _enemyBase = other.GetComponent<EnemyBase>();
            if (_enemyBase != null && !enemies.Contains(_enemyBase))
                enemies.Add(_enemyBase);
        }
        else if (other.gameObject.CompareTag("Player") && healingAllowed)
        {
            PlayerStatsController _playerStats = other.GetComponent<PlayerStatsController>();

            if (_playerStats != null && playerStats == null)
                playerStats = _playerStats;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy") && healingAllowed)
        {
            EnemyBase enemyBase = other.GetComponent<EnemyBase>();

            if (enemyBase != null && enemies.Contains(enemyBase))
                    enemies.Remove(enemyBase);
        }

        else if (other.gameObject.CompareTag("Player") && healingAllowed)
        {
            PlayerStatsController _playerStats = other.GetComponent<PlayerStatsController>();

            if (_playerStats != null && playerStats != null)
                playerStats = null;
        }
    }

    private void HealEnemy(EnemyBase enemyBase)
    {
        float healEnemyAmount = Random.Range(healEnemyAmountMinimum, healEnemyAmountMaximum);
        enemyBase.EnemyHeal(healEnemyAmount);
    }

    private void HealPlayer()
    {
        float healPlayerAmount = Random.Range(healPlayerAmountMinimum, healPlayerAmountMaximum);
        playerStats.HealPlayer(healPlayerAmount);
    }
}
