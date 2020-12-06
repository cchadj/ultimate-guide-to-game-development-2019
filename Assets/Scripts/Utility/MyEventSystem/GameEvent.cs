using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void GameEventDelegate();

[CreateAssetMenu(menuName = "Event System / Game Event")]
public partial class GameEvent : ScriptableObject
{
    public string EventName;

    public event EventHandler Event;
    public event EventHandler<ScriptableEventArgs> EventScriptable;

    // Hashes the handlers for each listener object.
    // Used with AddListener and RemoveListener.
    private Dictionary<object, Dictionary<GameEventDelegate, EventHandler>> _eventHandlers =
        new Dictionary<object, Dictionary<GameEventDelegate, EventHandler>>();
    
    [ContextMenu("Raise Event")]
    public void Raise()
    {
            Event?.Invoke(this, EventArgs.Empty);
    }

    public void Raise(ScriptableObject so)
    {
        if (so != null) EventScriptable?.Invoke(this, new ScriptableEventArgs(so));
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
