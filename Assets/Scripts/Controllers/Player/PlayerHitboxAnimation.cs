using System.Collections;
using System.Collections.Generic;
using Controllers.Player;
using UnityEngine;

public class PlayerHitboxAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject meleeHitbox;

    private EquipmentSystem equipmentSystem;
    private Character player;

    private void Start()
    {
        equipmentSystem = EquipmentSystem.Instance;
        player = PlayerManager.Instance.player.GetComponent<Character>();
    }

    public void EnableHitbox()
    {
        if (equipmentSystem._equippedWeapon.GetComponent<MeleeWeapon>() != null)
        {
            MeleeWeapon meleeWeapon = equipmentSystem._equippedWeapon.GetComponent<MeleeWeapon>();
            meleeHitbox.SetActive(true);
            MeleeWeaponHitbox meleeWeaponHitbox = meleeHitbox.GetComponent<MeleeWeaponHitbox>();

            meleeWeaponHitbox.allowAttack = true;
            meleeWeaponHitbox.SetDamageHitbox(meleeWeapon.GetMinDamage(), meleeWeapon.GetMaxDamage());

            if (player.attackCount >= 2)
            {
                player.attackCount = 0;
            }

            player.attackCount++;
        }
    }

    public void DisableHitbox()
    {
        if (equipmentSystem._equippedWeapon.GetComponent<MeleeWeapon>() != null)
        {
            MeleeWeapon meleeWeapon = equipmentSystem._equippedWeapon.GetComponent<MeleeWeapon>();
            MeleeWeaponHitbox meleeWeaponHitbox = meleeHitbox.GetComponent<MeleeWeaponHitbox>();

            meleeWeaponHitbox.allowAttack = false;
            meleeWeaponHitbox.SetDamageHitbox(0, 0);

            meleeHitbox.SetActive(false);
        }
    }
}
