using Controllers.Player;
using TMPro;
using UnityEngine;

namespace UI.Runtime
{
    public class Magazine : MonoBehaviour
    {
        [SerializeField]
        private GameObject container;

        [SerializeField]
        private TextMeshProUGUI currentAmmo;

        [SerializeField]
        private TextMeshProUGUI reserveAmmo;

        private EquipmentSystem _equippedSystem;

        private void Update()
        {
            if (_equippedSystem == null)
                _equippedSystem = EquipmentSystem.Instance;
            else if (_equippedSystem._equippedWeapon != null && _equippedSystem._equippedWeapon.CompareTag("Ranged"))
            {
                if (!container.activeSelf)
                    container.SetActive(true);

                var rangeWeapon = _equippedSystem._equippedWeapon.GetComponent<RangeWeapon>();
                currentAmmo.text = rangeWeapon.currentAmmo.ToString();
                // reserveAmmo.text = rangeWeapon.reserveAmmo.ToString();
            }
            else if (container.activeSelf)
                container.SetActive(false);
        }
    }
}