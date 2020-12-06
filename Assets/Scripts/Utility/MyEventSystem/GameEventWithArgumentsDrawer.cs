using System.Collections.Generic;
using System.Reflection;
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
        
        private float _curElementY = 0f;
        private Rect GetElementPosition(Rect position, float height=-1)
        {
            var currElementHeight = height < 0 ? EditorGUIUtility.singleLineHeight : height;
            var widthRatio = .95f;
            var newPosition = new Rect(position.x + BoundingBoxWidth,
                _curElementY,
                position.width * widthRatio,
                currElementHeight);

            _curElementY += currElementHeight + _elementOffset;
            return newPosition;
        }

        
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_property == null)
                property = _property;
            var initialOffset = (position.y + OffsetFromPreviousProperty + BoundingBoxWidth);
            _curElementY = initialOffset;
            
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.DrawRect(position, new Color(.1f, .2f, .4f, .1f));

            var rectPropertyPosition = GetElementPosition(position);
            EditorGUI.PropertyField(rectPropertyPosition, property);

            if (property == null) return;
            
            var isPropertySetInTheEditor = property.objectReferenceValue != null;
            if (!isPropertySetInTheEditor) return;
            DrawChildrenProperties(position);

            var raiseEventButtonRect = GetElementPosition(position);
            if (GUI.Button(raiseEventButtonRect, "Raise Event"))
            {
                var gameEvent = RetrieveField<GameEventWithArguments>(property);
                if (gameEvent != null) gameEvent.Raise();
            }
            EditorGUI.EndProperty();
        }
        
        private void DrawChildrenProperties(Rect position)
        {
            var areChildrenPropertiesSet = ChildrenProperties != null;
            if (areChildrenPropertiesSet)
            {
                foreach (var serializedProperty in ChildrenProperties)
                {
                    var propertyPosition = GetElementPosition(position, EditorGUI.GetPropertyHeight(serializedProperty));
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

            var miniViewTitleOffset = EditorGUIUtility.singleLineHeight;
            var elementOffsets = 2 * _elementOffset;
            var height = base.GetPropertyHeight(property,  label) + OffsetFromPreviousProperty + 2 * BoundingBoxWidth + 
                         miniViewTitleOffset + elementOffsets;

            if (ChildrenProperties == null) return height;
        
            foreach (var childProperty in ChildrenProperties)
            {
                var propertyHeight = EditorGUI.GetPropertyHeight(childProperty);
                height += propertyHeight + _elementOffset;
            }
            return height;
        }

        private static T RetrieveField<T>(SerializedProperty property)
        {
            var retrievedValue = default(T);
            var targetObject = property.serializedObject.targetObject;
            var field = targetObject.GetType().GetField(property.propertyPath, 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance );
            if (field != null)
            {
                retrievedValue = (T) field.GetValue(targetObject);
            }
            return retrievedValue;
        }
    }
}
#endif