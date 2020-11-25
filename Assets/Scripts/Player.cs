using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

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
    private Transform _transform;
    private PlayerInput _playerInput;
    
    [SerializeField] private Controls _controls;
    
    [SerializeField] private float _movementSpeed;

    [NonSerialized] public float Horizontal;

    private readonly Vector3 _direction2D = new Vector3(1, 1, 0);
    private Vector2 _inputDirection = Vector2.zero;

    private Vector4 bounds = new Vector4(5.5f, 8.3f, -3.4f, -8.3f);
    
    private void Awake()
    {
        _playerInput = GetComponent<PlayerInput>();
        _transform = GetComponent<Transform>();
        
        _controls = new Controls();
        _controls.Player.SetCallbacks(this);
    }

    private void OnEnable()
    {
        _controls.Enable();
    }

    private void OnDisable()
    {
        _controls.Disable();
    }

    private void Start()
    {
        _transform.position = new Vector3(0, 0, 0);
        
        _movementSpeed = Math.Abs(_movementSpeed) < 0.01f ? 1.5f : this._movementSpeed;
    }
    
    private void Update()
    {
        Move();
    }

    private Vector3 GetMovementDirection()
    {
        return _direction2D * _inputDirection;
    }

    private void Wrap()
    {
        throw new NotImplementedException();
    }


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

            return direction;}
    }

    private void Move()
    {
        _transform.Translate(GetMovementDirection() * _movementSpeed * Time.deltaTime);
        if (OutOfBounds != OutOfBoundsDirection.None)
        {
            HandleOutOfBounds();
        }
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
    }
    #endregion Input Handling
}
