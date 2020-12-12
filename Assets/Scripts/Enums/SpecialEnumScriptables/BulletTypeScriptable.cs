using System;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "EnumEntry/BulletType")]
public class BulletTypeScriptable : ScriptableObject, IEnumEntry
{
    [SerializeField] private string _enumEntryName;
    public string EnumEntryName 
    {
        get =>_enumEntryName;
        private set { _enumEntryName = value; }
    }

    private void OnEnable()
    {
        if (EnumEntryName == "")
        {
            EnumEntryName = name.Remove(name.IndexOf("Type", StringComparison.Ordinal));
        }
    }
    #if UNITY_EDITOR
    [CustomEditor(typeof(BulletTypeScriptable))]
    private class BulletTypeScriptableEditor : Editor
    {
        private BulletTypeScriptable Target => target as BulletTypeScriptable;

        public override void OnInspectorGUI()
        {
            Target.EnumEntryName = EditorGUILayout.DelayedTextField("Enum Entry Name", Target.EnumEntryName);
            if (string.IsNullOrWhiteSpace(Target.EnumEntryName))
            {
                var entryName = Target.name;
                var idx = Target.EnumEntryName.IndexOf("Type", StringComparison.Ordinal);
                if (idx > 0)
                {
                    entryName = entryName.Remove(idx);
                }
                Target.EnumEntryName = entryName;
            }
        }
    } 
    #endif
}
