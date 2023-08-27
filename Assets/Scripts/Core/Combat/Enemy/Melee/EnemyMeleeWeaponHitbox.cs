using Controllers.Player;
using UI.Inventory;
using UI.Tokens;
using UnityEngine;

public class EnemyMeleeWeaponHitbox : MonoBehaviour
{
    private int minDamage;
    private int maxDamage;

    private GameObject player;

    [HideInInspector]
    public bool allowAttack = false;

    private void Start()
    {
        player = PlayerManager.Instance.player;
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

        if (other.CompareTag("Player") && allowAttack)
        {
            allowAttack = false;
            var armor = player.GetComponent<EquipmentSystem>()._equippedArmor;
            player.GetComponent<PlayerStatsController>().TakeDamage(minDamage, maxDamage, armor != null ? armor.GetComponent<Item>().GetArmorModifier() : 0);
        }
    }
}
