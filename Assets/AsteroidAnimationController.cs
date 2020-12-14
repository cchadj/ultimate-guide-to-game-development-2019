using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AnimationController : MonoBehaviour
{
    protected Animator _animator;
    
    protected virtual void Awake()
    {
        _animator = GetComponent<Animator>();
    }
}
public class AsteroidAnimationController : AnimationController
{
    private int _onDestructionTriggerHash;
    
    private int _rotationsPerSecondFloatHash;

    
    private GameStateScriptable _gameState;

    [Inject] 
    private void InjectDependencies(GameStateScriptable gameState)
    {
        _gameState = gameState;
    }
    
    protected override void Awake()
    {
        base.Awake();
        _onDestructionTriggerHash = Animator.StringToHash("OnDestruction");
        _rotationsPerSecondFloatHash = Animator.StringToHash("RotationsPerSecond");
    }

    public void SetRotationsPerSecond(float rotationSpeed)
    {
        _animator.SetFloat(_rotationsPerSecondFloatHash, rotationSpeed); 
    }
    
    public void PlayDestructAnimation()
    {
       _animator.SetTrigger(_onDestructionTriggerHash); 
    }
    
}
