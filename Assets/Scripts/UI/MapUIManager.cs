using UI;
using UI.Runtime;
using UnityEngine;
using Utils;

public class MapUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject Map;

    public static MapUIManager instance { get; private set; }

    private InputManager _inputManager;
    private GameObject mapCamera;

    public bool mapIsActive
    {
        get => _mapIsActive;
        set
        {
            _mapIsActive = value;
            if (value)
                InventoryOverlayBehaviour.instance.inventoryOpened = false;
            OnMapToggled?.Invoke(mapIsActive);
            Map.SetActive(value);
            mapCamera.SetActive(value);
        }
    }

    private bool _mapIsActive;

    public delegate void MapToggled(bool mapOpened);
    public static event MapToggled OnMapToggled;

    private void Awake()
    {
        // Singleton
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        _inputManager = new InputManager();
    }

    void Update()
    {
        _inputManager.UI.Enable();
        if (_inputManager.UI.ToggleMap.triggered && !(DialogueManager.instance.dialogueActive || PauseMenu.instance.isPaused || Navigation.instance.loadingScreenActive))
        {
            ToggleMap();
        }
        if(mapCamera == null) GetMapCamera();
    }

    public void ToggleMap()
    {
        mapIsActive = !mapIsActive;

        Time.timeScale = mapIsActive ? 0f : 1f;
    }

    private void GetMapCamera()
    {
        foreach (Camera cam in Camera.allCameras)
        {
            if (cam.name == "MapCamera")
            {
                mapCamera = cam.gameObject;
                mapCamera.SetActive(false);
            }
        }
    }
}
