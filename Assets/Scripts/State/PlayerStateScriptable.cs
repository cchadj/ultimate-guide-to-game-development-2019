using System;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName="State/PlayerState")]
public class     PlayerStateScriptable : ScriptableObject, IInitializable, IConstructor, IDestructor
{
    [field:SerializeField] public int MaxHealthPoints { get; private set; }
    
    [field:SerializeField] public int MaxShieldPoints { get; private set; }

    [field:SerializeField] public float InitialMovementSpeed { get; private set;}
    
    [field:SerializeField] public float SpeedupMultiplier { get; private set;}
  
    [field:SerializeField] public GameEvent PlayerDied { get; private set; }
    [field:SerializeField] public GameEvent PlayerPickedSpeedBoost { get; private set; }
    [field:SerializeField] public GameEvent PlayerPickedTripleShot { get; private set; }
    [field:SerializeField] public GameEvent PlayerPickedShield { get; private set; }
    [field:SerializeField] public GameEvent PlayerShieldDestroyed { get; private set; }
    [field:SerializeField] public GameEventWithArguments PlayerTookDamage { get; private set; }
    
    [SerializeField, ReadOnly] private float _healthPoints;
    
    [SerializeField, ReadOnly]private int _shieldPoints;
    
    private const float FloatTolerance = 0.001f;

    public float HealthPoints
    {
        get => _healthPoints;
        set
        {
            _healthPoints = Mathf.Clamp(value,0, MaxHealthPoints);
            IsDead = Math.Abs(_healthPoints) < FloatTolerance;
            
            if (IsDead)
                PlayerDied.Raise();
        }
    }

    public int ShieldPoints { 
        get => _shieldPoints;
        set
        {
            _shieldPoints = Mathf.Clamp(value, 0, MaxShieldPoints);

            var isShieldNegative = value < 0;
            if (!isShieldNegative) return;
            
            PlayerShieldDestroyed.Raise();
            HealthPoints += value;
        } 
    }

    [field: SerializeField]
    [field: ReadOnly]
    public float MovementSpeed { get; set; }

    public void Initialize()
    {
        HealthPoints = MaxHealthPoints;
        ShieldPoints = 0;
        MovementSpeed = InitialMovementSpeed;
    }
    
    private void Subscribe()
    {
        if (PlayerDied != null)
            PlayerDied.AddListener(this, Kill);
        if (PlayerTookDamage != null)
            PlayerTookDamage.AddListener<FloatVariable>(this, Damage);
    }

    private void Unsubscribe()
    {
        if (PlayerDied != null)
            PlayerDied.RemoveListener(this, Kill);
        if (PlayerTookDamage != null)
            PlayerTookDamage.RemoveListener<FloatVariable>(this, Damage);
    }

    private void Damage(FloatVariable damageAmount)
    {
        if (ShieldPoints > 0)
        {
            ShieldPoints -= (int)damageAmount.Value;
        }
        else
        {
            HealthPoints -= (int)damageAmount.Value;
        }
    }

    public bool IsDead { get; set; }

    private void Kill()
    {
        IsDead = true;
    }

    public bool IsConstructed { get; private set; }

    public void Constructor()
    {
        if (IsConstructed)
            return;
        Debug.Log("Im being constructed");
        
        IsConstructed = true;
        IsDestructed = false;
    }

    public bool IsDestructed { get; private set; }
    public void Destructor()
    {
        if (IsDestructed)
            return;
        Debug.Log("Im being destructed");
        
        IsConstructed = false;
        IsDestructed = true;
    }
} 
