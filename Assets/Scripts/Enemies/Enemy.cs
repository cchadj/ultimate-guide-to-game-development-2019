using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public partial class Enemy : PoolableMonobehaviour, IDestructible
{

    #region SetByUnity
    [SerializeField] private float _speed = .8f;

    [SerializeField, MiniView] private FloatVariable _damageAmount;

    [SerializeField] private EnumEntry _enemyType;

    [SerializeField] private GameEvent OnExplosion;
    
    [SerializeField, ReadOnly, Space(10)] private GameStateScriptable _gameState;
    
    [SerializeField, ReadOnly] private SceneDataScriptable _sceneData;

    [SerializeField, ReadOnly] private EnemyAnimationController _animationController;
    
    #endregion

    #region CachedObjects
    private Collider2D _collider2D;
    
    private Transform _transform;
    
    #endregion

    private int _enemyLayerIndex;
    private int _playerLayerIndex;
    
    private void Awake()
    {
        _transform = transform;
        _animationController = GetComponent<EnemyAnimationController>();
        _collider2D = GetComponent<Collider2D>();

        _enemyLayerIndex  = LayerMask.NameToLayer("Enemy");
        _playerLayerIndex = LayerMask.NameToLayer("Friendly");
    }

    [Inject]
    private void InjectDependencies(GameStateScriptable gameState, SceneDataScriptable sceneData)
    {
        _gameState = gameState;
        _sceneData = sceneData;
    }
    
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
        // Don't interact with other enemies or enemy bullets
        var isEnemyObject = other.gameObject.layer == _enemyLayerIndex;
        if (isEnemyObject) return;

        // Don't interact with anything that's not a player for now.
        var isPlayerObject = other.gameObject.layer == _playerLayerIndex;
        if (!isPlayerObject)  return;
        
        other.GetComponent<IHarmable>()?.Damage((int)_damageAmount.Value);
        
        if (other.GetComponent<Player>())
            Destroy();
        
        var bullet = other.GetComponent<IBullet>();
        
        if (bullet == null)
           return;

        other.GetComponent<IDestructible>()?.Destroy();
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

    protected override void OnEnable()
    {
        base.OnEnable();
        OnExplosion.AddOwner(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnExplosion.RemoveOwner(this);
    }

    public void Destroy()
    {
        _collider2D.enabled = false;
        _animationController.PlayDeathAnimation();
        _gameState.EnemyDestroyed.Raise(_enemyType);
        OnExplosion.Raise(this);
        StartCoroutine(Destroy(2));
    }

    private IEnumerator Destroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }

    public override void Reset()
    {
        base.Reset();
        _collider2D.enabled = true;
        _animationController.Reset();
    }
}
