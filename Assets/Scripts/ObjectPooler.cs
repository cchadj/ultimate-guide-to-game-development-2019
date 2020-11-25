using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private PoolableMonobehaviour _poolableMonobehaviourPrefab;
    [SerializeField] private int _poolSize = 15;
    
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
    
    private void Awake()
    {
       _availableObjectsPool = new Queue<PoolableMonobehaviour>(_poolSize); 
       _activeObjectsPool = new Queue<PoolableMonobehaviour>(_poolSize);
    }

    private void Start()
    {
        for (var i = 0; i < _poolSize; i++)
        {
            var instantiatedObject = Instantiate(_poolableMonobehaviourPrefab, transform, true);
            _availableObjectsPool.Enqueue(instantiatedObject);
            
            instantiatedObject.OnDestroyEvent += () => MakeObjectAvailable(instantiatedObject);
            instantiatedObject.OnEnableEvent += () => MakeObjectActive(instantiatedObject);
        }
    }

    private void MakeObjectActive(PoolableMonobehaviour pooledObject)
    {
        Debug.Log("Object Activated");
        pooledObject.gameObject.SetActive(true);
        _activeObjectsPool.Enqueue(pooledObject);
    }

    private void MakeObjectAvailable(PoolableMonobehaviour pooledObject)
    {
        Debug.Log("Object Available");
        pooledObject.gameObject.SetActive(false);
        _availableObjectsPool.Enqueue(pooledObject);
    }
}
