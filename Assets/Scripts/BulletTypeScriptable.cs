using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "EnumEntry/BulletType")]
public class BulletTypeScriptable : ScriptableObject, IEnumEntry
{
    [field:SerializeField] public string EnumEntryName { get; private set; }

    private void OnEnable()
    {
        if (EnumEntryName == "")
        {
            EnumEntryName = name;
        }
    }
}
