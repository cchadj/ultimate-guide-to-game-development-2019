using UnityEngine;
using Zenject;

/// <summary>
/// Spawns an object at a random position at the top of the screen. 
/// </summary>
public class RandomTopBoundSpawner : Spawner
{
    [SerializeField, ReadOnly] private SceneDataScriptable _sceneData;
    [Inject]
    private void InjectDependencies(SceneDataScriptable sceneData)
    {
        _sceneData = sceneData;
    }

    protected override PoolableMonobehaviour Spawn(ObjectPooler pooler)
    {
        var pooledObject =  base.Spawn(pooler);
        
        // Spawn object at a random position at the top bound of the screen.
        var randomXposition = Mathf.Lerp(_sceneData.LeftBound, _sceneData.RightBound, Random.value);
        pooledObject.transform.position = new Vector3(randomXposition, _sceneData.TopBound, 0);
        
        return pooledObject;
    }
}