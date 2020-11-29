using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
public class BulletTypeScriptable : ScriptableObject, IEnumEntry
{
    [field:SerializeField] public string EnumEntryName { get; set; }

    private void OnEnable()
    {
        if (EnumEntryName == "")
        {
            EnumEntryName = name;
        }
    }
}
