using UI.Runtime;
using UI.Shop;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI
{
    public class InventoryOverlayBehaviour : MonoBehaviour
    {
        public delegate void InventoryToggle(bool inventoryOpened);
        public static event InventoryToggle OnInventoryToggle;

        [SerializeField]
        private GameObject overlay;

        [SerializeField]
        private GameObject entryButton;

        [SerializeField]
        private GameObject shop;

        private InputManager _inputManager;
        private bool _inventoryOpened;

        public bool inventoryOpened
        {
            get { return _inventoryOpened; }
            set
            {
                _inventoryOpened = value;
                if (value)
                    MapUIManager.instance.mapIsActive = false;
                OnInventoryToggle?.Invoke(value);
                overlay.SetActive(inventoryOpened);
                EventSystem.current.SetSelectedGameObject(value ? entryButton : null);
                if (shop != null)
                    shop.SetActive(value);
            }
        }

        public static InventoryOverlayBehaviour instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            _inputManager = new InputManager();
        }

        private void OnEnable()
        {
            _inputManager.Enable();
            _inputManager.UI.ToggleInventory.performed += _ => OnToggleInventory();
        }

        private void OnToggleInventory()
        {
            if (PauseMenu.instance.isPaused) return;
            if (DialogueManager.instance.dialogueActive) return;
            if (Navigation.instance.loadingScreenActive) return;

            inventoryOpened = !overlay.activeSelf;
            Time.timeScale = inventoryOpened ? 0f : 1f;
        }

        private void OnDisable()
        {
            _inputManager.Disable();
            _inputManager.UI.ToggleInventory.performed -= _ => OnToggleInventory();
        }
    }
}
