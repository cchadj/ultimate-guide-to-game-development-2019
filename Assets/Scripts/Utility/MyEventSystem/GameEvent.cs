using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void GameEventDelegate();

[CreateAssetMenu(menuName = "Event System / Game Event")]
public partial class GameEvent : ScriptableObject
{
    public string EventName;

    public event EventHandler Event;

    // Hashes the handlers for each listener object.
    // Used with AddListener and RemoveListener.
    private Dictionary<object, Dictionary<GameEventDelegate, EventHandler>> _eventHandlers =
        new Dictionary<object, Dictionary<GameEventDelegate, EventHandler>>();

    [ContextMenu("Raise Event")]
    public void Raise()
    {
            Event?.Invoke(this, EventArgs.Empty);
    }

    /// Adds a listener function to this event.
    /// Returns a handle that can be used to remove listener (i.e gameEvent.Event -= handler)
    public EventHandler AddListener(object subscriberObject, GameEventDelegate eventDelegate)
    {
        var handler = new EventHandler((o, e) => eventDelegate());
        Event += handler;
        
        if (!_eventHandlers.ContainsKey(subscriberObject))
        {
            _eventHandlers[subscriberObject] = new Dictionary<GameEventDelegate, EventHandler>();
        }
        
        _eventHandlers[subscriberObject][eventDelegate] = handler;
        return handler;
    }

    // Remove the handler from the event. Warning, the event must have been added with AddListener to be
    // able to be removed.
    public void RemoveListener(object subscriberObject, GameEventDelegate eventDelegate)
    {
        if (!_eventHandlers.ContainsKey(subscriberObject)) return;

        var delegateToHandlerDict = _eventHandlers[subscriberObject]; 
        var handlerToRemove = delegateToHandlerDict[eventDelegate];
        Event -= handlerToRemove;
    }

    public void RemoveListeners(object subscriberObject)
    {
        if (!_eventHandlers.ContainsKey(subscriberObject)) return;

        var subscriberHandlers = _eventHandlers[subscriberObject];
        foreach (var delegateHandlerPair in subscriberHandlers)
        {
            var handler = delegateHandlerPair.Value;
            Event -= handler;
        }
        
        _eventHandlers[subscriberObject].Clear();
        _eventHandlers.Remove(subscriberObject);
    }

    private void Reset()
    {
        _eventHandlers = new Dictionary<object, Dictionary<GameEventDelegate, EventHandler>>();
        
        if (Event == null) return;
        
        foreach (var d in Event.GetInvocationList())
        {
            Event -= d as EventHandler;
        }
    }

#if UNITY_EDITOR
#endif
}

//    [ContextMenu("Enable")]
//    private void OnEnable()
//    {
//        if (SelectedEvent == null) return;
//        Event += RaiseLinkedEvent;
//        SubscribeToLinkedEvent();
//    }
//    
//    [ContextMenu("Disable")]
//    private void OnDisable()
//    {
//        if (SelectedEvent == null) return;
//        Event -= RaiseLinkedEvent;
//        UnsubscribeFromLinkedEvent();
//    }
//
//    private void RaiseLinkedEvent(object sender, EventArgs e)
//    {
//       if (SelectedEvent == null) return;
//        
//       var eventDelegate = (MulticastDelegate)EventObject.GetType().GetField(SelectedEvent.Name,
//           BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(EventObject);
//
//       if (eventDelegate == null) return;
//       
//       foreach (var handler in eventDelegate.GetInvocationList())
//       {
//           handler.Method.Invoke(handler.Target, new object[] { EventObject, EventArgs.Empty });
//       }
//    }
//
//    private Delegate _currentHandler;
//    public void CallRaise(object sender, EventArgs e)
//    {
//       Raise(); 
//    }
//    private void SubscribeToLinkedEvent()
//    {
//        var method = GetType().GetMethod(nameof(CallRaise));
//        _currentHandler = Delegate.CreateDelegate(SelectedEvent.EventHandlerType, this, method);
//        SelectedEvent.AddEventHandler(EventObject, _currentHandler);
//    }
//    
//    private void UnsubscribeFromLinkedEvent()
//    {
//        SelectedEvent.RemoveEventHandler(EventObject, _currentHandler);
//    }
//    
//
//    [SerializeField] private ScriptableObject _eventObject;
//    private ScriptableObject EventObject
//    {
//        get => _eventObject;
//        set
//        {
//            var valueChanged = _eventObject != value;
//            _eventObject = value;
//            if (valueChanged)
//                CacheEventObjectEvents();
//        }
//    }
//    
//    public List<EventInfo> CacheEventObjectEvents()
//    {
//        if (EventObject == null)
//            return null;
//                
//        var eventObjectType = EventObject.GetType();
//        var eventInfos = eventObjectType.GetEvents();
//        
//        _eventNames = new List<string>();
//        _eventInfos = new List<EventInfo>();
//        foreach (var eventInfo in eventInfos)
//        {
//            _eventInfos.Add(eventInfo);
//            _eventNames.Add(eventInfo.Name);
//        }
//
//        return _eventInfos;
//    }
//    
//    private List<EventInfo> _eventInfos  = new List<EventInfo>(); 
//    
//    private List<string> _eventNames = new List<string>();
//    public string[] EventObjectEventNames => _eventNames.ToArray();
//     
//    private EventInfo SelectedEvent
//    {
//        get
//        {
//            if (_eventInfos != null && _eventInfos.Count == 0)
//                _eventInfos = CacheEventObjectEvents();
//            
//            if (_eventInfos != null && _eventInfos.Count == 0)
//                return null;
//            
//            return _eventInfos?[_selectedEventIndex];
//        }
//    }
//
//    [SerializeField, HideInInspector] private int _selectedEventIndex;
//    
//#if UNITY_EDITOR
//    [CustomEditor(typeof(GameEvent))]
//    public class GameEventEditor : Editor
//    {
//        private GameEvent Target => (GameEvent) target;
//        
//        private SerializedProperty _eventNameProperty;
//        
//        void OnEnable()
//        {
//            _eventNameProperty = serializedObject.FindProperty(nameof(Target.EventName));
//        }
//        
//        public override void OnInspectorGUI()
//        {
//            EditorGUILayout.PropertyField(_eventNameProperty, new GUIContent("Event Name (Optional)"));
//            
//            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
//            EditorGUILayout.LabelField("(Optional) Select an existing event to link to.");
//            Target.EventObject = EditorGUILayout.ObjectField("Event Scriptable Object",
//                Target.EventObject, typeof(ScriptableObject), true) as ScriptableObject;
//            
//            if (Target.EventObject)
//            {
//                if (Target.EventObjectEventNames.Length != 0)
//                    Target._selectedEventIndex = EditorGUILayout.Popup(Target._selectedEventIndex, Target.EventObjectEventNames);
//                else
//                    EditorGUILayout.HelpBox("No suitable events found for this object", MessageType.Warning);
//            }
//        }
//    }
//
//#endif
//    public void Initialize()
//    {
//       Debug.Log("Am I ever called?" + name); 
//    }
