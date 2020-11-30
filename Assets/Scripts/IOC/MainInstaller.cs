using System.Runtime.CompilerServices;
using UnityEngine;
using Zenject;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private SceneDataScriptable _sceneData;
    
    public override void InstallBindings()
    {
        Container.Bind<SceneDataScriptable>().FromInstance(_sceneData);
    }
}