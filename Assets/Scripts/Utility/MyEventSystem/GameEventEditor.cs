using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public partial class GameEvent
{
    [CustomEditor(typeof(GameEvent))]
    private class GameEventEditor : Editor
    {
        private GameEvent Target { get => target as GameEvent;}

        public override void OnInspectorGUI()
        {
            var prop = serializedObject.FindProperty(nameof(Target.EventName));
            Target.EventName = EditorGUILayout.DelayedTextField(new GUIContent(prop.displayName), Target.EventName);
            if (string.IsNullOrEmpty(Target.EventName))
            {
                Target.EventName = Target.name;
            }
            
            if (GUILayout.Button("Raise Event"))
            {
                Debug.Log($"Raising event {Target.EventName} from {Target.name}");
                Target.Raise();
            }
        }
    }
}
#endif
