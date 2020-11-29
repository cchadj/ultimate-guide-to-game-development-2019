using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ObjectPooler : MonoBehaviour
{
    #region MyRegion

    [SerializeField] private PoolableMonobehaviour _poolableMonobehaviourPrefab;
    [SerializeField] private int _initialPoolSize;
    [SerializeField, Tooltip("Gameobject that will contain pooled objects")] private Transform _container;
    
    #endregion

    #region Defaults

    private const int DefaultInitialPoolSize = 500;

    #endregion
    
    private Queue<PoolableMonobehaviour> _availableObjectsPool;
    private Queue<PoolableMonobehaviour> _activeObjectsPool;

    public PoolableMonobehaviour NextPoolableObject
    {
        get
        {
            var obj =  _availableObjectsPool.Dequeue();   
            _activeObjectsPool.Enqueue(obj);
            

            return obj;
        }
    }
    
    public bool IsEmpty => _availableObjectsPool.Count == 0;

    public int CurrentPoolSize { get; private set; }

    private void Awake()
    {
        if (_initialPoolSize == 0)
            _initialPoolSize = DefaultInitialPoolSize;
        
        CurrentPoolSize = _initialPoolSize;

        if (_container == null)
            _container = transform;
        
       _availableObjectsPool = new Queue<PoolableMonobehaviour>(); 
       _activeObjectsPool = new Queue<PoolableMonobehaviour>();
    }

    protected virtual void Start()
    {
        for (var i = 0; i < _initialPoolSize; i++)
        {
            var instantiatedObject = Instantiate(_poolableMonobehaviourPrefab, _container, true);
            _availableObjectsPool.Enqueue(instantiatedObject);
            
            instantiatedObject.OnDestroyEvent += () => MakeObjectAvailable(instantiatedObject);
            instantiatedObject.OnEnableEvent += () => MakeObjectActive(instantiatedObject);
        }
    }

    public void ExpandPool(int poolSize)
    {
        if (poolSize <= CurrentPoolSize)
            return;
        
        var expandAmount = poolSize - CurrentPoolSize;
        for (var i = 0; i < expandAmount; i++)
        {
            var instantiatedObject = Instantiate(_poolableMonobehaviourPrefab, _container, true);
            _availableObjectsPool.Enqueue(instantiatedObject);
            
            instantiatedObject.OnDestroyEvent += () => MakeObjectAvailable(instantiatedObject);
            instantiatedObject.OnEnableEvent += () => MakeObjectActive(instantiatedObject);
        }

        CurrentPoolSize = poolSize;
    }
    
    private void MakeObjectActive(PoolableMonobehaviour pooledObject)
    {
        pooledObject.gameObject.SetActive(true);
        _activeObjectsPool.Enqueue(pooledObject);
    }


    private bool _lock;
    private void MakeObjectAvailable(PoolableMonobehaviour pooledObject)
    {
//        pooledObject.transform.SetParent(_transform);
//        complains when changing parent the same frame when being deactivated
        pooledObject.gameObject.SetActive(false);
        _availableObjectsPool.Enqueue(pooledObject);
    }
}
