using System;
using UnityEngine;

public interface ICollectible
{
    void Collect();
}

public class Powerup : MonoBehaviour, ICollectible
{
    [field:SerializeField] public PowerupType PowerupType { get; private set; }
    
    [SerializeField] private float _speed = DefaultSpeed;

    [SerializeField, Space] private SceneDataScriptable _sceneData;

    private Transform _transform;
    
    private const float DefaultSpeed = 3;
        
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
        if (_speed <= Math.Abs(0.01f))
        {
            _speed = DefaultSpeed;
        }
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
        if (OutOfBounds.HasFlag(OutOfBoundsDirection.Down))
        {
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        CalculateMovement();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<Player>();
        if (player != null)
        {
            player.Collect(gameObject);
        }
    }

    public void Collect()
    {
        gameObject.SetActive(false);
    }
}
