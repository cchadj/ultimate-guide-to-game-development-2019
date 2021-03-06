﻿using System;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName="State/PlayerState")]
public class     PlayerStateScriptable : ScriptableObject, IConstructor, IDestructor
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
    
    [SerializeField, MiniView] private ObservableFloatVariable _healthPoints;
    
    [SerializeField, MiniView] private ObservableIntVariable _shieldPoints;
    
    [SerializeField, MiniView] private ObservableIntVariable _score;
    
    [field: SerializeField, ReadOnly] public float MovementSpeed { get; set; }
    
    private const float FloatTolerance = 0.001f;

    public float HealthPoints
    {
        get => _healthPoints.Value;
        set
        {
            _healthPoints.Value = Mathf.Clamp(value,0, MaxHealthPoints);
            IsDead = Math.Abs(_healthPoints.Value) < FloatTolerance;
            
            if (IsDead)
                PlayerDied.Raise(this);
        }
    }

    public int ShieldPoints { 
        get => _shieldPoints.Value;
        set
        {
            _shieldPoints.Value = Mathf.Clamp(value, 0, MaxShieldPoints);

            var isShieldNegative = value < 0;
            if (!isShieldNegative) return;
            
            PlayerShieldDestroyed.Raise(this);
            HealthPoints += value;
        } 
    }

    private void OnEnable()
    {
        PlayerShieldDestroyed.AddOwner(this);
        PlayerDied.AddOwner(this);
    }

    private void OnDisable()
    {
        PlayerShieldDestroyed.RemoveOwner(this);
        PlayerDied.RemoveOwner(this);
    }

    public int Score
    {
        get => _score.Value;
        set => _score.Value = value;
    }

    private GameStateScriptable _gameState;
    
    [Inject]
    private void InitializeInjections(GameStateScriptable gameState)
    {
        _gameState = gameState;
    }
    
    
    public void Constructor()
    {
        if (IsConstructed)
            return;
        
        Subscribe();
        _gameState.EnemyDestroyed.AddListener<EnumEntry>(this, EnemyDestroyed);
        
        HealthPoints = MaxHealthPoints;
        ShieldPoints = 0;
        Score = 0;
        MovementSpeed = InitialMovementSpeed;
        
        IsConstructed = true;
        IsDestructed = false;
    }
    
    public bool IsDestructed { get; private set; }
    public void Destructor()
    {
        if (IsDestructed)
            return;
        
        Unsubscribe();
        _gameState.EnemyDestroyed.RemoveListener<EnumEntry>(this, EnemyDestroyed);
        
        IsConstructed = false;
        IsDestructed = true;
    }

    private void EnemyDestroyed(EnumEntry enemyTypeEnumEntry)
    {
        var isParseSuccessful = Enum.TryParse(enemyTypeEnumEntry.EnumEntryName, true, out EnemyType enemyType);

        if (!isParseSuccessful) return;
        
        switch (enemyType)
        {
            case EnemyType.NormalEnemy:
                Score += 10;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
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

} 
