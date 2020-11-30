using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Object = UnityEngine.Object;

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
    void Damage();
}

[DisallowMultipleComponent]
public class Player : MonoBehaviour, Controls.IPlayerActions, IHarmable
{
    #region SerialisedFields
    [SerializeField] private Controls _controls;
    
    [SerializeField] private ObjectPooler _laserPooler;
    
    [SerializeField] private ObjectPooler _tripleShotLaserPooler;
    
    [SerializeField] private float _movementSpeed;
    
    [SerializeField] private float _laserCooldown;

    [SerializeField, Space] private SceneDataScriptable _sceneData;
    
    [SerializeField] private PlayerStateScriptable _playerState;
    
    [SerializeField] private BulletType _currentBulletType;
    #endregion SerialisedFields

    #region GameObject Components
    
    private Transform _transform;
    
    #endregion GameObject Components

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
    private void Constructor(PlayerStateScriptable playerState, SceneDataScriptable sceneData)
    {
        _playerState = playerState; 
        _sceneData = sceneData;
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

        _bulletPoolers = new Dictionary<BulletType, ObjectPooler>
        {
            [BulletType.Laser] = _laserPooler,
            [BulletType.LaserTripleShot] = _tripleShotLaserPooler
        };

        _transform = GetComponent<Transform>();
        
        _playerState.PlayerDied += Destroy;
        _playerState.HealthPoints = _playerState.PlayerMaxHealth;
        
        _controls = new Controls();
        _controls.Player.SetCallbacks(this);
    }

    private void OnEnable()
    {
        _timeSinceLastLaser = .0f;
        _controls.Enable();
    }

    private void OnDisable()
    {
        _playerState.PlayerDied -= Destroy;
        _controls.Disable();
    }

    private void Start()
    {
        _transform.position = new Vector3(0, 0, 0);
        
        _movementSpeed = Math.Abs(_movementSpeed) < .01f ? 1.5f : _movementSpeed;
        _laserCooldown = Math.Abs(_laserCooldown) < .01f ? .8f : _laserCooldown;
    }
    
    private void Update()
    {
        CalculateMovement();
        
        _timeSinceLastLaser += Time.deltaTime;
    }
    
    private void CalculateMovement()
    {
        _transform.Translate(GetMovementDirection() * _movementSpeed * Time.deltaTime);
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
        
        if (canFire)
        {
            
            var bullet = bulletPooler.NextPoolableObject;
            bullet.transform.SetPositionAndRotation(_transform.position + Vector3.up * LaserSpawnOffset, Quaternion.identity);
            bullet.gameObject.SetActive(true);
            
            _timeSinceLastLaser = .0f;
        }
    }


    #region Input Handling 
    public void OnMove(InputAction.CallbackContext context)
    {
        _inputDirection = context.ReadValue<Vector2>();
    }
    
    public void OnLook(InputAction.CallbackContext context)
    {
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
            FireBullet();
    }
    #endregion Input Handling

    [ContextMenu("Destroy")]
    public void Destroy()
    {
        gameObject.SetActive(false);
    }

    public void Damage()
    {
        _playerState.PlayerTookDamage?.Invoke();

        if (_playerState.ShieldPoints > 0)
            _playerState.ShieldPoints--;
        else
            _playerState.HealthPoints--;

        if (_playerState.HealthPoints <= 0)
        {
            _playerState.IsPlayerDead = true;
            _playerState.PlayerDied?.Invoke();
        }
    }

    public void Collect(GameObject o)
    {    
        o.GetComponent<ICollectible>()?.Collect();

        var powerUp = o.GetComponent <Powerup>();
        if (!powerUp) return;
        
        switch (powerUp.PowerupType)
        {
            case PowerupType.Speedboost:
                Speedup(2, 3); 
                break;
            case PowerupType.TripleShot:
                SetBulletType(BulletType.LaserTripleShot, 5);
                break;
            case PowerupType.Shield:
                if (_playerState.ShieldPoints == 0)
                    _playerState.ShieldPoints++;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Speedup(float multiplier, float seconds)
    {
        StartCoroutine(SpeedupCoroutine(multiplier, seconds));
    }
    
    private void SetBulletType(BulletType bulletType, float seconds)
    {
       StartCoroutine(SetBulletTypeCoroutine(bulletType, seconds));
    }

    private IEnumerator SpeedupCoroutine(float multiplier, float seconds)
    {
        var initialSpeed = _movementSpeed;
        _movementSpeed *= multiplier;
        yield return new WaitForSeconds(seconds);
        _movementSpeed = initialSpeed;
    }
    
    private IEnumerator SetBulletTypeCoroutine(BulletType bulletType, float seconds)
    {
        _currentBulletType = bulletType;
        yield return new WaitForSeconds(seconds);
        _currentBulletType = DefaultBulletType;
    }
    
}
