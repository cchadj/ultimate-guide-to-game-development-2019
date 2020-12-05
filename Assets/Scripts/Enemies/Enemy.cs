using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public partial class Enemy : MonoBehaviour, IDestructible
{
    private Transform _transform;
    
    [SerializeField] private float _speed = .8f;

    [SerializeField, Space] private SceneDataScriptable _sceneData;
        
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
        _transform = transform;
    }

    private void CalculateMovement()
    {
        _transform.Translate(new Vector3(0, -_speed * Time.deltaTime, 0));
        if (OutOfBounds != OutOfBoundsDirection.None)
        {
            HandleOutOfBounds();
        }
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
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<IDestructible>()?.Destroy();

        other.GetComponent<IHarmable>()?.Damage();
        
        if (other.GetComponent<Player>())
            Destroy();
        
        var bullet = other.GetComponent<IBullet>();
        if (bullet == null)
           return;
        
        switch (bullet.BulletType)
        {
            case BulletType.Laser:
                Destroy();
                break;
            case BulletType.TripleShot:
                Destroy();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Update()
    {
        CalculateMovement();
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
    }
}
