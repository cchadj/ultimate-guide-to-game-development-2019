using System.Runtime.CompilerServices;
using UnityEngine;
using Zenject;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class PrefabInstaller : MonoInstaller
{
    [SerializeField] private GameObject _enemyPrefab;
    
    public override void InstallBindings()
    {
    }
}