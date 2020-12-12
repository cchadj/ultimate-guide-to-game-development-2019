using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MyProjectInstaller : MonoInstaller
{
    [SerializeField] private GameManager _gameManager;
    
    [SerializeField] private SceneLoader _sceneLoader;
    
    public override void InstallBindings()
    {
        Container.Bind<GameManager>().FromInstance(_gameManager);
        
        Container.Bind<SceneLoader>().FromInstance(_sceneLoader);
    }
}
