using UnityEditor;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(ZenjectObjectPoolerContext), typeof(ZenjectObjectPoolerInstaller))]
public partial class ZenjectObjectPooler : ObjectPooler
{
    private PoolableMonobehaviour.Factory _poolableMonobehaviourFactory;

    [Inject]
    private void InjectDependencies(PoolableMonobehaviour.Factory factory)
    {
        _poolableMonobehaviourFactory = factory; 
    }
    
    protected override PoolableMonobehaviour CreateNewPrefab(Transform container)
    {
        var newPrefab = _poolableMonobehaviourFactory.Create();
        newPrefab.transform.parent = container;
        return newPrefab;
    }
}

#if UNITY_EDITOR
public partial class ZenjectObjectPooler : ObjectPooler
{
    [CustomEditor(typeof(ZenjectObjectPooler))]
    public class ZenjectObjectPoolerEditor : Editor
    {
        public ZenjectObjectPooler Pooler => target as ZenjectObjectPooler;
        private ZenjectObjectPoolerInstaller _installer;
        private ZenjectObjectPoolerContext _context;
        private void OnEnable()
        {
            _installer = Pooler.GetComponent<ZenjectObjectPoolerInstaller>();
            _context = Pooler.GetComponent<ZenjectObjectPoolerContext>();
            _context.AddMonobehaviourInstaller(_installer);
        }

        public override void OnInspectorGUI()
        {
            var poolableMonoBehaviourPrefab = serializedObject.FindProperty(nameof(ZenjectObjectPooler._poolableMonobehaviourPrefab));
            EditorGUILayout.PropertyField(poolableMonoBehaviourPrefab);

            if (_installer != null && _installer.PoolableMonobehaviourPrefab != Pooler._poolableMonobehaviourPrefab)
                _installer.PoolableMonobehaviourPrefab = Pooler._poolableMonobehaviourPrefab;
            
            var container = serializedObject.FindProperty(nameof(ZenjectObjectPooler._container));
            EditorGUILayout.PropertyField(container);
            
            var initialCapacity = serializedObject.FindProperty(nameof(ZenjectObjectPooler._initialPoolSize));
            EditorGUILayout.PropertyField(initialCapacity);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif