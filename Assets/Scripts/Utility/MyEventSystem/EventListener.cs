using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ModestTree;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
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
    [field:SerializeField, HideInInspector] public string SelectedComponentName { get; private set; }
    private int SelectedComponentIndex
    {
        get => _selectedComponentIndex;
        set
        {
            var valueChanged = _selectedComponentIndex != value;
            _selectedComponentIndex = value;
            if (valueChanged)
            {
                CacheListenerComponents();
                CacheMethods();
            }
            SelectedComponentName = _componentNames[_selectedComponentIndex];
        }
    }

    [SerializeField, HideInInspector] private List<int> _selectedMethodIndices;

    private int[] SelectedMethodIndices
    {
        get => _selectedMethodIndices.ToArray();
        set
        {
            var noChange = _selectedMethodIndices.All(value.Contains) &&
                           _selectedMethodIndices.Count == value.Length;

            if (noChange)
                return;
            
            _selectedMethodIndices = new List<int>(value);
            _selectedMethodNames = new List<string>(); 
            foreach (var i in _selectedMethodIndices)
            {
                var methodInfo = _methodInfos[i];
                _selectedMethodNames.Add(methodInfo.Name);
            }
        }
    }

    private void CacheSelectedMethodNames()
    {
        Cache();
        _selectedMethodNames = new List<string>();
        foreach (var i in _selectedMethodIndices)
        {
            var methodInfo = _methodInfos[i];
            _selectedMethodNames.Add(methodInfo.Name);
        }
    }
    
    public bool IsListenerObjectSet => _listenerGameObject != null;
    public bool IsEventObjectSet => EventObject != null;
    
    // Event Information
    private List<EventInfo> _eventInfos;
    private List<object> _eventOwners;

    private List<string> _eventNames;
    public string[] EventObjectEventNames => _eventNames?.ToArray();
    
    // Listener component information
    private Component[] _components;
    private Component[] Components
    {
        get {
            if (_components == null) 
                CacheListenerComponents();
            return _components; 
        }
        set => _components = value;
    }

    private List<string> _componentNames;
    
    public string[] ListenerComponentNames => _componentNames.ToArray();

    // Component's Methods
    private List<MethodInfo> _methodInfos;

    private List<string> _methodNames;
    public string[] MethodNames => _methodNames.ToArray();

    private void Awake()
    {
        Cache();
    }

    [ContextMenu("Cache Everything")]
    public void Cache()
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

    private EventInfo SelectedEvent
    {
        get
        {
            var areEventsCached = _eventInfos != null && _eventInfos.Count != 0;
            
            if (areEventsCached) 
                return _eventInfos[_selectedEventIndex];

            CacheEventObjectEvents();
            
            var areNoEventsFound = _eventInfos.Count == 0; 
            
            return areNoEventsFound ? null : _eventInfos[_selectedEventIndex];
        }
    }
    private object SelectedEventOwner => _eventOwners?[_selectedEventIndex];

    private Component SelectedComponent => Components?[SelectedComponentIndex];

    private List<Action> _selectedMethods;

    [SerializeField, HideInInspector] private List<string> _selectedMethodNames;
    public List<string> SelectedMethodNames => _selectedMethodNames;

    private IEnumerable<Action> SelectedComponentMethods
    {
        get
        {
            var isSelectedMethodsNotInitialised = _selectedMethods == null || _selectedMethods.Count == 0;
            
            if (!isSelectedMethodsNotInitialised) return _selectedMethods;
            
            _selectedMethods = new List<Action>(); 
            foreach (var i in _selectedMethodIndices)
            {
                var methodInfo = _methodInfos[i];
                var methodsInstance = SelectedComponent;
                if(methodInfo.IsStatic)
                    methodsInstance = null;
                var del = (Action)Delegate.CreateDelegate(typeof(Action), methodsInstance, methodInfo);
                _selectedMethods.Add(del);
            }

            return _selectedMethods;
        }
    }
    private Delegate _currentHandler;

    public void Subscribe()
    {
        var method = GetType().GetMethod(nameof(CallSelectedMethods));

        System.Diagnostics.Debug.Assert(method != null, nameof(method) + " != null");
        _currentHandler = Delegate.CreateDelegate(SelectedEvent.EventHandlerType, this, method);
        SelectedEvent.AddEventHandler(SelectedEventOwner, _currentHandler);
    }

    public void CallSelectedMethods(object sender, EventArgs eventArgs)
    {
        foreach (var method in SelectedComponentMethods)
        {
            // This is slightly slower than a normal method call 
            method();
        }
    }

    [ContextMenu("PerformanceTest")]
    private void PerformanceTest()
    {
        const int repeats = 50000;
        print($"Performing test for {repeats} times");
        print("Call through delegates");
        var sw = new Stopwatch();
        sw.Start();
        for (var i = 0; i <= repeats; i++)
            CallSelectedMethods(null, null); 
        sw.Stop();
        Debug.Log("Elapsed={0}" + sw.Elapsed);

        print("Direct call.");
        var eventTester = GetComponent <MyEventsTester>();
        sw = new Stopwatch();
        sw.Start();
        for (var i = 0; i <= repeats; i++)
            eventTester.TestNoOp();
        sw.Stop();
        Debug.Log("Elapsed={0}" + sw.Elapsed);
        
        print("Call through raise event.");
        sw = new Stopwatch();
        sw.Start();
        for (var i = 0; i <= repeats; i++)
            eventTester.RaisePerformanceEvent();
        sw.Stop();
        Debug.Log("Elapsed={0}" + sw.Elapsed);
    }
    
    private void Unsubscribe()
    {
        SelectedEvent.RemoveEventHandler(SelectedEventOwner, _currentHandler);
    }

    [ContextMenu("Raise Event")]
    private void RaiseEvent()
    {
       // This function is (relatively) slow because it uses reflection on runtime but is only used for testing.
       // Should not be used for testing!
       var eventDelegate = (MulticastDelegate)SelectedEventOwner.GetType().GetField(SelectedEvent.Name,
           BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(SelectedEventOwner);
       
       if (eventDelegate == null) return;
       
       var methodArguments = new[] {SelectedEventOwner, EventArgs.Empty};
       foreach (var handler in eventDelegate.GetInvocationList())
       {
           handler.Method.Invoke(handler.Target, methodArguments);
       }
    }
    
    public void CacheListenerComponents()
    {
        _selectedMethods = null;
        if (_listenerGameObject == null) return;
        
        Components = _listenerGameObject.GetComponents<Component>();

        _componentNames = new List<string>();
        foreach (var component in Components)
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
        _eventOwners = new List<object>();
        foreach (var eventInfo in eventInfos)
        {
            _eventOwners.Add(_eventObject);
            _eventInfos.Add(eventInfo);
            _eventNames.Add(eventInfo.Name);
        }

        var eventFields = eventObjectType.GetFields().Where(field => field.FieldType == typeof(GameEvent));
        var gameEventFields = eventFields;
        foreach (var gameEventField in gameEventFields)
        {
            _eventOwners.Add(gameEventField.GetValue(_eventObject));
            _eventInfos.Add(gameEventField.FieldType.GetEvents()[0]);
            _eventNames.Add(gameEventField.Name);
        }

        return _eventInfos;
    }
    
    public void CacheMethods()
    {
        _methodNames = new List<string>();
        _methodInfos = new List<MethodInfo>();
        _selectedMethods = null;
        
        var typeMethods = SelectedComponent.GetType()
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
    public class EventListenerEditor : Editor
    {
        private EventListener Target => (EventListener) target;
        private List<int> _onEnabledSelectedMethodIndices;

        private void OnEnable()
        {
            Target.Cache();
            _onEnabledSelectedMethodIndices = new List<int>();
            for (var i = 0; i < Target.SelectedMethodIndices.Length; i++)
            {
                var selectedIndex = 0;
                if (i < Target.SelectedMethodNames.Count)
                {
                    var methodName = Target.SelectedMethodNames?[i];
                    selectedIndex = Target.MethodNames.IndexOf(methodName);
                }
                else
                {
                    selectedIndex = -1;
                }
                if (selectedIndex <0)
                    continue;
                _onEnabledSelectedMethodIndices.Add(selectedIndex);
            }

            Target.SelectedMethodIndices = _onEnabledSelectedMethodIndices.ToArray();
        }

        public override void OnInspectorGUI()
        {
            Target.EventObject = EditorGUILayout.ObjectField("Event Scriptable Object",
                Target.EventObject, typeof(ScriptableObject), true) as ScriptableObject;
            
            Target.ListenerGameObject = EditorGUILayout.ObjectField(label:"Listener Game Object", Target.ListenerGameObject,
                typeof(GameObject), true) as GameObject;
            
            
            if (!Target.IsListenerObjectSet || !Target.IsEventObjectSet)
                return;

            var isListenerComponentsNotInitialised = Target.ListenerComponentNames == null || Target.ListenerComponentNames.IsEmpty();  
            if (isListenerComponentsNotInitialised)
                Target.CacheListenerComponents();

            var noListenerComponentsFoundExceptTransform = Target.ListenerComponentNames.Length <= 1; 
            if (noListenerComponentsFoundExceptTransform)
                return;
            
            EditorGUILayout.Space();

            var areEventsNotInitialised = Target.EventObjectEventNames.IsEmpty();
            if (areEventsNotInitialised)
            {
                var events = Target.CacheEventObjectEvents();

                var noEventsFound = events != null && events.Count == 0;
                if (noEventsFound)
                {
                    EditorGUILayout.LabelField("No suitable events found in this object.");
                    return;
                }
            }
                
            EditorGUILayout.LabelField("Select event to listen to:");
            Target._selectedEventIndex = EditorGUILayout.Popup(Target._selectedEventIndex, Target.EventObjectEventNames);

            var selectedComponentIndex = -1;
            if (!string.IsNullOrEmpty(Target.SelectedComponentName))
                selectedComponentIndex = Target._componentNames.IndexOf(Target.SelectedComponentName);

            var selectedComponentNoLongerExists = selectedComponentIndex < 0; 
            if (selectedComponentNoLongerExists)
                selectedComponentIndex = 0;
            EditorGUILayout.LabelField("Select listeners component:");
            Target.SelectedComponentIndex = EditorGUILayout.Popup(selectedComponentIndex, Target.ListenerComponentNames);

            var selectedMethodIndicesListProperty = serializedObject.FindProperty(nameof(Target._selectedMethodIndices));
            var hasValueChanged = Show(selectedMethodIndicesListProperty);

            if (hasValueChanged)
            {
                EditorUtility.SetDirty(target);
            }
        }

        public bool Show(SerializedProperty list)
        {
            var hasValueChanged = false;
            EditorGUILayout.LabelField("Select listener methods:");
            EditorGUI.indentLevel += 1;
            list.isExpanded = true;
            if (list.isExpanded)
            {
                for (var i = 0; i < list.arraySize; i++)
                {
                    Target._selectedMethodIndices[i] = EditorGUILayout.Popup(Target._selectedMethodIndices[i], Target.MethodNames);
                }
                Target.CacheSelectedMethodNames();
            }

            if (GUILayout.Button("+"))
            {
                Target._selectedMethodIndices.Add(0);
                hasValueChanged = true;
            }

            if (GUILayout.Button("-") && Target._selectedMethodIndices.Count >= 1)
            {
                Target._selectedMethodIndices.RemoveAt(Target._selectedMethodIndices.Count - 1);
                hasValueChanged = true;
            }

            if (hasValueChanged)
            {
                Target.CacheMethods();
            }
            return hasValueChanged;
        }
    }
#endif
    
}

