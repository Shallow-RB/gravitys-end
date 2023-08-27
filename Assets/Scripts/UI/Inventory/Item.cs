using Core.Audio;
using UnityEngine;

namespace UI.Inventory
{
    public class Item : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField]
        [Tooltip("The icon to display in the inventory")]
        public Sprite icon;

        [SerializeField]
        [Tooltip("The type of item this is")]
        public ItemType type;

        [Header("Gameplay")]
        [SerializeField]
        [Range(0, 100)]
        [Tooltip("Modifier in percentage (armor only)")]
        public int armorModifier;

        [SerializeField]
        [Tooltip("The prefab to spawn when this item is dropped")]
        public GameObject prefab;

        [SerializeField]
        [Tooltip("The mesh renderer of the item")]
        private MeshRenderer meshRenderer;

        [SerializeField]
        [Range(0f, 5f)]
        [Tooltip("The price of the item in the shop in minutes")]
        public float price = 3f;

        [HideInInspector]
        public bool IsInInventory;

        public delegate void ItemPickupEventHandler(bool canPickup, ItemType type);
        public static event ItemPickupEventHandler OnItemPickup;

        private GameObject _player;
        private GameInput _gameInput;
        private bool _gamePaused;
        private float originalYPosition;
        private bool isPlayerNearby;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            _gameInput = FindObjectOfType<GameInput>();
            UIHandler.OnPauseGameToggle += PauseGame;

            originalYPosition = transform.position.y;
        }

        public void PauseGame(bool paused)
        {
            _gamePaused = paused;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && !IsInInventory)
            {
                OnItemPickup?.Invoke(true, type);
                isPlayerNearby = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnItemPickup?.Invoke(false, type);
                isPlayerNearby = false;
            }
        }

        private void Update()
        {
            if (_player == null) _player = GameObject.FindGameObjectWithTag("Player");
            if (_gameInput == null) _gameInput = FindObjectOfType<GameInput>();

            Pickup();
        }

        public float GetArmorModifier()
        {
            return (float)armorModifier / 100;
        }

        public void Spawn(Vector3 position)
        {
            IsInInventory = false;
            RenderItem(true);
            OnItemPickup?.Invoke(true, type);
            isPlayerNearby = true;
            gameObject.transform.position = position;
        }

        public void Spawn()
        {
            Spawn(_player.transform.position);
        }

        private void Pickup()
        {
            if (!CanPickup())
                return;

            if (!InventoryManager.instance.PickupItem(this))
                return;

            meshRenderer.enabled = false;
            IsInInventory = true;
            OnItemPickup?.Invoke(false, type);
            isPlayerNearby = false;
            if (type == ItemType.WEAPON)
            {
                SoundEffectsManager.instance.PlaySoundEffect(SoundEffect.GUN_PICKUP);
                return;
            }

            SoundEffectsManager.instance.PlaySoundEffect(SoundEffect.ARMOR_PICKUP);
        }

        private bool CanPickup()
        {
            return meshRenderer.enabled && !IsInInventory && !_gamePaused && _gameInput.GetPickUp() && isPlayerNearby;
        }

        public void RenderItem(bool render)
        {
            if (render)
                originalYPosition = transform.position.y;
            meshRenderer.enabled = render;
        }
    }

    public enum ItemType
    {
        [InspectorName("Weapon")]
        WEAPON,

        [InspectorName("Armor")]
        ARMOR,
    }
}