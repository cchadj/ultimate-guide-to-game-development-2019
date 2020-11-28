using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemySpawner : MonoBehaviour
{
    #region Set by unity
    
    [SerializeField] private float _spawnRate = 0.333f;
    
    
    [SerializeField, Space] private GameStateScriptable _gameState;
    
    [SerializeField] private PlayerStateScriptable _playerState;
    
    #endregion 
    
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
        _playerState.PlayerDied += () => _shouldStopSpawning = true;
    }

    private void SpawnEnemy()
    {
        var enemy = _enemyPooler.NextPoolableObject;
        var enemyTransform = enemy.transform;
        
        enemy.Activate();
        enemyTransform.position = new Vector3( Mathf.Lerp(MinXBound, MaxXBound, Random.value), 6, 0);
    }
    
    private void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }

    
    private IEnumerator SpawnCoroutine()
    {
        while (!_shouldStopSpawning)
        {
            yield return new WaitForSeconds(1.0f / _spawnRate);
            var canSpawn = !_enemyPooler.IsEmpty && !_shouldStopSpawning;
            
            if (canSpawn)
            {
                SpawnEnemy();
            }
        }

        yield return null;
    }
}
