using Controllers.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        public GameObject iconDisplay;

        [SerializeField]
        public GameObject removeButton;

        [SerializeField]
        public bool isEquippedSlot;

        public GameObject item { get; private set; }

        private InputManager _inputManager;
        private bool isHighlighted;

        private void Awake()
        {
            _inputManager = new InputManager();
        }

        private void Update()
        {
            if ((EventSystem.current.currentSelectedGameObject == iconDisplay || isHighlighted) && _inputManager.UI.DropItem.triggered)
                DropItem(true);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHighlighted = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHighlighted = false;
        }

        private void OnEnable()
        {
            _inputManager.UI.Enable();
        }

        private void OnDisable()
        {
            _inputManager.UI.Disable();
        }

        public void SetItem(Item obj, bool weapon = false)
        {
            // Toggle the item in the slot to the given object and its icon sprite.
            ToggleItem(obj.gameObject, obj.icon);

            obj.IsInInventory = true;
            obj.RenderItem(false);

            if (obj.gameObject.CompareTag("Ranged"))
                obj.gameObject.GetComponent<PlayerShoot>().OnWeaponUnequipped();

            // If the slot is an equipped slot and the item is a weapon, attach the weapon to the EquipmentSystem and render it as in the inventory.
            switch (isEquippedSlot)
            {
                case true when weapon:
                    EquipmentSystem.Instance.SetCurrentWeapon(obj.gameObject);
                    obj.RenderItem(true);

                    // If the equipped weapon is a ranged weapon, call the OnWeaponEquipped method of the PlayerShoot component.
                    GameObject equippedWeapon = EquipmentSystem.Instance._equippedWeapon;
                    if (equippedWeapon.CompareTag("Ranged"))
                    {
                        equippedWeapon.GetComponent<PlayerShoot>().OnWeaponEquipped();
                    }
                    break;

                // If the slot is an equipped slot and the item is not a weapon, attach the armor to the EquipmentSystem and render it as in the inventory.
                case true when !weapon:
                    EquipmentSystem.Instance.SetCurrentArmor(obj.gameObject);
                    obj.RenderItem(true);
                    break;
            }
        }

        public void DropItem(bool spawn = false)
        {
            // If there is no item in the slot, return early.
            if (item is null)
                return;

            var script = item.GetComponent<Item>();
            script.RenderItem(false);

            // If the slot is an equipment slot, detach the weapon or armor from the EquipmentSystem.
            switch (isEquippedSlot)
            {
                case true when script.type == ItemType.WEAPON:
                    EquipmentSystem.Instance.DetachWeapon();
                    break;
                case true when script.type == ItemType.ARMOR:
                    EquipmentSystem.Instance.DetachArmor();
                    break;
                default:
                    break;
            }

            // If the item is a ranged weapon and was equipped, call the OnWeaponUnequipped method of the PlayerShoot component.
            if (item.gameObject.CompareTag("Ranged") && isEquippedSlot)
            {
                item.GetComponent<PlayerShoot>().OnWeaponUnequipped();
            }

            // If the 'spawn' parameter is true, spawn the item in the game world.
            if (spawn)
                item.GetComponent<Item>().Spawn();

            // Toggle the item in the slot to null and the icon sprite to null.
            ToggleItem(null, null);
        }

        private void ToggleItem(GameObject obj, Sprite sprite)
        {
            // Get the Image and Button components of the icon display and remove button
            var iconImage = iconDisplay.GetComponent<Image>();
            var iconButton = iconDisplay.GetComponent<Button>();
            var removeImage = removeButton.GetComponent<Image>();
            var removeBtn = removeButton.GetComponent<Button>();

            // Set the item and icon sprite
            item = obj;
            iconImage.sprite = sprite;

            // Enable or disable the icon display and remove button based on whether the slot is empty
            var isEmpty = IsEmpty();
            iconImage.enabled = !isEmpty;
            removeImage.enabled = !isEmpty;
            removeBtn.interactable = !isEmpty;
        }

        public bool IsEmpty()
        {
            return item is null || item.gameObject is null;
        }

        public void Use()
        {
            InventoryManager.instance.UseItem(this);
        }
    }
}
