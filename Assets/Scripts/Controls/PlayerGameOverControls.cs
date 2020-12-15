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
        _gameState.PlayerPressedRestart.AddOwner(this);
        _playerState.PlayerDied.AddListener(this, EnableGameOverControls);
    }
    
    private void OnDisable()
    {
        _gameState.PlayerPressedRestart.RemoveOwner(this);
        _playerState.PlayerDied.RemoveListener(this, EnableGameOverControls);
        _controls?.Disable();
        _isPlayerPressedStartAlreadyRaised = false;
    }

    private void EnableGameOverControls()
    {
        _controls?.Enable();
    }

    private bool _isPlayerPressedStartAlreadyRaised = false;
    public void OnRestart(InputAction.CallbackContext context)
    {
        if (_isPlayerPressedStartAlreadyRaised) return;
        
        _gameState.PlayerPressedRestart.Raise(this);
        _isPlayerPressedStartAlreadyRaised = true;
    }
}
