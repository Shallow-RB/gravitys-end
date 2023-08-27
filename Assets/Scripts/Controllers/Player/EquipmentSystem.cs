using System;
using UnityEngine;

namespace Controllers.Player
{
    public class EquipmentSystem : MonoBehaviour
    {
        public static EquipmentSystem Instance;

        [SerializeField]
        public GameObject _equippedWeapon;

        [SerializeField]
        public GameObject _equippedArmor;

        [SerializeField]
        private Transform meleeHolder;

        [SerializeField]
        private Transform rangedHolder;

        [SerializeField]
        private Transform armorHolder;

        private Quaternion oldRotation;
        private Character player;

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
        }
        private void Start()
        {
            player = PlayerManager.Instance.player.GetComponent<Character>();
        }

        public void SetCurrentWeapon(GameObject weapon)
        {
            _equippedWeapon = weapon;

            if (_equippedWeapon is null)
            {
                PlayerAnimator.Instance._animator.SetTrigger("unequip");
            }

            if (_equippedWeapon.CompareTag("Melee"))
            {
                var _meleeWeapon = _equippedWeapon.GetComponent<MeleeWeapon>();

                PlayerAnimator.Instance._animator.SetTrigger("meleeEquip");

                player.GetComponent<PlayerWeaponSlash>().SetSlashStyle(
                    _meleeWeapon.slashColor,
                    _meleeWeapon.slashTexture,
                    _meleeWeapon.smokeColor,
                    _meleeWeapon.sparkColor,
                    _meleeWeapon.hitColor,
                    _meleeWeapon.sparkCoreColor,
                    _meleeWeapon.fireColor
                );

                SetMeleeHolder();
            }

            if (_equippedWeapon.CompareTag("Ranged"))
            {
                PlayerAnimator.Instance._animator.SetTrigger("rangedEquip");
                SetRangedHolder();
            }
            return;
        }

        public void SetCurrentArmor(GameObject armor)
        {
            _equippedArmor = armor;

            if (_equippedArmor.CompareTag("Armor"))
            {
                SetArmorHolder();
            }
        }

        private void SetArmorHolder()
        {
            if (_equippedArmor is null) return;

            _equippedArmor.transform.SetParent(armorHolder);

            _equippedArmor.transform.localPosition = Vector3.zero;
            _equippedArmor.transform.localRotation = Quaternion.identity;
            _equippedArmor.transform.localScale = Vector3.one;
        }

        private void SetRangedHolder()
        {
            if (_equippedWeapon is null) return;

            _equippedWeapon.transform.SetParent(rangedHolder);

            _equippedWeapon.transform.localPosition = Vector3.zero;
            _equippedWeapon.transform.localRotation = Quaternion.identity;
            _equippedWeapon.transform.localScale = Vector3.one;

        }

        private void SetMeleeHolder()
        {
            if (_equippedWeapon is null) return;

            oldRotation = _equippedWeapon.transform.rotation;

            _equippedWeapon.transform.SetParent(meleeHolder);

            _equippedWeapon.transform.localPosition = Vector3.zero;
            _equippedWeapon.transform.localRotation = Quaternion.identity;
            _equippedWeapon.transform.localScale = Vector3.one;
        }

        public void DetachWeapon()
        {
            if (_equippedWeapon == null) return;

            _equippedWeapon.transform.rotation = oldRotation;
            PlayerAnimator.Instance._animator.SetTrigger("unequip");
            _equippedWeapon.transform.SetParent(null);
            _equippedWeapon = null;
        }

        public void DetachArmor()
        {
            if (_equippedArmor == null) return;

            _equippedArmor.transform.SetParent(null);
            _equippedArmor = null;
        }
    }
}
