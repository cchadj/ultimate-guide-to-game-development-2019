using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTester : MonoBehaviour
{
    public GameEventWithArguments GameEvent;
    
    // Start is called before the first frame update
    private void OnEnable()
    {
        GameEvent.AddListener<GameStateScriptable>(this, Atest);
    }

    private void OnDisable()
    {
        GameEvent.RemoveListener<GameStateScriptable>(this, Atest);
    }

    private void Atest(GameStateScriptable o)
    {
        Debug.Log(o.name);
    }
}
