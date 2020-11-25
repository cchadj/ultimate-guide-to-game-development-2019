using System;
using UnityEngine;

public class PoolableMonobehaviour : MonoBehaviour
{
    public event Action OnDestroyEvent;
    public event Action OnEnableEvent;

    private void Awake()
    {
        gameObject.SetActive(false); 
    }

    protected virtual void OnEnable() => OnEnableEvent?.Invoke();

    protected virtual void OnDisable() => OnDestroyEvent?.Invoke();
}
