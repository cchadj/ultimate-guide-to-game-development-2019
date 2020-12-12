using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName="State/GameState")]
public class GameStateScriptable : ScriptableObject, IConstructor, IDestructor
{
    [field:SerializeField] public GameEventWithArguments EnemyDestroyed { get; private set; }
    
    [field:SerializeField] public GameEvent PlayerPressedRestart { get; private set; }

    public bool IsConstructed { get; private set; }
    public void Constructor()
    {
        if (IsConstructed) return;
        
        IsConstructed = true;
        IsConstructed = false;
    }

    
    public bool IsDestructed { get; private set; }

    public void Destructor()
    {
        if (IsDestructed) return;
        
        IsConstructed = false;
        IsDestructed = true;
    }
}
