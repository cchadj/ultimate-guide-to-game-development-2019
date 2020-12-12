using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ZenjectObjectPoolerContext : GameObjectContext
{
    private MonoInstaller _installer;
        
    public void AddMonobehaviourInstaller(MonoInstaller installer)
    {
        var installers = ((List<MonoInstaller>) Installers);

        if (!installers.Contains(installer))
        {
            installers.Add(installer);
        }
    }
    
    protected override void GetInjectableMonoBehaviours(List<MonoBehaviour> monoBehaviours)
    {
        if (_installer)
            monoBehaviours.Add(_installer);
        
        base.GetInjectableMonoBehaviours(monoBehaviours);
    }
}
