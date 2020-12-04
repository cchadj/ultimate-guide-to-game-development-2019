using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Zenject;

public class MyEventsTester : MonoBehaviour
{
    public GameEvent Event;
    private PlayerStateScriptable _playerState;

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

    [ContextMenu("Add Listeners test")]
    public void AddListeners()
    {
       Event.AddListener(this, Test5);
    }
    
    [ContextMenu("Remove Listeners test")]
    public void RemoveListeners()
    {
       Event.RemoveListeners(this);
    }

    [Inject]
    private void Constructor(PlayerStateScriptable playerState)
    {
       _playerState = playerState;
    }

    private void PlayerTookDamageTester()
    {
       Debug.Log("PlayerTookDamage() called from " + name + " of " + gameObject.name);
    }

    private void OnEnable()
    {
       _playerState.PlayerTookDamageEvent.AddListener(this, PlayerTookDamageTester);
    }
    
    private void OnDisable()
    {
       _playerState.PlayerTookDamageEvent.RemoveListener(this, PlayerTookDamageTester);
    }
}
