using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private SceneDataScriptable _sceneData;
    
    public override void InstallBindings()
    {
        Container.Bind<SceneDataScriptable>().FromInstance(_sceneData);
    }
}