using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerGameOverControls : MonoBehaviour, Controls.IGameOverActions
{
    [SerializeField, ReadOnly] private GameStateScriptable _gameState;
    
    [SerializeField, ReadOnly] private PlayerStateScriptable _playerState;

    [SerializeField] private Controls _controls;

    [Inject]
    private void InitInject(GameStateScriptable gameState, PlayerStateScriptable playerState)
    {
        _gameState = gameState;
        _playerState = playerState;
    }

    private void Awake()
    {
        _controls = new Controls();
        _controls.GameOver.SetCallbacks(this);
    }

    private void OnEnable()
    {
        _playerState.PlayerDied.AddListener(this, EnableGameOverControls);
    }
    
    private void OnDisable()
    {
        _playerState.PlayerDied.RemoveListener(this, EnableGameOverControls);
        _controls?.Disable(); 
    }

    private void EnableGameOverControls()
    {
        _controls?.Enable();
    }

    public void OnRestart(InputAction.CallbackContext context)
    {
        _gameState.PlayerPressedRestart.Raise();
    }
}
