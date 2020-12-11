using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[CreateAssetMenu(menuName="Variables / Int Variable")]
public class IntVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline] public string DeveloperDescription = "";
#endif
    [SerializeField] protected int _value;
    public virtual int Value
    {
        get => _value;
        set => _value = value;
    }

    public void SetValue(int value)
    {
        Value = value;
    }

    public void SetValue(IntVariable value)
    {
        Value = value.Value;
    }

    public void ApplyChange(int amount)
    {
        Value += amount;
    }

    public void ApplyChange(IntVariable amount)
    {
        Value += amount.Value;
    }
}

