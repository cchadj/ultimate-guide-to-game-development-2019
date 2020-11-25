using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private float _spawnRate = 0.333f;
    private ObjectPooler _enemyPooler;

    private const float MinXBound = -8.3f, MaxXBound = 8.3f;
    private void Awake()
    {
        _enemyPooler = GetComponent<ObjectPooler>();
    }

    private void SpawnEnemy()
    {
        var enemy = _enemyPooler.NextPoolableObject;
        var enemyTransform = enemy.transform;
        
        enemy.gameObject.SetActive(true);
        enemyTransform.position = new Vector3( Mathf.Lerp(MinXBound, MaxXBound, Random.value), 6, 0);
    }
    
    IEnumerator Start()
    {
        while (true)
        {
            yield return new WaitForSeconds(1.0f / _spawnRate);
            if (!_enemyPooler.IsEmpty)
            {
                SpawnEnemy();
            }
        }
    }
}
