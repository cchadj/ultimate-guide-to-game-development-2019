using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image _playerLivesDisplay;
    
    [SerializeField] private Sprite[] _playerLiveSprites;

    [SerializeField] private TextMeshProUGUI _gameOverText;
    
    [SerializeField] private TextMeshProUGUI _playerRestartText;
    
    [SerializeField] private ObservableFloatVariable _playerHealthPoints;
    
    [SerializeField, ReadOnly] private PlayerStateScriptable _playerState;
    
    [SerializeField, ReadOnly] private GameStateScriptable _gameState;

    [Inject]
    private void Constructor(PlayerStateScriptable playerState, GameStateScriptable gameState)
    {
        _playerState = playerState;
        _gameState = gameState;
    }

    private void OnEnable()
    {
        _playerHealthPoints.valueChanged += PlayerHealthChanged;
        _playerState.PlayerDied.AddListener(this, OnPlayerDied);
        _gameState.PlayerPressedRestart.AddListener(this, OnPlayerPressedRestart);
    }

    private void OnDisable()
    {
        _playerHealthPoints.valueChanged -= PlayerHealthChanged;
        _playerState.PlayerDied.RemoveListener(this, OnPlayerDied);
        _gameState.PlayerPressedRestart.RemoveListener(this, OnPlayerPressedRestart);
    }

    private Coroutine _playerDiedCoroutine;
    private void OnPlayerDied()
    {
        _playerRestartText.enabled = true;
        _playerDiedCoroutine = StartCoroutine(FlashGameOverText());
    }

    private void OnPlayerPressedRestart()
    {
        if (!_playerState.IsDead) return;
        
        StopCoroutine(_playerDiedCoroutine);
        _playerRestartText.enabled = false;
        _gameOverText.enabled = false;
    }
    
    private IEnumerator FlashGameOverText()
    {
        while (true)
        {
            _gameOverText.enabled = true;
            yield return new WaitForSeconds(1);
            _gameOverText.enabled = false;
            yield return new WaitForSeconds(1);
            _gameOverText.enabled = false;
        }
    }
    
    private void PlayerHealthChanged(object sender, ScriptableEventArgs e)
    {
        var playerHealthPoints = e.GetArguments<FloatVariable>().Value;
        var healthRatio = playerHealthPoints / _playerState.MaxHealthPoints;
        var healthSpriteIndex = Mathf.CeilToInt(Mathf.Lerp(0, _playerLiveSprites.Length - 1, healthRatio));

        _playerLivesDisplay.sprite = _playerLiveSprites[healthSpriteIndex];
    }
}
