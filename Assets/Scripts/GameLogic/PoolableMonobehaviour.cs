using System;
using UnityEngine;
using Zenject;
using Zenject.Asteroids;

public partial class PoolableMonobehaviour : MonoBehaviour
{
    public event Action OnDisableEvent;
    public event Action OnEnableEvent;

    protected Vector3 _cachedInitialLocalPosition;
    
    public virtual void Reset()
    {
        gameObject.transform.localPosition = _cachedInitialLocalPosition;
    }
    
    protected virtual void OnEnable() => OnEnableEvent?.Invoke();

    protected virtual void OnDisable() => OnDisableEvent?.Invoke();
    
    private void Awake()
    {
        _cachedInitialLocalPosition = transform.localPosition;
        gameObject.SetActive(false);
        OnDisableEvent += Reset;
    }


    public void Activate()
    {
        gameObject.SetActive(true);
    }
    
    public void Deactivate()
    {
        Reset();
        gameObject.SetActive(false);
    }
}

public partial class PoolableMonobehaviour : MonoBehaviour
{
    public class Factory : PlaceholderFactory<PoolableMonobehaviour> {}
}
