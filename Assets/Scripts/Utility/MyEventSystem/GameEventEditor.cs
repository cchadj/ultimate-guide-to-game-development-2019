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
            DrawDefaultInspector();
            if (GUILayout.Button("Raise Event"))
            {
                Debug.Log($"Raising event {Target.EventName} from {Target.name}");
                Target.Raise();
            }
        }
    }

}
#endif
