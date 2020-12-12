using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public interface IConstructor
{
    bool IsConstructed { get; }

    void Constructor();
}

public interface IStarter
{
    bool IsStarter { get; }
    void Start();
}

public interface IDestructor
{
    bool IsDestructed { get; }

    void Destructor();
}


public class ScriptableObjectLifecycleAttacher : MonoBehaviour
{
    [field:SerializeField, DragAndDropPath()] public string ScriptableObjectFolder { get; private set; }
    
    public static T[] GetAllInstances<T>(string folder) where T : ScriptableObject
    {
        var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name, new []{ folder }); //FindAssets uses tags check documentation for more info
        var a = new T[guids.Length];
        for (var i = 0; i < guids.Length; i++) //probably could get optimized 
        {
            var path = AssetDatabase.GUIDToAssetPath(guids[i]);
            a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
        }

        return a;
    }

    private List<ScriptableObject> _scriptableObjects;
    [SerializeField, ReadOnly] private List<ScriptableObject> _startable;
    [SerializeField, ReadOnly] private List<ScriptableObject> _constructable;
    [SerializeField, ReadOnly] private List<ScriptableObject> _destructable;
    
    private List<IConstructor> _constructorScriptableObjects;
    private List<IDestructor> _destructorScriptableObjects;
    private List<IStarter> _startableScriptableObjects;

    void Awake()
    {
        _scriptableObjects = GetAllInstances<ScriptableObject>(ScriptableObjectFolder).Where(m => m is IConstructor).ToList();
        
        _constructorScriptableObjects = _scriptableObjects.Where(so => so is IConstructor).ToList().OfType<IConstructor>().ToList();
        _destructorScriptableObjects = _scriptableObjects.Where(so => so is IDestructor).ToList().OfType<IDestructor>().ToList();
        _startableScriptableObjects = _scriptableObjects.Where(so => so is IStarter).ToList().OfType<IStarter>().ToList();

        _constructable = _constructorScriptableObjects.OfType<ScriptableObject>().ToList();
        _destructable = _destructorScriptableObjects.OfType<ScriptableObject>().ToList();
        _startable = _startableScriptableObjects.OfType<ScriptableObject>().ToList();

        foreach (var constructable in _constructorScriptableObjects)
        {
            constructable.Constructor();
        }
    }

    private void Start()
    {
        foreach (var startable in _startableScriptableObjects)
        {
            startable.Start();
        }
    }

    private void OnDestroy()
    {
        foreach (var destructable in _destructorScriptableObjects)
        {
            destructable.Destructor();
        }

        _scriptableObjects = null;
    }
}
