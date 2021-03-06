﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

[Flags]
internal enum OutOfBoundsDirection
{
    None=0, 
    Up=1,
    Right=2,
    Left=4,
    Down=8
}

public interface IHarmable
{
    void Damage(int amount=1);
}

[DisallowMultipleComponent]
public class Player : MonoBehaviour, Controls.IPlayerActions, IHarmable
{
    #region SerialisedFields
    [SerializeField] private Controls _controls;
    
    [SerializeField] private ObjectPooler _laserPooler;
    
    [SerializeField] private ObjectPooler _tripleShotLaserPooler;
    
    [SerializeField] private float _laserCooldown;

    [SerializeField] private GameEvent _playerFired;
    
    [SerializeField] private GameEvent _PlayerPickedPowerup;

    [SerializeField, ReadOnly, Space] private SceneDataScriptable _sceneData;
    
    [SerializeField, ReadOnly] private BulletType _currentBulletType;
    
    [SerializeField, ReadOnly] private PlayerStateScriptable _playerState;
    
    [SerializeField, ReadOnly] private PlayerThrusterAnimationController _thrusterAnimationController;
    
    #endregion SerialisedFields

    #region GameObject Components
    
    private Transform _transform;
    
    #endregion GameObject Components

    private int _friendlyLayerIndex;
    
    private int _enemyLayerIndex;

    private const BulletType DefaultBulletType = BulletType.Laser;
    
    private Dictionary<BulletType, ObjectPooler> _bulletPoolers;
    
    private const float LaserSpawnOffset = .8f;
    
    private readonly Vector3 _direction2D = new Vector3(1, 1, 0);
    
    // Input direction received from player
    private Vector2 _inputDirection = Vector2.zero;

    private float _timeSinceLastLaser;

    private OutOfBoundsDirection OutOfBounds
    {
        get
        {
            var direction = OutOfBoundsDirection.None;
            if (_transform.position.y - _transform.localScale.y> _sceneData.TopBound)
                direction |= OutOfBoundsDirection.Up;
            if (_transform.position.x - _transform.localScale.x> _sceneData.RightBound)
                direction |= OutOfBoundsDirection.Right;
            if (_transform.position.y + _transform.localScale.y < _sceneData.BottomBound)
                direction |= OutOfBoundsDirection.Down;
            if (_transform.position.x + _transform.localScale.x < _sceneData.LeftBound)
                direction |= OutOfBoundsDirection.Left;
            return direction;
        }
    }

    [Inject]
    private void Constructor(
        PlayerStateScriptable playerState,
        GameStateScriptable gameState,
        SceneDataScriptable sceneData,
        PlayerThrusterAnimationController thrusterAnimationController
        )
    {
        _playerState = playerState; 
        _sceneData = sceneData;
        _thrusterAnimationController = thrusterAnimationController;
    }

    private GameObject _gameObject;
    private void Awake()
    {
        _gameObject = gameObject;
        if (_gameObject == null)
        {
            print("_gameObject is null");
            _gameObject = (GameObject)FindObjectOfType(typeof(GameObject));
        }

        _currentBulletType = DefaultBulletType;
        _bulletPoolers = new Dictionary<BulletType, ObjectPooler>
        {
            [BulletType.Laser] = _laserPooler,
            [BulletType.TripleShot] = _tripleShotLaserPooler
        };

        _transform = GetComponent<Transform>();
        
        _controls = new Controls();
        _controls.Player.SetCallbacks(this);

        _friendlyLayerIndex = LayerMask.NameToLayer("Friendly");
        
        _enemyLayerIndex = LayerMask.NameToLayer("Enemy");
    }

    private void OnEnable()
    {
        _controls?.Enable(); 
        _playerState.PlayerDied.AddListener(this, Destroy);
        _playerState.PlayerPickedShield.AddListener(this, PickShield);
        _playerState.PlayerPickedSpeedBoost.AddListener(this, PickSpeedboost);
        _playerState.PlayerPickedTripleShot.AddListener(this, PickTripleShot);
        
        _playerFired.AddOwner(this);
        _playerState.PlayerPickedShield.AddOwner(this);
        _playerState.PlayerPickedSpeedBoost.AddOwner(this);
        _playerState.PlayerPickedTripleShot.AddOwner(this);
        _PlayerPickedPowerup.AddOwner(this);
        
        // Player can immediately shoot
        _timeSinceLastLaser = _laserCooldown + .1f;
    }
    
    private void OnDisable()
    {
        _controls?.Disable();
        _playerState.PlayerDied.RemoveListeners(this);
        _playerState.PlayerPickedShield.RemoveListeners(this);
        _playerState.PlayerPickedSpeedBoost.RemoveListeners(this);
        _playerState.PlayerPickedTripleShot.RemoveListeners(this);
        
        _playerFired.RemoveOwner(this);
        _playerState.PlayerPickedShield.RemoveOwner(this);
        _playerState.PlayerPickedSpeedBoost.RemoveOwner(this);
        _playerState.PlayerPickedTripleShot.RemoveOwner(this);
        _PlayerPickedPowerup.RemoveOwner(this);
    }

    private void PickTripleShot()
    {
        _currentBulletType = BulletType.TripleShot;
    }

    private void PickSpeedboost()
    {
        Speedup(_playerState.SpeedupMultiplier, 3);
    }

    private void PickShield()
    {    
        _playerState.ShieldPoints++;
    }

    private void Start()
    {
        _laserCooldown = Math.Abs(_laserCooldown) < .01f ? .8f : _laserCooldown;
    }
    
    private void Update()
    {
        CalculateMovement();
        
        _timeSinceLastLaser += Time.deltaTime;
    }
    
    private void CalculateMovement()
    {
        var displacement = GetMovementDirection() * _playerState.MovementSpeed * Time.deltaTime;
        _transform.Translate(displacement);
        
        if (displacement.Equals(Vector3.zero))
        {
            _thrusterAnimationController.StopThrusters();
        }
        else
        {
           _thrusterAnimationController.StartThrusters(); 
        }
            
        if (OutOfBounds != OutOfBoundsDirection.None)
        {
            HandleOutOfBounds();
        }
    }


    private Vector3 GetMovementDirection()
    {
        return _direction2D * _inputDirection;
    }

    private void HandleOutOfBounds()
    {
        if (OutOfBounds.HasFlag(OutOfBoundsDirection.Up))
        {
            _transform.position = new Vector3(_transform.position.x, _sceneData.BottomBound - _transform.localScale.y, 0.0f);
        }

        if (OutOfBounds.HasFlag(OutOfBoundsDirection.Right))
        {
            _transform.position = new Vector3(_sceneData.LeftBound - _transform.localScale.x, _transform.position.y, 0.0f);
        }

        if (OutOfBounds.HasFlag(OutOfBoundsDirection.Down))
        {
            _transform.position = new Vector3(_transform.position.x, _sceneData.TopBound + _transform.localScale.y, 0.0f);
        }

        if (OutOfBounds.HasFlag(OutOfBoundsDirection.Left))
        {
            _transform.position = new Vector3(_sceneData.RightBound + _transform.localScale.x, _transform.position.y, 0.0f);
        }
    }
    
    private void FireBullet()
    {
        var bulletPooler = _bulletPoolers[_currentBulletType];
        var canFire = (_timeSinceLastLaser >= _laserCooldown) && !bulletPooler.IsEmpty;

        if (!canFire) return;
        
        var bullet = bulletPooler.NextPoolableObject;
        bullet.transform.SetPositionAndRotation(_transform.position + Vector3.up * LaserSpawnOffset, Quaternion.identity);
        bullet.gameObject.SetActive(true);
            
        _timeSinceLastLaser = .0f;
        
        _playerFired.Raise(this);
    }

    #region Input Handling 
    public void OnMove(InputAction.CallbackContext context)
    {
        _inputDirection = context.ReadValue<Vector2>();
    }

    public void OnRestart(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
            FireBullet();
    }

    public void OnExitGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    #endregion Input Handling

    public void Destroy()
    {
        gameObject.SetActive(false);
    }

    public void Damage(int amount=1)
    {
        var damageAmount = ScriptableObject.CreateInstance<FloatVariable>();
        damageAmount.Value = amount;
        _playerState.PlayerTookDamage.Raise(damageAmount);
    }

    public void Collect(GameObject o)
    {    
        o.GetComponent<ICollectible>()?.Collect();

        var powerUp = o.GetComponent <Powerup>();
        if (!powerUp) return;
        
        switch (powerUp.PowerupType)
        {
            case PowerupType.Speedboost:
                _playerState.PlayerPickedSpeedBoost.Raise(this);
                break;
            case PowerupType.TripleShot:
                _playerState.PlayerPickedTripleShot.Raise(this);
                SetBulletType(BulletType.TripleShot, 5);
                break;
            case PowerupType.Shield:
                _playerState.PlayerPickedShield.Raise(this);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _PlayerPickedPowerup.Raise(this);
    }

    private Coroutine _currentSpeedupCoroutine;
    private void Speedup(float multiplier, float seconds)
    {
        if (_currentSpeedupCoroutine != null)
            StopCoroutine(_currentSpeedupCoroutine);
        _currentSpeedupCoroutine = StartCoroutine(SpeedupCoroutine(multiplier, seconds));
    }

    private Coroutine _currentBulletTypeCoroutine;


    private void SetBulletType(BulletType bulletType, float seconds)
    {
        if (_currentBulletTypeCoroutine != null)
            StopCoroutine(SetBulletTypeCoroutine(bulletType, seconds));
        _currentBulletTypeCoroutine = StartCoroutine(SetBulletTypeCoroutine(bulletType, seconds));
    }

    private IEnumerator SpeedupCoroutine(float multiplier, float seconds)
    {
        _playerState.MovementSpeed = _playerState.InitialMovementSpeed * multiplier;
        yield return new WaitForSeconds(seconds);
        _playerState.MovementSpeed = _playerState.InitialMovementSpeed;
    }
    
    private IEnumerator SetBulletTypeCoroutine(BulletType bulletType, float seconds)
    {
        _currentBulletType = bulletType;
        yield return new WaitForSeconds(seconds);
        _currentBulletType = DefaultBulletType;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only interact with enemy objects for now.
        var isEnemyObject = other.gameObject.layer == _enemyLayerIndex;
        if (!isEnemyObject) return;
        
        // Only Interact with enemy bullets. Enemy collision with player is handled with Enemy script.
        var bullet = other.GetComponent<IBullet>();
        if (bullet == null) return;
        
        Damage((int)bullet.BulletDamage);
        var destructible = other.GetComponent<IDestructible>();
        destructible?.Destroy();
    }
}
