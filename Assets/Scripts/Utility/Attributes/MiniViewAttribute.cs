using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
internal sealed class MiniViewAttribute : PropertyAttribute{}

#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( MiniViewAttribute ) )]
public class MiniViewDrawer : PropertyDrawer
{
    private SerializedProperty _property;
    
    private List<SerializedProperty> _childProperties;

    private SerializedObject _serializedObject;

    private const float BoundingBoxWidth = 3f;

    private const float OffsetFromPreviousProperty= 5f;
    
    public SerializedObject SerializedObject =>
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
    
    public override void OnGUI ( Rect position, SerializedProperty property, GUIContent label )
    {
        // This must be always come first in the methods!
        if (_property == null)
            _property = property;
 
        var initialOffset = (position.y + OffsetFromPreviousProperty + BoundingBoxWidth);
        _curElementY = initialOffset;

        EditorGUI.BeginProperty(position, label, property);
        
        DrawBoundingBox(position);
        
//        var titleRect = GetElementPosition(position);
        var style = new GUIStyle(GUI.skin.label) {alignment = TextAnchor.UpperRight};
        style.normal.textColor = Color.grey;
        // Draw the label of the property
        var labelRect = GetElementPosition(position);
        
        EditorGUI.LabelField(labelRect, new GUIContent("Mini View Window"), style);
        EditorGUI.LabelField(labelRect, new GUIContent(property.displayName + ": "));
        
        // Draw the property field
        var propertyRect = GetElementPosition(position);
        EditorGUI.PropertyField(propertyRect,  property, GUIContent.none, true);

        var isPropertySetInTheEditor = property.objectReferenceValue != null;
        if (!isPropertySetInTheEditor) return;

        DrawChildrenProperties(position);

        EditorGUI.EndProperty();
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

    private float _elementOffset = 5f;
    private float _curElementY;
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

    private void DrawBoundingBox(Rect position)
    {
        var indent = -3f;
        EditorGUI.DrawRect(new Rect(new Vector2(position.x + indent, position.y), new Vector2(position.width, BoundingBoxWidth)), Color.grey);
        EditorGUI.DrawRect(new Rect(new Vector2(position.x + indent, position.y), new Vector2(BoundingBoxWidth, position.height - BoundingBoxWidth)), Color.grey);
        EditorGUI.DrawRect(new Rect(new Vector2(position.x + indent + position.width, position.y), new Vector2(BoundingBoxWidth, position.height)), Color.grey);
        EditorGUI.DrawRect(new Rect(new Vector2(position.x + indent, position.y + position.height - BoundingBoxWidth), new Vector2(position.width, BoundingBoxWidth)), Color.grey);
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
}
#endif
