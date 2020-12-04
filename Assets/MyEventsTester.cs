using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class MyEventsTester : MonoBehaviour
{
    private PlayerStateScriptable _playerState;
    [SerializeField] private GameEvent _performanceEvent;

    
    private void OnEnable()
    {
       _performanceEvent?.AddListener(this, TestNoOp);
       _playerState.PlayerTookDamageEvent.AddListener(this, TestEvent);
    }
    
    private void OnDisable()
    {
       _performanceEvent?.RemoveListener(this, TestNoOp);
       _playerState.PlayerTookDamageEvent.RemoveListener(this, TestEvent);
    }
    
    public void RaisePerformanceEvent()
    {
      _performanceEvent.Raise(); 
    }

    public void TestNoOp()
    {
       var a = 5;
       Console.WriteLine("This is an output" + a + Random.value);
    }
    
    public void Test()
    {
       Debug.Log("Test() function called " + gameObject.name );
    }
    
    public void Test1()
    {
       Debug.Log("Test1"); 
    }
    
    public void Test2()
    {
       Debug.Log("Test2"); 
    }
    
    public void Test3()
    {
       Debug.Log("Test3"); 
    }
    
    public void Test4()
    {
       Debug.Log("Test4"); 
    }
    
    public void Test5()
    {
       Debug.Log("Test5"); 
    }

    [Inject]
    private void Constructor(PlayerStateScriptable playerState)
    {
       _playerState = playerState;
    }

    private void TestEvent()
    {
       Debug.Log("TestEvent() called from " + name + " of " + gameObject.name );
    }

}
