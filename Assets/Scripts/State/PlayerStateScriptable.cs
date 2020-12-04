using System;
using UnityEngine;

[CreateAssetMenu(menuName="State/PlayerState")]
public class PlayerStateScriptable : ScriptableObject
{
    public int PlayerMaxHealth;
    
    [field:SerializeField, Space(5)] public float HealthPoints { get; set; }
    [field:SerializeField] public int ShieldPoints { get; set; }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public bool IsPlayerDead { get; set; }

    public event EventHandler PlayerDied;

    public event EventHandler PlayerTookDamage; 
    
    public GameEvent PlayerTookDamageEvent; 

    [ContextMenu("Raise PlayerDied")]
    public virtual void OnPlayerDied()
    {
        PlayerDied?.Invoke(this, EventArgs.Empty);
    }

    [ContextMenu("Raise PlayerTookDamage")]
    public virtual void OnPlayerTookDamage()
    {
        PlayerTookDamage?.Invoke(this, EventArgs.Empty);
        PlayerTookDamageEvent.Raise();
    }
}
