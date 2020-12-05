using System;
using System.Runtime.Serialization;
using UnityEditor;
using UnityEngine;
using Zenject;

[CreateAssetMenu(menuName="State/PlayerState")]
public class PlayerStateScriptable : ScriptableObject, IInitializable
{
    public float heyMate;
    [field:SerializeField] public int MaxHealthPoints { get; private set; }
    
    [field:SerializeField] public int MaxShieldPoints { get; private set; }

    [field:SerializeField] public float InitialMovementSpeed { get; private set;}
    
    [field:SerializeField] public float SpeedupMultiplier { get; private set;}
    
     private const float FloatTolerance = 0.001f;
    
    [SerializeField, EnumMask] private BulletType _bulletType;
    
    public GameEvent PlayerDied;

    public GameEvent PlayerTookDamage;

    public GameEvent PlayerPickedSpeedBoost;

    public GameEvent PlayerPickedTripleShot;

    public GameEvent PlayerPickedShield;
    
    [SerializeField, ReadOnly] private float _healthPoints;
    public float HealthPoints
    {
        get => _healthPoints;
        set
        {
            _healthPoints = Mathf.Clamp(value,0, MaxHealthPoints);
            IsDead = Math.Abs(_healthPoints) < FloatTolerance;
            
            PlayerTookDamage.Raise();

            if (IsDead)
                PlayerDied.Raise();
        }
    }

    [SerializeField, ReadOnly]private int _shieldPoints;
    public int ShieldPoints { 
        get => _shieldPoints;
        set
        {
            _shieldPoints = Mathf.Clamp(value, 0, MaxShieldPoints);
            PlayerTookDamage.Raise();
        } 
    }

    [SerializeField, ReadOnly] private float _movementSpeed;

    public float MovementSpeed
    {
        get { return _movementSpeed; }
        set { _movementSpeed = value; }
    }

    public void Initialize()
    {
        HealthPoints = MaxHealthPoints;
        ShieldPoints = 0;
        MovementSpeed = InitialMovementSpeed;
    }
    
    private void OnEnable()
    {
        PlayerDied.AddListener(this, Kill);
    }
    
    private void OnDisable()
    {
        PlayerDied.RemoveListener(this, Kill);
    }

    public bool IsDead { get; set; }

    private void Kill()
    {
        IsDead = true;
    }
    
} 
