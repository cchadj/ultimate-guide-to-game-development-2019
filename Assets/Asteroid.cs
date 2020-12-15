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
        AsteroidDestroyed.AddListener(this, Destroy);
    }

    private void OnDisable()
    {
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
                AsteroidDestroyed.Raise();
                break;
            case BulletType.TripleShot:
                AsteroidDestroyed.Raise();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void Destroy()
    {
        _collider2D.enabled = false;
        _animationController.PlayDestructAnimation();
        OnExplosion.Raise();
        StartCoroutine(Destroy(2));
    }
    
    private IEnumerator Destroy(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        GameObject.Destroy(gameObject);
    }
}
