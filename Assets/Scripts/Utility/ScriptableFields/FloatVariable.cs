using UnityEngine;

[CreateAssetMenu(menuName="Variables / Float Variable")]
public class FloatVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline] public string DeveloperDescription = "";
#endif
    public float Value;

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

[CreateAssetMenu(menuName="Variables / Int Variable")]
public class IntVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline] public string DeveloperDescription = "";
#endif
    public int Value;

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

