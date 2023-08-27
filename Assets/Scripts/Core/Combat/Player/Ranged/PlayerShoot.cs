using System;
using System.Collections;
using System.Collections.Generic;
using Controllers.Player;
using UI.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public static Action shootInput;

    [HideInInspector]
    public RangeWeapon weapon;

    private void Start()
    {
        weapon = gameObject.GetComponent<RangeWeapon>();
    }

    public void OnWeaponEquipped()
    {
        shootInput += weapon.Shoot;
    }

    public void OnWeaponUnequipped()
    {
        if (shootInput == null){
            return;
        }
        shootInput -= weapon.Shoot;
    }

    public void Shoot()
    {
        if (EquipmentSystem.Instance._equippedWeapon == gameObject && weapon.currentAmmo > 0)
        {
            shootInput?.Invoke();
        }
    }
}
