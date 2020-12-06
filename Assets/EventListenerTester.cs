using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class EventListenerTester : MonoBehaviour
{

    public void MethodWithScriptableObjectArguments(ScriptableObject scriptableObject)
    {
        Debug.Log("MethodWithScriptableObjectArguments");
    }
    
    public void MethodWithDerivedScriptableObjectArgumentPlayerStateScriptable(PlayerStateScriptable scriptableObject)
    {
        Debug.Log($"MethodWithDerivedScriptableObjectArgumentPlayerStateScriptable ({nameof(PlayerStateScriptable)})");
    }
    
    public void MethodWithDerivedScriptableObjectArgumentsFloatVariable(FloatVariable scriptableObject)
    {
            Console.WriteLine(new Random().Next() * scriptableObject.Value);
//        Debug.Log($"MethodWithDerivedScriptableObjectArgumentsFloatVariable ({nameof(FloatVariable)})");
    }

    public void MethodWithNoArguments()
    {
        Debug.Log("MethodWithNoArguments");
    }
}
