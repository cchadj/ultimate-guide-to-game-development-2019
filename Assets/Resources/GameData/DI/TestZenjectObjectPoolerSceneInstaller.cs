using UnityEngine;
using Zenject;

public class TestZenjectObjectPoolerSceneInstaller : MonoInstaller
{
    [SerializeField] private SceneDataScriptable _sceneData;
    [SerializeField] private GameStateScriptable _gameState;
    public override void InstallBindings()
    {
        Container.Bind<SceneDataScriptable>().FromInstance(_sceneData);
        Container.Bind<GameStateScriptable>().FromInstance(_gameState);
    }
}