using System;
using UnityEngine;

public class PoolableMonobehaviour : MonoBehaviour
{
    public event Action OnDestroyEvent;
    public event Action OnEnableEvent;
    
    protected virtual void OnEnable() => OnEnableEvent?.Invoke();

    protected virtual void OnDisable() => OnDestroyEvent?.Invoke();

    private Vector3 _cachedInitialLocalPosition;
    
    private void Awake()
    {
        _cachedInitialLocalPosition = transform.localPosition;
        gameObject.SetActive(false); 
        OnDestroyEvent += () => transform.localPosition = _cachedInitialLocalPosition;
    }


    public void Activate()
    {
        gameObject.SetActive(true);
    }
    
    public void Deactivate()
    {
        gameObject.transform.localPosition = _cachedInitialLocalPosition;
        gameObject.SetActive(false);
    }
}
