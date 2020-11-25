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
    
public class Player : MonoBehaviour, Controls.IPlayerActions
{
    [SerializeField] private Controls _controls;
    
    [SerializeField]private GameObject _laserPrefab;
    
    [SerializeField] private float _movementSpeed;

    private Transform _transform;
    private PlayerInput _playerInput;
    
    
    // Input direction received from player
    private Vector2 _inputDirection = Vector2.zero;

    // The bounds of the screen, up, right, down, left (clockwise starting from up)
    private Vector4 bounds = new Vector4(5.5f, 8.3f, -3.4f, -8.3f);

    private const float LaserSpawnOffset = .8f;

    private readonly Vector3 _direction2D = new Vector3(1, 1, 0);
    private float _timeSinceLastLaser;
    [SerializeField] private float _laserCooldown;

    private OutOfBoundsDirection OutOfBounds
    {
        get
        {
            var direction = OutOfBoundsDirection.None;
            
            if (_transform.position.y - _transform.localScale.y> bounds[0])
                direction |= OutOfBoundsDirection.Up;
            if (_transform.position.x - _transform.localScale.x> bounds[1])
                direction |= OutOfBoundsDirection.Right;
            if (_transform.position.y + _transform.localScale.y < bounds[2])
                direction |= OutOfBoundsDirection.Down;
            if (_transform.position.x + _transform.localScale.x < bounds[3])
                direction |= OutOfBoundsDirection.Left;

            return direction;
            
        }
    }
    
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
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
            _transform.position = new Vector3(_transform.position.x, bounds[2] - _transform.localScale.y, 0.0f);
        }

        if (OutOfBounds.HasFlag(OutOfBoundsDirection.Right))
        {
            _transform.position = new Vector3(bounds[3] - _transform.localScale.x, _transform.position.y, 0.0f);
        }

        if (OutOfBounds.HasFlag(OutOfBoundsDirection.Down))
        {
            _transform.position = new Vector3(_transform.position.x, bounds[0] + _transform.localScale.y, 0.0f);
        }

        if (OutOfBounds.HasFlag(OutOfBoundsDirection.Left))
        {
            _transform.position = new Vector3(bounds[1] + _transform.localScale.x, _transform.position.y, 0.0f);
        }
    }
    
    private void FireBullet()
    {
        var canFire = _timeSinceLastLaser >= _laserCooldown;
        
        if (canFire)
        {
            Instantiate(_laserPrefab, _transform.position + Vector3.up * LaserSpawnOffset, Quaternion.identity);
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
        print("HELLO");
        if (context.performed)
            FireBullet();
    }
    #endregion Input Handling
}
