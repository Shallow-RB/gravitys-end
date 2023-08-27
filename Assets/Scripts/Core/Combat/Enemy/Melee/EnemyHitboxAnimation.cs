using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitboxAnimation : MonoBehaviour
{
    [SerializeField]
    private GameObject weapon;
    [SerializeField]
    private GameObject meleeHitbox;

    public void EnableHitbox()
    {
        EnemyMeleeWeapon meleeWeapon = weapon.GetComponent<EnemyMeleeWeapon>();
        EnemyMeleeWeaponHitbox meleeWeaponHitbox = meleeHitbox.GetComponent<EnemyMeleeWeaponHitbox>();

        meleeHitbox.SetActive(true);

        meleeWeaponHitbox.allowAttack = true;
        meleeWeaponHitbox.SetDamageHitbox(meleeWeapon.GetMinDamage(), meleeWeapon.GetMaxDamage());
    }

    public void DisableHitbox()
    {
        EnemyMeleeWeapon meleeWeapon = weapon.GetComponent<EnemyMeleeWeapon>();
        EnemyMeleeWeaponHitbox meleeWeaponHitbox = meleeHitbox.GetComponent<EnemyMeleeWeaponHitbox>();

        meleeWeaponHitbox.allowAttack = false;
        meleeWeaponHitbox.SetDamageHitbox(0, 0);

        meleeHitbox.SetActive(false);
    }
}
