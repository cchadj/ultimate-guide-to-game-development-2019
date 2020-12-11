using System;
using UnityEngine;

[CreateAssetMenu(menuName="Variables / Observable Int Variable")]
public class ObservableIntVariable : IntVariable
{
    public GameEventWithArguments ValueChanged;

    public override int Value
    {
        get => _value;
        set
        {
            var hasValueChanged = _value != value;
            _value = value;
            
            if (hasValueChanged)
                ValueChanged.Raise(this);
        }
    }
}

