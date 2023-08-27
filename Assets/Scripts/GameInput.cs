using UnityEngine;

public class GameInput : MonoBehaviour
{
    private InputManager _inputManager;

    private void Awake()
    {
        _inputManager = new InputManager();
        _inputManager.Player.Enable();
    }

    public Vector2 GetMovement()
    {
        var inputVector = _inputManager.Player.Move.ReadValue<Vector2>();
        return inputVector;
    }

    public Vector2 GetLookPosition()
    {
        var lookPosition = _inputManager.Player.Look.ReadValue<Vector2>();
        return lookPosition;
    }

    public bool GetDash()
    {
        var dash = _inputManager.Player.Dash.ReadValue<float>() > 0;
        return dash;
    }

    public bool GetAttack()
    {
        var attack = _inputManager.Player.Attack.triggered;
        return attack;
    }

    public bool GetPickUp()
    {
        var pickup = _inputManager.Player.LootPickup.triggered;
        return pickup;
    }

    public bool GetNextSentence()
    {
        var nextSentence = _inputManager.UI.DisplayNextSentence.triggered;
        return nextSentence;
    }
}
