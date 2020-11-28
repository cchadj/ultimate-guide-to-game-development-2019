using System.Runtime.CompilerServices;
using UnityEngine;
using Zenject;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class MainInstaller : MonoInstaller
{
    [SerializeField] private Player _player;
    
    public override void InstallBindings()
    {
        Container.Bind<Player>().FromInstance(_player).AsSingle();
        Debug.Log(_player.GetHashCode());
    }
}