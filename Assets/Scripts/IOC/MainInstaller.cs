using UnityEngine;
using Zenject;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private SceneDataScriptable _sceneData;
    [SerializeField] private Camera _mainCamera;
    public override void InstallBindings()
    {
        Container.Bind<SceneDataScriptable>().FromInstance(_sceneData);
        Container.Bind<Camera>().FromInstance(Camera.main);
        
        Container.QueueForInject(_sceneData);
    }
}