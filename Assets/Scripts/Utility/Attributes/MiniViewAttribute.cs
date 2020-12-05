using System;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
internal sealed class MiniViewAttribute : PropertyAttribute{}

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( MiniViewAttribute ) )]
public class MiniViewDrawer : PropertyDrawer {
    public override void OnGUI ( Rect position, SerializedProperty property, GUIContent label )
    {
        EditorGUILayout.LabelField(new GUIContent(property.displayName + ": "));
        EditorGUILayout.PropertyField(property,  GUIContent.none, true);
        
        EditorGUILayout.Space(30);
        if (property.objectReferenceValue == null) return;
        
        var serializedObject = new SerializedObject( property.objectReferenceValue );
        GUIUtility.ScaleAroundPivot(new Vector2(.85f, 0.85f), 
            new Vector2(.77f, .77f));

        var prop = serializedObject.GetIterator();
        
        EditorGUI.indentLevel = 2;
        if (prop.NextVisible(true)) {

            do
            {
                if (prop.name == "m_Script") continue;
                EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true);
            }
            while (prop.NextVisible(false));
        }
        serializedObject.ApplyModifiedProperties();
    }
}
#endif
