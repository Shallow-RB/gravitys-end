using System.Collections;
using System.Collections.Generic;
using Controllers.Player;
using UnityEngine;

public class PlayerShootAnimation : MonoBehaviour
{
    private EquipmentSystem equipmentSystem;

    private void Start()
    {
        equipmentSystem = EquipmentSystem.Instance;
    }

    public void PerformShot()
    {
        if (equipmentSystem._equippedWeapon.CompareTag("Ranged"))
        {
            equipmentSystem._equippedWeapon.GetComponent<PlayerShoot>().Shoot();
        }
    }
}
