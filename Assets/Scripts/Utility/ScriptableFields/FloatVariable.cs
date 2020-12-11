using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

[CreateAssetMenu(menuName="Variables / Float Variable")]
public class FloatVariable : ScriptableObject
{
#if UNITY_EDITOR
          [Multiline] public string DeveloperDescription = "";
#endif
    [SerializeField] protected float _value;
    public virtual float Value { get => _value; set=> _value=value; }

    public void SetValue(float value)
    {
        Value = value;
    }

    public void SetValue(FloatVariable value)
    {
        Value = value.Value;
    }

    public void ApplyChange(float amount)
    {
        Value += amount;
    }

    public void ApplyChange(FloatVariable amount)
    {
        Value += amount.Value;
    }
}
