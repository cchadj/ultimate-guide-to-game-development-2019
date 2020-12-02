using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.Interactions;
using Zenject;

[CreateAssetMenu(menuName="State/PlayerState")]
public class PlayerStateScriptable : ScriptableObject
{
    public int PlayerMaxHealth;

    
    [field:SerializeField, Space(5)] public float HealthPoints { get; set; }
    [field:SerializeField] public int ShieldPoints { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool IsPlayerDead { get; set; }
    
    public Action PlayerDied { get; set; }
    
    public Action PlayerTookDamage { get; set; }

    public GameEvent PlayerDiedUnityEvent;
    
    public GameEvent PlayerTookDamageUnityEvent;
    

    private void OnEnable()
    {
        PlayerDied += RaisePlayerDied;
        PlayerTookDamage += RaisePlayerTookDamage;
    }
    
    private void OnDisable()
    {
        PlayerDied -= RaisePlayerDied;
        PlayerTookDamage -= RaisePlayerTookDamage;
    }

    private void RaisePlayerDied()
    {
        PlayerDiedUnityEvent.Raise();
    }

    public void RaisePlayerTookDamage()
    {
        PlayerTookDamageUnityEvent.Raise();
    }
}
