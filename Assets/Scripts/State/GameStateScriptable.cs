using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

[CreateAssetMenu(menuName="State/GameState")]
public class GameStateScriptable : ScriptableObject, IConstructor, IDestructor
{
    [field:SerializeField] public GameEventWithArguments EnemyDestroyed { get; private set; }
    
    [field:SerializeField] public GameEvent PlayerPressedRestart { get; private set; }

    [field: SerializeField] private SceneLoader _sceneLoader;

    [Inject]
    private void DependencyInjection(SceneLoader sceneLoader)
    {
        _sceneLoader = sceneLoader;
    }

    public bool IsConstructed { get; private set; }
    public void Constructor()
    {
        if (IsConstructed) return;
        
        PlayerPressedRestart.AddListener(this, RestartGame); 
        
        IsConstructed = true;
        IsDestructed = false;
    }

    
    public bool IsDestructed { get; private set; }

    public void Destructor()
    {
        if (IsDestructed) return;
        
        PlayerPressedRestart.RemoveListener(this, RestartGame); 
        
        IsConstructed = false;
        IsDestructed = true;
    }

    private void RestartGame()
    {
        _sceneLoader.LoadLevel(1);
    }
}
