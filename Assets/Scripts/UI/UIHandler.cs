using UnityEngine;
using UI.Runtime;
using UI.Inventory;
using System.Collections.Generic;
using TMPro;
using UnityEngine.InputSystem;
using Core.Chest;

namespace UI
{
    public class UIHandler : MonoBehaviour
    {
        public delegate void PauseGameToggle(bool gamePaused);
        public static event PauseGameToggle OnPauseGameToggle;

        [SerializeField]
        private GameObject inventoryMenuFirstSelected;

        [SerializeField]
        private GameObject pauseMenuFirstSelected;

        [SerializeField]
        private GameObject itemPickupPrompt;

        private List<int> _itemsNearby = new List<int>();
        private InputManager _inputManager;

        private void Start()
        {
            InventoryOverlayBehaviour.OnInventoryToggle += (bool active) => PauseGame(active);
            PauseMenu.OnPauseToggle += (bool active) => PauseGame(active);
            MapUIManager.OnMapToggled += (bool active) => PauseGame(active);

            Item.OnItemPickup += OnShowPickupPrompt;
            Chest.OnChestOpen += OnChestOpen;
            _inputManager = new InputManager();
        }

        private void OnDestroy()
        {
            Item.OnItemPickup -= OnShowPickupPrompt;
            Chest.OnChestOpen -= OnChestOpen;
        }

        private void PauseGame(bool condition)
        {
            OnPauseGameToggle.Invoke(condition);
        }

        private void OnShowPickupPrompt(bool show, ItemType type)
        {
            if (!show)
            {
                itemPickupPrompt.SetActive(false);
                return;
            }

            if (InventoryManager.instance.IsInventoryFull(type))
                SetPlayerPrompt("Inventory Full");
            else
            {
                string key = FetchReadableKey(_inputManager.Player.LootPickup);
                SetPlayerPrompt($"[{key}] Take");
            }

            itemPickupPrompt.SetActive(true);
        }

        private void OnChestOpen(bool show)
        {
            if (!show)
            {
                itemPickupPrompt.SetActive(false);
                return;
            }

            string key = FetchReadableKey(_inputManager.Player.OpenChest);
            SetPlayerPrompt($"[{key}] Open Chest");

            itemPickupPrompt.SetActive(true);
        }

        private string FetchReadableKey(InputAction action)
        {
            if (action == null)
                return string.Empty;

            int bindingIndex = action.GetBindingIndexForControl(action.controls[0]);
            string key = InputControlPath.ToHumanReadableString(
                action.bindings[bindingIndex].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );
            return key;
        }

        private void SetPlayerPrompt(string value)
        {
            var prompt = itemPickupPrompt.GetComponent<TextMeshProUGUI>();
            prompt.text = value;
        }
    }
}
