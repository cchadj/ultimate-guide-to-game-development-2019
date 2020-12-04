using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Zenject;

public class MyEventsTester : MonoBehaviour
{
    private PlayerStateScriptable _playerState;

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

    private void OnEnable()
    {
       _playerState.PlayerTookDamageEvent.AddListener(this, TestEvent);
    }
    
    private void OnDisable()
    {
       _playerState.PlayerTookDamageEvent.RemoveListener(this, TestEvent);
    }
}
