using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class StateInstaller : MonoInstaller<StateInstaller>
{
    [SerializeField] private GameStateScriptable _gameState;
    [SerializeField] private PlayerStateScriptable _playerState;
    
    public override void InstallBindings()
    {
        Container.Bind<GameStateScriptable>().FromInstance(_gameState).AsSingle();
        Container.Bind<PlayerStateScriptable>().FromInstance(_playerState).AsSingle();
        
        // ScriptableObjects are not Injected in the begining (only monobehaviours in the scene)
        // So we do manual injection here
        Container.QueueForInject(_gameState);
        Container.QueueForInject(_playerState);
    }
}
