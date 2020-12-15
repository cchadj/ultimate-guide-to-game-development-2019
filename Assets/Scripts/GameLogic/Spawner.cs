using System;
using System.Collections;
using System.Collections.Generic;
using GD.MinMaxSlider;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    #region Set by unity

    [SerializeField] private List<ObjectPooler> _objectPoolers;
    
    [SerializeField, MinMaxSlider(0.1f, 40)]
    private Vector2 _spawnEverySeconds;

    [SerializeField] private bool _isSpawning;
    public bool IsSpawning
    {
        get => _isSpawning;
        set => _isSpawning = value;
    }

    #endregion

    protected virtual void Awake()
    {
        var areObjectPoolersProvided = _objectPoolers.Count > 0;
        if (areObjectPoolersProvided) return;
        
        var pooler = GetComponent<ObjectPooler>();
        var noObjectPoolerComponent = pooler == null;
        
        if (noObjectPoolerComponent)
        {
            throw new Exception("No object ObjectPoolers provided  and no ObjectPoolers components in this object.");
        }
        _objectPoolers.Add(pooler);

    }

    private void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }

    private Coroutine _spawnCoroutine;
    public void StartSpawning()
    {
//        if (_spawnCoroutine != null)
//            StopCoroutine(_spawnCoroutine);
        
        _isSpawning = true;
        
//        _spawnCoroutine = StartCoroutine(SpawnCoroutine());
    }
    
    public void StopSpawning()
    {
//        if (_spawnCoroutine != null)
//            StopCoroutine(_spawnCoroutine);
        
        _isSpawning = false;
    }

    protected virtual PoolableMonobehaviour Spawn(ObjectPooler pooler)
    {
        var pooledObject = pooler.NextPoolableObject;
        pooledObject.Activate();
        return pooledObject;
    }
    
    
    private IEnumerator SpawnCoroutine()
    {
        while (true)
        {    
            var seconds = Mathf.Lerp(_spawnEverySeconds[0], _spawnEverySeconds[1], Random.value);
            if (_isSpawning)
            {
                var randomPooler = _objectPoolers[Random.Range(0, _objectPoolers.Count)];
                var canSpawn = !randomPooler.IsEmpty;
                
                if (canSpawn)
                {
                    Spawn(randomPooler);
                }
            }
            yield return new WaitForSeconds(seconds);
        }
    }
}