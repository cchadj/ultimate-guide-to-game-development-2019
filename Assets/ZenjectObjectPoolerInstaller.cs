using UnityEngine;
using Zenject;

public class ZenjectObjectPoolerInstaller : MonoInstaller
{
    [SerializeField, ReadOnly] public PoolableMonobehaviour PoolableMonobehaviourPrefab;

    public override void InstallBindings()
    {
        Container.BindFactory<PoolableMonobehaviour, PoolableMonobehaviour.Factory>().FromComponentInNewPrefab(PoolableMonobehaviourPrefab);
    }
}
