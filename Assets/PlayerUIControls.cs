using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerUIControls : MonoBehaviour, Controls.IUIActions
{
    [SerializeField, ReadOnly] private GameStateScriptable _gameState;

    private Controls _controls;

    [Inject]
    private void InitInject(GameStateScriptable gameState)
    {
        _gameState = gameState;
    }

    private void Awake()
    {
        _controls = new Controls();
        _controls.UI.SetCallbacks(this);
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        throw new System.NotImplementedException();
    }

    public void OnRestart(InputAction.CallbackContext context)
    {
        _gameState.PlayerPressedRestart.Raise();
    }
}
