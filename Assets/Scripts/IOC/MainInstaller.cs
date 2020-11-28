using System.Runtime.CompilerServices;
using UnityEngine;
using Zenject;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private Player _player;
    [SerializeField] private SceneDataScriptable _sceneData;
    
    public override void InstallBindings()
    {
        Container.Bind<Player>().FromInstance(_player).AsSingle();
        Container.Bind<SceneDataScriptable>().FromInstance(_sceneData);
    }
}