using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
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

public class Player : MonoBehaviour, Controls.IPlayerActions, IDestructible
{
    #region SerialisedFields
    [SerializeField] private Controls _controls;
    
    [SerializeField]private GameObject _laserPrefab;
    
    [SerializeField] private float _movementSpeed;
    
    [SerializeField] private SceneDataScriptable _sceneData;
    
    [SerializeField] private float _laserCooldown;
    #endregion SerialisedFields

    #region GameObject Components
    private Transform _transform;
    
    private ObjectPooler _laserPooler;
    #endregion GameObject Components

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
    
    private void Awake()
    {
        _laserPooler = GetComponent<ObjectPooler>();
        _transform = GetComponent<Transform>();
        
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
        var canFire = (_timeSinceLastLaser >= _laserCooldown) && !_laserPooler.IsEmpty;
        
        if (canFire)
        {
            var laserBullet = _laserPooler.NextPoolableObject;
            laserBullet.transform.parent = null;
            laserBullet.transform.SetPositionAndRotation(_transform.position + Vector3.up * LaserSpawnOffset, Quaternion.identity);
            laserBullet.gameObject.SetActive(true);
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

    public void Destroy()
    {
        GameObject.Destroy(gameObject);
    }
}
