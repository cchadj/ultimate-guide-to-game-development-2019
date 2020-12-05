using System.Runtime.CompilerServices;
using UnityEngine;
using Zenject;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public interface IMyFooFactory : IFactory<Enemy>
{
}

public partial class Enemy
{
    public class Factory : PlaceholderFactory<Enemy>, IMyFooFactory
    {
    }
}

public class MainInstaller : MonoInstaller
{
    [SerializeField] private SceneDataScriptable _sceneData;
    
    public override void InstallBindings()
    {
        Container.Bind<SceneDataScriptable>().FromInstance(_sceneData);
        Container.BindFactoryCustomInterface<Enemy, Enemy.Factory, IMyFooFactory>();
    }
}