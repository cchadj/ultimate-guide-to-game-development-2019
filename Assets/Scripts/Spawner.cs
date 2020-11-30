using System;
using System.Collections;
using System.Collections.Generic;
using GD.MinMaxSlider;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ObjectPooler))]
public class Spawner : MonoBehaviour
{
    #region Set by unity

    [SerializeField, MinMaxSlider(0.1f, 40)]
    private Vector2 _spawnEverySeconds;
    
    [SerializeField, Space] private GameStateScriptable _gameState;
    
    [SerializeField] private PlayerStateScriptable _playerState;
    
    #endregion

    private const float DefaultSpawnEverySeconds = 2f;
    private ObjectPooler _enemyPooler;
    private bool _shouldStopSpawning;

    private const float MinXBound = -8.3f, MaxXBound = 8.3f;

    [Inject]
    private void Constructor(GameStateScriptable gameState, PlayerStateScriptable playerState)
    {
        _gameState = gameState;
        _playerState = playerState;
    }
    
    private void Awake()
    {
        _enemyPooler = GetComponent<ObjectPooler>();
    }

    private void Start()
    {
        StartSpawning();
    }

    private void OnEnable()
    {
        _playerState.PlayerDied += StopSpawning;
    }

    private void OnDisable()
    {
        _playerState.PlayerDied -= StopSpawning;
    }

    private void StartSpawning()
    {
        _shouldStopSpawning = false;
        StartCoroutine(SpawnCoroutine());
    }
    
    private void StopSpawning()
    {
        _shouldStopSpawning = true;
    }

    private void SpawnEnemy()
    {
        var enemy = _enemyPooler.NextPoolableObject;
        var enemyTransform = enemy.transform;
        
        enemy.Activate();
        enemyTransform.position = new Vector3( Mathf.Lerp(MinXBound, MaxXBound, Random.value), 6, 0);
    }
    
    
    private IEnumerator SpawnCoroutine()
    {
        while (!_shouldStopSpawning)
        {
            var seconds = Mathf.Lerp(_spawnEverySeconds[0], _spawnEverySeconds[1], Random.value);
            yield return new WaitForSeconds(seconds);
            
            var canSpawn = !_enemyPooler.IsEmpty && !_shouldStopSpawning;
            
            if (canSpawn)
            {
                SpawnEnemy();
            }
        }

        yield return null;
    }
}
