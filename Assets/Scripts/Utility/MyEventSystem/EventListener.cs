using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using Component = UnityEngine.Component;
using Debug = UnityEngine.Debug;

public partial class EventListener : MonoBehaviour
{
    [SerializeField] private ScriptableObject _eventObject;
    [SerializeField] private ScriptableObject _eventArguments;
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
                CacheSelectedComponentMethods();
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

    public bool IsEventWithArgumentsSelected
    {
        get
        {
            if (SelectedEvent == null)
                Cache();
            return SelectedEvent.EventHandlerType == typeof(EventHandler<ScriptableEventArgs>);
        }
    }

    private void CacheSelectedMethodNames()
    {
        Cache();
        
        if (_selectedMethodIndices == null) return;
        
        _selectedMethodNames = new List<string>();
        foreach (var i in _selectedMethodIndices)
        {
            if (i >= _methodInfos.Count)
                continue;
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

    private Dictionary<EventInfo, FieldInfo> _eventToGameEventFieldInfo;
    
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
        CacheSelectedComponentMethods();
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
            {
                try
                {
                    return _eventInfos?[_selectedEventIndex];
                }
                catch (ArgumentOutOfRangeException)
                {
                    _selectedEventIndex = 0;
                    return _eventInfos?[_selectedEventIndex];
                }
            }
            
            var areNoEventsFound = _eventInfos != null && _eventInfos.Count == 0; 
            return areNoEventsFound ? null : _eventInfos?[_selectedEventIndex];
        }
    }

    private GameEventWithArguments GameEventWithArguments
    {
        get
        {
            if (SelectedEvent == null) return null;
            
            if (!IsEventWithArgumentsSelected) return null;
            
            var gameEventField = _eventToGameEventFieldInfo[SelectedEvent];
            var gameEventWithArgumentsInstance = gameEventField.GetValue(_eventObject) as GameEventWithArguments;

            return gameEventWithArgumentsInstance;
        }
    }
    private ScriptableObject MockEventArgumentsScriptable
    {
        get
        {
            if (SelectedEvent == null) return null;
            
            if (!IsEventWithArgumentsSelected) return null;
            
            var gameEventField = _eventToGameEventFieldInfo[SelectedEvent];
            var gameEventWithArgumentsInstance = gameEventField.GetValue(_eventObject) as GameEventWithArguments;
            
            var mockArgumentPropertyInfo = gameEventField.FieldType.GetProperty(nameof(GameEventWithArguments.MockArgumentsScriptable));
            if (mockArgumentPropertyInfo != null)
            {
                return mockArgumentPropertyInfo.GetValue(gameEventWithArgumentsInstance) as ScriptableObject;
            }

            return null;
        }
    }

    // ReSharper disable once Unity.NoNullPropogation
    private Type SelectedEventArgumentType => MockEventArgumentsScriptable?.GetType();

    private object SelectedEventOwner
    {
        get
        {
            try
            {
                return _eventOwners?[_selectedEventIndex];
            }
            catch (ArgumentOutOfRangeException e)
            {
                return null;
            }
        }
    } 

    private Component SelectedComponent => Components?[SelectedComponentIndex];



    [SerializeField, HideInInspector] private List<string> _selectedMethodNames;
    public List<string> SelectedMethodNames => _selectedMethodNames;

    private List<Action> _selectedMethods;
    private IEnumerable<Action> SelectedComponentMethods
    {
        get
        {
            var isSelectedMethodsInitialised = _selectedMethods != null && _selectedMethods.Count != 0;

            if (isSelectedMethodsInitialised) return _selectedMethods;

            // Initialise selected methods
            _selectedMethods = new List<Action>();
            foreach (var i in _selectedMethodIndices)
            {
                var methodInfo = _methodInfos[i];
                var methodsInstance = SelectedComponent;
                if (methodInfo.IsStatic)
                    methodsInstance = null;
                var del = (Action) Delegate.CreateDelegate(typeof(Action), methodsInstance, methodInfo);
                _selectedMethods.Add(del);
            }

            return _selectedMethods;
        }
    }
    
    private List<Action<object>> _selectedMethodsWithArguments;

    private IEnumerable<Action<object>> SelectedComponentMethodsWithArguments
    {
        get
        {
            var isSelectedMethodsInitialised = _selectedMethodsWithArguments != null && _selectedMethodsWithArguments.Count != 0;

            if (isSelectedMethodsInitialised) return _selectedMethodsWithArguments;

            // Initialise selected methods
            _selectedMethodsWithArguments = new List<Action<object>>();
            foreach (var i in _selectedMethodIndices)
            {
                var methodInfo = _methodInfos[i];
                var methodsInstance = SelectedComponent;
                if (methodInfo.IsStatic)
                    methodsInstance = null;
                var actionT = typeof (Action<>).MakeGenericType(SelectedEventArgumentType);
                var del = Delegate.CreateDelegate(actionT, methodsInstance, methodInfo);
                var a = (Action<object>)typeof(EventListener)
                    .GetMethod(nameof(Convert))
                    .MakeGenericMethod(SelectedEventArgumentType)
                    .Invoke(null, new object[] { del });
                _selectedMethodsWithArguments.Add(a);
            }

            return _selectedMethodsWithArguments;
        }
    }
    
    public static Action<object> Convert<T>(Action<T> myActionT)
    {
        return o => myActionT((T)o);
    }
    
    private Delegate _currentHandler;

    public void Subscribe()
    {
        var method = GetType().GetMethod(IsEventWithArgumentsSelected? 
            nameof(CallSelectedMethodsWithArguments) : 
            nameof(CallSelectedMethods));
        
        if (SelectedEventOwner == null)
        {
            Debug.LogWarning($"At EventListener in gameObject '{gameObject.name}' no subscription made because of no SelectedEvent");
        }
           
        _currentHandler = Delegate.CreateDelegate(SelectedEvent.EventHandlerType, this, method);
        SelectedEvent.AddEventHandler(SelectedEventOwner, _currentHandler);
    }

    public void CallSelectedMethods(object sender, EventArgs eventArgs)
    {
        foreach (var method in SelectedComponentMethods)
        {
            // This is only slightly slower than a normal method call 
            method();
        }
    }
    
    public void CallSelectedMethodsWithArguments(object sender, ScriptableEventArgs eventArgs)
    {
        var eventArgumentType = _eventArguments.GetType();
        foreach (var method in SelectedComponentMethodsWithArguments)
        {
            
            var actionT = typeof (Action<>).MakeGenericType(SelectedEventArgumentType);
            // This is only slightly slower than a normal method call 
            method(eventArgs.GetArguments<ScriptableObject>());
        }
    }

    [ContextMenu("PerformanceTest")]
    private void PerformanceTest()
    {
        const int repeats = 100000;
        print($"Performing test for {repeats} times");
        print("Call through delegates");
        var sw = new Stopwatch();
        sw.Start();
        for (var i = 0; i <= repeats; i++)
            CallSelectedMethodsWithArguments(this, new ScriptableEventArgs(ScriptableObject.CreateInstance<FloatVariable>()));; 
        sw.Stop();
        Debug.Log("Elapsed={0}" + sw.Elapsed);

        print("Direct call.");
        var eventTester = GetComponent <EventListenerTester>();
        sw = new Stopwatch();
        sw.Start();
        for (var i = 0; i <= repeats; i++)
            eventTester.MethodWithDerivedScriptableObjectArgumentsFloatVariable(ScriptableObject.CreateInstance<FloatVariable>());
        sw.Stop();
        Debug.Log("Elapsed={0}" + sw.Elapsed);
        
        print("Call through raise event.");
        sw = new Stopwatch();
        sw.Start();
        for (var i = 0; i <= repeats; i++)
            GameEventWithArguments.Raise(ScriptableObject.CreateInstance<FloatVariable>());
        sw.Stop();
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
       if (IsEventWithArgumentsSelected)
       {
           methodArguments = new[] {SelectedEventOwner, new ScriptableEventArgs(MockEventArgumentsScriptable)};
       }
       
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
        _eventToGameEventFieldInfo = new Dictionary<EventInfo, FieldInfo>();
        foreach (var eventInfo in eventInfos)
        {
            _eventOwners.Add(_eventObject);
            _eventInfos.Add(eventInfo);
            _eventNames.Add(eventInfo.Name);
        }

        var gameEventFields = eventObjectType.
            GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(field => field.FieldType == typeof(GameEvent) || field.FieldType == typeof(GameEventWithArguments));

        foreach (var gameEventField in gameEventFields)
        {
            var eventInfo = gameEventField.FieldType.GetEvents()[0];
            _eventOwners.Add(gameEventField.GetValue(_eventObject));
            _eventInfos.Add(eventInfo);
            _eventToGameEventFieldInfo[eventInfo] = gameEventField;
            
            var eventName = gameEventField.Name;
            if (gameEventField.Name.Contains("<") && gameEventField.Name.Contains(">"))
            {
                var reg = new Regex("<.*?>");
                var matches = reg.Matches(eventName);
                if (matches.Count >= 1)
                {
                    eventName = matches[0].ToString();
                }
            }
            _eventNames.Add(eventName);
        }

        return _eventInfos;
    }
    
    public void CacheSelectedComponentMethods()
    {
        if (SelectedComponent == null) return;
         
        _methodNames = new List<string>();
        _methodInfos = new List<MethodInfo>();
        _selectedMethods = null;
        
        var typeMethods = SelectedComponent.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public &
                        ~BindingFlags.GetProperty & ~BindingFlags.SetProperty)
            .Where(m => !m.IsSpecialName && 
                        !m.Name.Contains("Get") &&
                        !m.Name.Contains("get") && 
                        !m.Name.Contains("_"));

        var eventArgumentType = typeof(ScriptableObject);
        if (_eventArguments != null)
            eventArgumentType = _eventArguments.GetType();
        
        typeMethods = IsEventWithArgumentsSelected ? 
            GetMethodsWithArgumentOfTypeOrSubtype(typeMethods, eventArgumentType) :
            GetMethodsWithoutArguments(typeMethods);
        
        foreach (var methodInfo in typeMethods)
        {
            _methodInfos.Add(methodInfo);
            _methodNames.Add(methodInfo.Name);
        }
    }
    
    private static IEnumerable<MethodInfo> GetMethodsWithoutArguments(IEnumerable<MethodInfo> methodInfos)
    {
        return methodInfos.Where(m => m.GetParameters().Length == 0);
    }

    private static IEnumerable<MethodInfo> GetMethodsWithArgumentOfTypeOrSubtype<T>(IEnumerable<MethodInfo> methodInfos)
    {
        return GetMethodsWithArgumentOfTypeOrSubtype(methodInfos, typeof(T));
    }
    
    private static IEnumerable<MethodInfo> GetMethodsWithArgumentOfTypeOrSubtype(IEnumerable<MethodInfo> methodInfos, Type type)
    {
        return methodInfos.Where(m => m.GetParameters().Length == 1 &&
                                      (m.GetParameters()[0].ParameterType.IsSubclassOf(type) ||
                                       m.GetParameters()[0].ParameterType == type));
    }
    
    private IEnumerable<MethodInfo> GetMethodsWithMatchingParameters(IEnumerable<MethodInfo> methodInfos,
        EventInfo eventInfo)
    {
        var suitableMethods = new List<MethodInfo>();
        methodInfos = methodInfos.Where(m =>
            m.GetParameters().Length == SelectedEvent.EventHandlerType.GetGenericArguments().Length);

        var eventArguments = SelectedEvent.EventHandlerType.GetGenericArguments();
        foreach (var method in methodInfos)
        {
            var methodParameters = method.GetParameters();
            for (var i = 0; i < methodParameters.Length; i++)
            {
                var methodParamType = methodParameters[i].ParameterType;
                var eventArgumentType = eventArguments[i].BaseType;
                if (methodParamType == eventArgumentType || !methodParamType.IsSubclassOf(eventArgumentType ?? throw new ArgumentNullException()))
                    suitableMethods.Add(method);
            }
        }
        return suitableMethods;
    }
}

