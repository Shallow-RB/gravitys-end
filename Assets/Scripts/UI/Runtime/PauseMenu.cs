using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Runtime
{
    public class PauseMenu : MonoBehaviour
    {
        public delegate void PauseToggle(bool paused);
        public static event PauseToggle OnPauseToggle;

        [SerializeField]
        public GameObject pauseMenu;

        [SerializeField]
        public GameObject controlsImage;

        [SerializeField]
        private GameObject entryButton;

        [Header("Settings")]
        [SerializeField]
        private GameObject settingsMenu;

        [SerializeField]
        private GameObject settingsEntryButton;

        [SerializeField]
        private GameObject dialogue;

        private InputManager _inputManager;
        private bool _isPaused;

        // Singleton for pausemenu
        public static PauseMenu instance;
        public bool isPaused
        {
            get => _isPaused;
            set
            {
                _isPaused = value;
                if (value)
                    InventoryOverlayBehaviour.instance.inventoryOpened = false;

                pauseMenu.SetActive(value);
                OnPauseToggle?.Invoke(value);
                EventSystem.current.SetSelectedGameObject(value ? entryButton : null);
            }
        }

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
            _inputManager.UI.TogglePauseMenu.performed += _ => TogglePause(!isPaused);
            _inputManager.UI.CloseMenu.performed += _ => CloseMenu();
        }

        private void OnDisable()
        {
            _inputManager.Disable();
        }

        public void ToggleControls()
        {
            controlsImage.SetActive(!controlsImage.activeSelf);
        }

        public void TogglePause(bool force = false)
        {
            if (settingsMenu.activeSelf)
            {
                ToggleSettings();
                return;
            }

            if (InventoryOverlayBehaviour.instance.inventoryOpened)
            {
                InventoryOverlayBehaviour.instance.inventoryOpened = false;
                Time.timeScale = 1f;
                return;
            }

            if(MapUIManager.instance.mapIsActive)
                MapUIManager.instance.ToggleMap();

            if(dialogue != null)
            {
                DialogueManager dialogueManager = dialogue.GetComponent<DialogueManager>();
                if (dialogueManager.dialogueActive)
                {
                    dialogue.SetActive(!force);
                    dialogue.GetComponent<DialogueManager>().ToggleDialoguePaused();
                }
            }
            
            isPaused = force;
            Time.timeScale = force ? 0f : 1f;
        }

        private void CloseMenu()
        {
            if (isPaused && pauseMenu.activeSelf)
                TogglePause();
        }

        public void ToggleSettings()
        {
            settingsMenu.SetActive(!settingsMenu.activeSelf);
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            EventSystem.current.SetSelectedGameObject(settingsMenu.activeSelf ? settingsEntryButton : entryButton);
        }
    }
}
