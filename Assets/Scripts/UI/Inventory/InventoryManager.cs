using System;
using System.Collections.Generic;
using System.Linq;
using Controllers.Player;
using TMPro;
using UI.Shop;
using UI.Tokens;
using UnityEngine;

namespace UI.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [Header("Equipped Slots")]
        [SerializeField]
        private InventorySlot equippedArmorSlot;

        [SerializeField]
        public InventorySlot equippedWeaponSlot;

        [Header("Inventory Slots")]
        [SerializeField]
        private List<InventorySlot> armorSlots;

        [SerializeField]
        private List<InventorySlot> weaponSlots;

        [Header("Settings")]
        [SerializeField]
        private TextMeshProUGUI damageReduction;

        [SerializeField]
        private TextMeshProUGUI damageRange;

        [SerializeField]
        private List<Item> weapons;

        public static InventoryManager instance { get; private set; }

        private void Awake()
        {
            // Singleton
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        private void Update()
        {
            RenderGearStats();
        }

        private void RenderGearStats()
        {
            if (equippedArmorSlot.IsEmpty())
                damageReduction.text = "0%";
            else
            {
                Item item = equippedArmorSlot.item.GetComponent<Item>();
                damageReduction.text = item.armorModifier.ToString() + "%";
            }

            if (equippedWeaponSlot.IsEmpty())
                damageRange.text = "0";
            else
            {
                Item item = equippedWeaponSlot.item.GetComponent<Item>();
                string start = string.Empty, end = string.Empty;
                if (equippedWeaponSlot.item.CompareTag("Melee"))
                {
                    start = item.GetComponent<MeleeWeapon>().GetMinDamage().ToString();
                    end = item.GetComponent<MeleeWeapon>().GetMaxDamage().ToString();
                }
                else if (equippedWeaponSlot.item.CompareTag("Ranged"))
                {
                    start = item.GetComponent<RangeWeapon>().minDamage.ToString();
                    end = item.GetComponent<RangeWeapon>().maxDamage.ToString();
                }
                damageRange.text = start + " - " + end;
            }
        }

        public bool PickupItem(Item item)
        {
            bool result;
            // Determine which list of slots and equipped slot corresponds to the item type
            switch (item.type)
            {
                // Call the Pickup method to add the item to the armor slots and equip it
                case ItemType.ARMOR:
                    result = Pickup(ref armorSlots, item);
                    break;
                // Call the Pickup method to add the item to the weapon slots and equip it
                case ItemType.WEAPON:
                    result = Pickup(ref weaponSlots, item);
                    break;
                // Throw an exception if the item type is not recognized
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return result;
        }

        public void UseItem(InventorySlot slot)
        {
            // Determine which list of slots and equipped slot corresponds to the item type of the clicked slot's item
            switch (slot.item.GetComponent<Item>().type)
            {
                // Call the Use method to swap the equipped slot item with the clicked slot item for armor items
                case ItemType.ARMOR:
                    Use(ref armorSlots, ref equippedArmorSlot, slot);
                    break;
                // Call the Use method to swap the equipped slot item with the clicked slot item for weapon items
                case ItemType.WEAPON:
                    Use(ref weaponSlots, ref equippedWeaponSlot, slot);
                    break;
                // Throw an exception if the item type is not recognized
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void Use(ref List<InventorySlot> slots, ref InventorySlot equippedSlot, InventorySlot slot)
        {
            // Check if the clicked slot is a equipped slot
            if (slot.isEquippedSlot)
            {
                if (InventoryManager.instance.IsInventoryFull(slot.item.GetComponent<Item>().type))
                    return;
                // Call the Pickup method to put the equipped item in the first empty slot and remove as equipped item
                Pickup(ref slots, ref equippedSlot, slot.item.GetComponent<Item>());
                equippedSlot.DropItem();
                return;
            }

            // Swap the items between the clicked slot and the equipped slot using temporary variables
            var index = slots.IndexOf(slot);
            var x = slot.item.GetComponent<Item>();
            var y = equippedSlot.IsEmpty() ? null : equippedSlot.item.GetComponent<Item>();

            slot.DropItem();
            equippedSlot.DropItem();

            equippedSlot.SetItem(x, x.type == ItemType.WEAPON);
            if (y != null)
                slots[index].SetItem(y);
        }

        private static bool Pickup(ref List<InventorySlot> slots, Item item)
        {
            // Find the first empty slot in the list of slots
            var slot = slots.FirstOrDefault(s => s.IsEmpty());
            if (slot is null) return false;

            slot.SetItem(item, item.type == ItemType.WEAPON);
            return true;
        }

        private static bool Pickup(ref List<InventorySlot> slots, ref InventorySlot equippedSlot, Item item)
        {
            // Find the first empty slot in the list of slots
            var slot = equippedSlot.IsEmpty() ? equippedSlot : slots.FirstOrDefault(s => s.IsEmpty());
            if (slot is null) return false;

            slot.SetItem(item, item.type == ItemType.WEAPON);
            return true;
        }

        public bool IsInventoryFull(ItemType type)
        {
            switch (type)
            {
                case ItemType.ARMOR:
                    return armorSlots.All(s => !s.IsEmpty());
                case ItemType.WEAPON:
                    return weaponSlots.All(s => !s.IsEmpty());
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SalvageArmor()
        {
            if (!equippedArmorSlot.IsEmpty())
            {
                equippedArmorSlot.DropItem();
                TokenManager.instance.AddToken();
            } 
        }

        public void SalvageWeapon()
        {
            if (!equippedWeaponSlot.IsEmpty())
            {
                equippedWeaponSlot.DropItem();
                TokenManager.instance.AddToken();
            }
        } 
    }
}
