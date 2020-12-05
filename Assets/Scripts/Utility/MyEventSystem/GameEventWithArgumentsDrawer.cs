using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public partial class GameEventWithArguments : ScriptableObject
{
    [CustomPropertyDrawer(typeof(GameEventWithArguments))]
    private class GameEventWithArgumentsPropertyDrawer : PropertyDrawer
    {
        private const float AdditionalPropertyHeight = 50f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            
            var rectPropertyPosition = new Rect(position.x, position.y + position.height * .1f, position.width, position.height * .3f);
            var raiseEventButtonRect = new Rect(position.x + position.width *.4f , position.y + position.height * .5f,
                position.width * .3f, position.height * .3f);
            EditorGUI.DrawRect(position, new Color(.1f, .2f, .4f, .1f));
            EditorGUI.PropertyField(rectPropertyPosition, property);
            var serializedObject = new SerializedObject( property.objectReferenceValue );
            
//            var prop = serializedObject.GetIterator();
//            EditorGUI.indentLevel = 2;
//            if (prop.NextVisible(true)) {
//
//                do
//                {
//                    if (prop.name == "m_Script") continue;
//                    EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.name), true);
//                }
//                while (prop.NextVisible(false));
//            }
//            serializedObject.ApplyModifiedProperties();
            
            if (GUI.Button(raiseEventButtonRect, "Raise Event"))
            {
                var gameEvent = RetrieveField<GameEventWithArguments>(property);
                if (gameEvent !=null) gameEvent.Raise();
            }
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + AdditionalPropertyHeight;
        }

        private static T RetrieveField<T>(SerializedProperty property)
        {
            var retrievedValue = default(T);
            var targetObject = property.serializedObject.targetObject;
            var field = targetObject.GetType().GetField(property.propertyPath);
            if (field != null)
            {
                retrievedValue = (T) field.GetValue(targetObject);
            }

            return retrievedValue;
        }
    }
}
#endif
