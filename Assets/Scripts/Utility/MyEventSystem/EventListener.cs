using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ModestTree;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class EventListener : MonoBehaviour
{
    [SerializeField] private ScriptableObject _eventObject;
    private ScriptableObject EventObject
    {
        get => _eventObject;
        set
        {
            var valueChanged = _eventObject != value;
            _eventObject = value;
            if (valueChanged)
                CacheEventObjectEvents();
        }
    }

    [SerializeField]private GameObject _listenerGameObject;
    private GameObject ListenerGameObject
    {
        get => _listenerGameObject;
        set
        {
            var valueChanged = _listenerGameObject != value; 
            _listenerGameObject= value;
            if (valueChanged)
            {
                CacheListenerComponents();
            }
        }
    }
    
    [SerializeField, HideInInspector] private int _selectedEventIndex;
    
    [SerializeField, HideInInspector] private int _selectedComponentIndex;
    private int SelectedComponentIndex
    {
        get => _selectedComponentIndex;
        set
        {
            var valueChanged = _selectedComponentIndex != value;
            _selectedComponentIndex = value;
            if (valueChanged)
            {
                CacheMethods();
            }
        }
    }

    [SerializeField, HideInInspector] private List<int> _selectedMethodIndices;

    public bool IsListenerObjectSet => _listenerGameObject != null;
    public bool IsEventObjectSet => EventObject != null;
    
    // Event Information
    private List<EventInfo> _eventInfos  = new List<EventInfo>(); 
    
    private List<string> _eventNames = new List<string>();
    public string[] EventObjectEventNames => _eventNames.ToArray();
    
    // Listener component information
    private Component[] _components;
    
    private List<string> _componentNames = new List<string>();
    
    public string[] ListenerComponentNames => _componentNames.ToArray();

    // Component's Methods
    private List<MethodInfo> _methodInfos = new List<MethodInfo>();

    private List<string> _methodNames = new List<string>();
    public string[] MethodNames => _methodNames.ToArray();

    private void Awake()
    {
        // Order must be preserved
        CacheEventObjectEvents();
        CacheListenerComponents();
        CacheMethods();
    }

    private void OnEnable()
    {
        Subscribe();
    }
    
    private void OnDisable()
    {
        Unsubscribe();
    }

    private EventInfo SelectedEvent => _eventInfos[_selectedEventIndex];
    private Component SelectedComponent => _components[_selectedComponentIndex];

    private IEnumerable<MethodInfo> SelectedMethods
    {
        get
        {
            var selectedMethods = new List<MethodInfo>();
            foreach (var i in _selectedMethodIndices)
            {
                selectedMethods.Add(_methodInfos[i]);
            }
            return selectedMethods;
        }
    }
    private Delegate _currentHandler;

    public void Subscribe()
    {
        var method = GetType().GetMethod("CallSelectedMethods");
        
        _currentHandler = Delegate.CreateDelegate(SelectedEvent.EventHandlerType, this, method);
        SelectedEvent.AddEventHandler(EventObject, _currentHandler);
    }

    public void CallSelectedMethods(object sender, EventArgs eventArgs)
    {
        foreach (var method in SelectedMethods)
        {
            method.Invoke(SelectedComponent, new object[] {});
        }
    }

    private void Unsubscribe()
    {
        SelectedEvent.RemoveEventHandler(EventObject, _currentHandler);
    }

    [ContextMenu("Raise Event")]
    public void RaiseEvent()
    {
       var eventDelegate = (MulticastDelegate)EventObject.GetType().GetField(SelectedEvent.Name,
           BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(EventObject);
       
       if (eventDelegate == null) return;
       
       foreach (var handler in eventDelegate.GetInvocationList())
       {
           handler.Method.Invoke(handler.Target, new object[] { EventObject, EventArgs.Empty });
       }
    }
    
    public void CacheListenerComponents()
    {
        if (_listenerGameObject == null) return;
        
        _components = _listenerGameObject.GetComponents<Component>();

        _componentNames = new List<string>();
        foreach (var component in _components)
        {
            _componentNames.Add(component.GetType().Name);
        }
        
    }
    
    public List<EventInfo> CacheEventObjectEvents()
    {
        if (EventObject == null)
            return null;
                
        var eventObjectType = EventObject.GetType();
        var eventInfos = eventObjectType.GetEvents();
        
        _eventNames = new List<string>();
        _eventInfos = new List<EventInfo>();
        foreach (var eventInfo in eventInfos)
        {
            _eventInfos.Add(eventInfo);
            _eventNames.Add(eventInfo.Name);
        }

        return _eventInfos;
    }
    
    public void CacheMethods()
    {
        _methodNames = new List<string>();
        _methodInfos = new List<MethodInfo>();
        
        var component = _components[_selectedComponentIndex];

        var typeMethods = component.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public &
                        ~BindingFlags.GetProperty & ~BindingFlags.SetProperty)
            .Where(m => !m.IsSpecialName && m.GetParameters().Length == 0 && !m.Name.Contains("Get") &&
                        !m.Name.Contains("get") && !m.Name.Contains("_"));


        foreach (var methodInfo in typeMethods)
        {
            _methodInfos.Add(methodInfo);
            _methodNames.Add(methodInfo.Name);
        }
    }

    public void TestMeVoid()
    {
        print("TestMeVoid called");
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EventListener))]
    public class SomeEditor : Editor
    {
        private int _eventIndex;
        private int _componentIndex;
        private int _methodIndex;

        private EventListener Target => (EventListener) target;
        
        public override void OnInspectorGUI()
        {
            Target.EventObject = EditorGUILayout.ObjectField("Event Scriptable Object",
                Target.EventObject, typeof(ScriptableObject), true) as ScriptableObject;
            
            Target.ListenerGameObject = EditorGUILayout.ObjectField(label:"Listener Game Object", Target.ListenerGameObject,
                typeof(GameObject), true) as GameObject;
            
            if (!Target.IsListenerObjectSet || !Target.IsEventObjectSet)
                return;
            
            _eventIndex = Target._selectedEventIndex;
            _componentIndex = Target._selectedComponentIndex;
            if (Target.ListenerComponentNames == null || Target.ListenerComponentNames.IsEmpty())
            {
                Target.CacheListenerComponents();
            }
            
            if (Target.ListenerComponentNames.Length > 1)
            {
                EditorGUILayout.Space();

                if (Target.EventObjectEventNames.IsEmpty())
                {
                    var events = Target.CacheEventObjectEvents();

                    if (events != null && events.Count == 0)
                    {
                       EditorGUILayout.LabelField("No suitable events found in this object.");
                       return;
                    }
                }
                
                EditorGUILayout.LabelField("Select event to listen to:");
                _eventIndex = EditorGUILayout.Popup(_eventIndex, Target.EventObjectEventNames);
                Target._selectedEventIndex = _eventIndex;
                
                EditorGUILayout.LabelField("Select listeners component:");
                Target.SelectedComponentIndex = EditorGUILayout.Popup(Target.SelectedComponentIndex, Target.ListenerComponentNames);

                var selectedMethodIndicesListProperty = serializedObject.FindProperty(nameof(Target._selectedMethodIndices));
                Show(selectedMethodIndicesListProperty);
            }

            // Save the changes back to the object
            EditorUtility.SetDirty(target);
        }

        public void Show(SerializedProperty list)
        {
            EditorGUILayout.LabelField("Select listener methods:");
            EditorGUI.indentLevel += 1;
            list.isExpanded = true;
            Target.CacheMethods();
            if (list.isExpanded)
            {
                for (var i = 0; i < list.arraySize; i++)
                {
                    Target._selectedMethodIndices[i] = EditorGUILayout.Popup(Target._selectedMethodIndices[i], Target.MethodNames);
                }
            }

            if (GUILayout.Button("+"))
            {
                Target._selectedMethodIndices.Add(0);
            }

            if (GUILayout.Button("-") && Target._selectedMethodIndices.Count >= 1)
            {
                Target._selectedMethodIndices.RemoveAt(Target._selectedMethodIndices.Count - 1);
            }
        }
    }
#endif
    
}

