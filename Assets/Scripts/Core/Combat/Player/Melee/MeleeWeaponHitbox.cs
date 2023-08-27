using Controllers;
using Controllers.Player;
using Core.Enemy;
using UI.Tokens;
using UnityEngine;

public class MeleeWeaponHitbox : MonoBehaviour
{
    private int minDamage;
    private int maxDamage;

    [HideInInspector]
    public bool allowAttack = false;

    private Character player;

    private void Start()
    {
        player = PlayerManager.Instance.player.GetComponent<Character>();
    }

    public void SetDamageHitbox(int _minDamage, int _maxDamage)
    {
        minDamage = _minDamage;
        maxDamage = _maxDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        float damageMod = TokenManager.instance.damageSection.GetModifier();
        int baseDamage = (int)Mathf.Round(minDamage * damageMod);
        int maxBaseDamage = (int)Mathf.Round(maxDamage * damageMod);

        if (other.CompareTag("Enemy") && allowAttack)
        {   
            allowAttack = false;

            EnemyController enemyController = other.GetComponent<EnemyController>();

            other.GetComponent<EnemyBase>().TakeDamage(baseDamage, maxBaseDamage, 0);

            if (player.attackCount >= 2 && !other.GetComponent<EnemyController>().isKnockbackInProgress)
            {
                enemyController.StartCoroutine(enemyController.PerformKnockback());
            }
        }

        if (other.CompareTag("Boss") && allowAttack)
        {
            allowAttack = false;
            
            other.GetComponent<Boss>().TakeDamage(baseDamage, maxBaseDamage, 0);
        }
    }
}
