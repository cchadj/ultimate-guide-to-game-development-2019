using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyInstaller : MonoInstaller
{
    [SerializeField] private Spawner _bulletSpawner;

    public override void InstallBindings()
    {
        Container.Bind<Spawner>().FromInstance(_bulletSpawner).AsSingle();
    }
}
