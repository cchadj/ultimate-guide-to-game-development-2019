using System;
using System.Collections;
using UnityEngine;

public class Asteroid : MonoBehaviour, IDestructible
{
    [SerializeField] private float _rotationsPerSecond = .45f;
    
    [SerializeField] public GameEvent AsteroidDestroyed;
    
    [SerializeField] public GameEvent OnExplosion;
    
    #region Cached
    private AsteroidAnimationController _animationController;

    private Collider2D _collider2D;
    #endregion
    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _animationController = GetComponent <AsteroidAnimationController>();
    }

    private void Start()
    {
        _animationController.SetRotationsPerSecond(_rotationsPerSecond);
    }
    
    private void OnEnable()
    {
        OnExplosion.AddOwner(this);
        AsteroidDestroyed.AddOwner(this);
        AsteroidDestroyed.AddListener(this, Destroy);
    }

    private void OnDisable()
    {
        OnExplosion.RemoveOwner(this);
        AsteroidDestroyed.RemoveOwner(this);
        AsteroidDestroyed.RemoveListener(this, Destroy);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        var bullet = other.GetComponent<IBullet>();

        if (bullet == null) return;
        
        other.GetComponent<IDestructible>()?.Destroy();

        switch (bullet.BulletType)
        {
            case BulletType.Laser:
                AsteroidDestroyed.Raise(this);
                break;
            case BulletType.TripleShot:
                AsteroidDestroyed.Raise(this);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Destroy()
    {
        _collider2D.enabled = false;
        _animationController.PlayDestructAnimation();
        OnExplosion.Raise(this);
        StartCoroutine(Destroy(2));
    }
    
    private IEnumerator Destroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameObject.Destroy(gameObject);
    }
}
