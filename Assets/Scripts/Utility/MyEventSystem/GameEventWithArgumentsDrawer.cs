using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public partial class GameEventWithArguments : ScriptableObject
{
    [CustomPropertyDrawer(typeof(GameEventWithArguments))]
    private class GameEventWithArgumentsPropertyDrawer : PropertyDrawer
    {    
        private SerializedProperty _property;
    
        private List<SerializedProperty> _childProperties;

        private SerializedObject _serializedObject;
        
        private const float AdditionalPropertyHeight = 200f;
        
        private float _elementOffset = 3f;

        private const float BoundingBoxWidth = 3f;

        private const float OffsetFromPreviousProperty= 8f;
    
        private SerializedObject SerializedObject =>
            _serializedObject ?? (_serializedObject = _property.objectReferenceValue != null
                ? new SerializedObject(_property.objectReferenceValue) : null);
    
        private List<SerializedProperty> ChildrenProperties
        {
            get
            {
                if (_childProperties != null) return _childProperties;

                if (SerializedObject == null) return null;

                var propertyIterator = SerializedObject.GetIterator();
            
                _childProperties = new List<SerializedProperty>();
                if (propertyIterator.NextVisible(true))
                {
                    do
                    {
                        if (propertyIterator.name == "m_Script") continue;
                    
                        var serializedProperty = SerializedObject.FindProperty(propertyIterator.name);
                        _childProperties.Add(serializedProperty);   
                    } while (propertyIterator.NextVisible(false));
                }
                return _childProperties;
            }
        }
        
        private float _curHeight = 0f;
        private Rect GetElementPosition(Rect position, int numElements, float height=-1)
        {
            var widthRatio = .95f;
            var newPosition = new Rect(position.x + BoundingBoxWidth,
                _curHeight,
                position.width * widthRatio,
                height < 0 ? EditorGUIUtility.singleLineHeight : height);
        
            if (height < 0)
            {
                _curHeight += EditorGUIUtility.singleLineHeight;
            }
            else
            {
                _curHeight += height;
            }

            _curHeight += _elementOffset;
            return newPosition;
        }
        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_property == null)
                property = _property;
            var initialOffset = (position.y + OffsetFromPreviousProperty + BoundingBoxWidth);
            _curHeight = initialOffset;
            
            var numElements = 2; // Two initial elements for the property itself and the raiseEventButton
            if (ChildrenProperties != null)
                numElements += ChildrenProperties.Count;
            
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.DrawRect(position, new Color(.1f, .2f, .4f, .1f));

            var rectPropertyPosition = GetElementPosition(position, numElements);
            EditorGUI.PropertyField(rectPropertyPosition, property);

            if (property == null) return;
            
            var isPropertySetInTheEditor = property.objectReferenceValue != null;
            if (!isPropertySetInTheEditor) return;
            DrawChildrenProperties(position, numElements);

            var raiseEventButtonRect = GetElementPosition(position, numElements);
            if (GUI.Button(raiseEventButtonRect, "Raise Event"))
            {
                var gameEvent = RetrieveField<GameEventWithArguments>(property);
                if (gameEvent != null) gameEvent.Raise();
            }
            EditorGUI.EndProperty();
        }
        
        private void DrawChildrenProperties(Rect position, int numElements)
        {
            var areChildrenPropertiesSet = ChildrenProperties != null;
            if (areChildrenPropertiesSet)
            {
                foreach (var serializedProperty in ChildrenProperties)
                {
                    var propertyPosition = GetElementPosition(position, numElements, EditorGUI.GetPropertyHeight(serializedProperty));
                    EditorGUI.PropertyField(propertyPosition, serializedProperty, includeChildren:true);
                }
                SerializedObject?.ApplyModifiedProperties();
            }
        }
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // This must be always come first in the methods!
            if (_property == null)
                _property = property;

            var height =  EditorGUIUtility.singleLineHeight * 3 + OffsetFromPreviousProperty + 2 * BoundingBoxWidth;

            if (ChildrenProperties == null) return height;

            foreach (var childProperty in ChildrenProperties)
            {
//                var propertyHeight = base.GetPropertyHeight(childProperty, new GUIContent(childProperty.displayName));
//                EditorGUILayout.PropertyField(property);
                var propertyHeight  = EditorGUI.GetPropertyHeight(childProperty);
                height += propertyHeight + _elementOffset;
            }

            return height;
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