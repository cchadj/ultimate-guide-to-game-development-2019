using System;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using Zenject;

[CreateAssetMenu(menuName="State/PlayerState")]
public class PlayerStateScriptable : ScriptableObject
{
    public int PlayerMaxHealth;

    [field:SerializeField, Space] public float HealthPoints { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool IsPlayerDead { get; set; }
    
    public Action PlayerDied { get; set; }
    public Action PlayerTookDamage { get; set; }
}
