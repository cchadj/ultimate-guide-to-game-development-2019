using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Image _playerLivesDisplay;
    
    [SerializeField] private Sprite[] _playerLiveSprites;
    
    [SerializeField] private ObservableFloatVariable _playerHealthPoints;
    
    [SerializeField, ReadOnly] private PlayerStateScriptable _playerState;

    [Inject]
    private void Constructor(PlayerStateScriptable playerState)
    {
        _playerState = playerState;
    }

    private void OnEnable()
    {
        _playerHealthPoints.valueChanged += PlayerHealthChanged;
    }
    
    private void OnDisable()
    {
        _playerHealthPoints.valueChanged -= PlayerHealthChanged;
    }

    private void PlayerHealthChanged(object sender, ScriptableEventArgs e)
    {
        var playerHealthPoints = e.GetArguments<FloatVariable>().Value;
        var healthRatio = playerHealthPoints / _playerState.MaxHealthPoints;
        var healthSpriteIndex = Mathf.CeilToInt(Mathf.Lerp(0, _playerLiveSprites.Length - 1, healthRatio));

        _playerLivesDisplay.sprite = _playerLiveSprites[healthSpriteIndex];
    }
}
