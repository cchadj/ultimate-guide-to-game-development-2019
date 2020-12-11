using System;
using UnityEngine;
[CreateAssetMenu(menuName="Variables / Observable Float Variable")]
public class ObservableFloatVariable : FloatVariable
{
    public EventHandler<ScriptableEventArgs> valueChanged;

    public override float Value
    {
        get => _value;
        set
        {
            _value = value;
            valueChanged?.Invoke(this, new ScriptableEventArgs(this));
        }
    }
}

