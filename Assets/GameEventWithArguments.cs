using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public delegate void GameEventDelegateWithArguments<in T>(T arguments) where T: ScriptableObject;
public class ScriptableEventArgs : EventArgs
{
    private ScriptableObject Arguments { get; }

    public T GetArguments<T>() where T : ScriptableObject
    {
        var data = (object) Arguments;
        if (data is T) {
            return (T)data;
        } 
        try {
           return (T)Convert.ChangeType(data, typeof(T));
        } 
        catch (InvalidCastException) {
           return default(T);
        }
    }
    
    public ScriptableEventArgs(ScriptableObject arguments) => Arguments = arguments;
}

[CreateAssetMenu(menuName = "Event System / Game Event With Arguments")]
public partial class GameEventWithArguments : ScriptableObject
{
    [field:SerializeField] public string EventName { get; private set; }

    [SerializeField, MiniView] private ScriptableObject _mockArgumentsScriptable;

    public ScriptableObject MockArgumentsScriptable => _mockArgumentsScriptable;
    
    public event EventHandler<ScriptableEventArgs> Event;
    
    private Dictionary<object, Dictionary<object, EventHandler<ScriptableEventArgs>>> _scriptableEventHandlers =
        new Dictionary<object, Dictionary<object, EventHandler<ScriptableEventArgs>>>();

    [ContextMenu("Raise Event")]
    private void Raise()
    {
        Event?.Invoke(this, new ScriptableEventArgs(MockArgumentsScriptable));
    }

    public void Raise(ScriptableObject so)
    {
        Event?.Invoke(this, new ScriptableEventArgs(so));
    }
    
    /// Adds a listener function to this event.
    /// Returns a handle that can be used to remove listener (i.e gameEvent.Event -= handler)
    public EventHandler<ScriptableEventArgs> AddListener<T>(object subscriberObject, GameEventDelegateWithArguments<T> eventDelegate) where T: ScriptableObject
    {
        var isExpectedTypeDifferentFromPassedType = typeof(T) != MockArgumentsScriptable.GetType();
        if (isExpectedTypeDifferentFromPassedType)
            throw new ArgumentException($"Listener '{subscriberObject}' of type <{subscriberObject.GetType()}> passed type <{typeof(T)}> " +
                                        $"when expected type is <{MockArgumentsScriptable.GetType()}>." +
                                        $" \n Please change subscriber type in AddListener<{typeof(T)}>() to AddListener<{MockArgumentsScriptable.GetType()}>()");
        
        var handler = new EventHandler<ScriptableEventArgs>((o, e) => eventDelegate(e.GetArguments<T>()));
        Event += handler;
        
        if (!_scriptableEventHandlers.ContainsKey(subscriberObject))
        {
            _scriptableEventHandlers[subscriberObject] = new Dictionary<object, EventHandler<ScriptableEventArgs>>();
        }
        
        _scriptableEventHandlers[subscriberObject][eventDelegate] = handler;
        return handler;
    }
    
    // Remove the handler from the event. Warning, the event must have been added with AddListener to be
    // able to be removed.
    public void RemoveListener<T>(object subscriberObject, GameEventDelegateWithArguments<T> eventDelegate) where T: ScriptableObject
    {
        var isExpectedTypeDifferentFromPassedType = typeof(T) != MockArgumentsScriptable.GetType();
        if (isExpectedTypeDifferentFromPassedType)
            throw new ArgumentException($"Listener '{subscriberObject}' of type <{subscriberObject.GetType()}> passed type <{typeof(T)}> " +
                                        $"when expected type is <{MockArgumentsScriptable.GetType()}>." +
                                        $" \n Please change subscriber type in RemoveListener<{typeof(T)}>() to RemoveListener<{MockArgumentsScriptable.GetType()}>() to successfully " +
                                        $"remove listener.");
        if (!_scriptableEventHandlers.ContainsKey(subscriberObject)) return;

        var delegateToHandlerDict = _scriptableEventHandlers[subscriberObject]; 
        var handlerToRemove = delegateToHandlerDict[eventDelegate];
        Event -= handlerToRemove;
    }

    public void RemoveListeners(object subscriberObject)
    {
        if (!_scriptableEventHandlers.ContainsKey(subscriberObject)) return;

        var subscriberHandlers = _scriptableEventHandlers[subscriberObject];
        foreach (var delegateHandlerPair in subscriberHandlers)
        {
            var handler = delegateHandlerPair.Value;
            Event -= handler;
        }
        
        _scriptableEventHandlers[subscriberObject].Clear();
        _scriptableEventHandlers.Remove(subscriberObject);
    }

    private void Reset()
    {
        _scriptableEventHandlers = new Dictionary<object, Dictionary<object, EventHandler<ScriptableEventArgs>>>();
        
        if (Event == null) return;
        
        foreach (var d in Event.GetInvocationList())
        {
            Event -= d as EventHandler<ScriptableEventArgs>;
        }
    }
}


#if UNITY_EDITOR
public partial class GameEventWithArguments
{
    /// <summary>
    /// Gets all children of `SerializedProperty` at 1 level depth.
    /// </summary>
    /// <param name="serializedProperty">Parent `SerializedProperty`.</param>
    /// <returns>Collection of `SerializedProperty` children.</returns>
    public static IEnumerable<SerializedProperty> GetChildren(SerializedProperty serializedProperty)
    {
        var currentProperty = serializedProperty.Copy();
        var nextSiblingProperty = serializedProperty.Copy();
        {
            nextSiblingProperty.Next(false);
        }
 
        if (currentProperty.Next(true))
        {
            do
            {
                if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                    break;
 
                yield return currentProperty;
            }
            while (currentProperty.Next(false));
        }
    }
 
    /// <summary>
    /// Gets visible children of `SerializedProperty` at 1 level depth.
    /// </summary>
    /// <param name="serializedProperty">Parent `SerializedProperty`.</param>
    /// <returns>Collection of `SerializedProperty` children.</returns>
    public static IEnumerable<SerializedProperty> GetVisibleChildren(SerializedProperty serializedProperty)
    {
        var currentProperty = serializedProperty.Copy();
        var nextSiblingProperty = serializedProperty.Copy();
        {
            nextSiblingProperty.NextVisible(false);
        }
 
        if (currentProperty.NextVisible(true))
        {
            do
            {
                if (SerializedProperty.EqualContents(currentProperty, nextSiblingProperty))
                    break;
 
                yield return currentProperty;
            }
            while (currentProperty.NextVisible(false));
        }
    }
    
    [CustomEditor(typeof(GameEventWithArguments))]
    private class GameEventWithArgumentsEditor : Editor
    {
        private GameEventWithArguments Target { get => target as GameEventWithArguments;}

        public override void OnInspectorGUI()
        {
            Target.EventName = EditorGUILayout.DelayedTextField(new GUIContent("Event Name"), Target.EventName);
            if (string.IsNullOrWhiteSpace(Target.EventName))
                Target.EventName = Target.name;
            
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(Target._mockArgumentsScriptable)));
            if (Target.MockArgumentsScriptable == null)
            {
                EditorGUILayout.HelpBox("Please set a mock ScriptableObject based object for testing and " +
                                        " to set the data type of the arguments. (Runtime error will be thrown if subscriber" +
                                        " subscribes with wrong data type).", MessageType.Warning);
            }

            serializedObject.ApplyModifiedProperties();
            
            if (Target.MockArgumentsScriptable == null) return;
            
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("Raise Event"))
            {
                Debug.Log($"Raising event {Target.EventName} from {Target.name} with mock arguments from {Target.MockArgumentsScriptable}");
                Target.Raise();
            }
        }
    }
}
#endif
