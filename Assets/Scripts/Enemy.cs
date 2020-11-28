using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Enemy : MonoBehaviour, IDestructible
{
    private Transform _transform;
    
    [SerializeField] private float _speed = .8f;

    [SerializeField, Space] private SceneDataScriptable _sceneData;
    
    private Vector4 bounds = new Vector4(5.5f, 8.3f, -3.4f, -8.3f);
        
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

    [Inject]
    private void Construction(SceneDataScriptable sceneData)
    {
        _sceneData = sceneData;
    }
    
    void Awake()
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
        var destructible = other.GetComponent<IDestructible>();
        destructible?.Destroy();

        var player = other.GetComponent<Player>();
        if (player)
        {
            Destroy();
        }
        
        var harmable = other.GetComponent<IHarmable>();
        harmable?.Damage();
        
        var bullet = other.GetComponent<IBullet>();
        if (bullet?.BulletType.name == "LaserBullet")
        {
            Destroy();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    public void Destroy()
    {
        gameObject.SetActive(false);
    }
}
