using System.IO;
using UnityEngine;
using UnityEditor;


#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(DragAndDropPathAttribute))]
public class DragAndDropPathDrawer : PropertyDrawer
{
	private DragAndDropPathAttribute Attribute => attribute as DragAndDropPathAttribute;

	private float _curElementY = 0f;

	private Rect GetElementPosition(Rect position, float height = -1)
	{
		var currElementHeight = height < 0 ? EditorGUIUtility.singleLineHeight : height;
		var widthRatio = .95f;
		var newPosition = new Rect(position.x,
			_curElementY,
			position.width * widthRatio,
			currElementHeight);

		_curElementY += currElementHeight;
		return newPosition;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return EditorGUIUtility.singleLineHeight * 2;
	}

	// Draw the property inside the given rect
    public override void OnGUI(Rect fieldRect, SerializedProperty property, GUIContent label)
    {
	    _curElementY = fieldRect.y;
	    Attribute.FieldPath = property.stringValue;

	    EditorGUI.BeginProperty(fieldRect, label, property);

	    var fileTypeStr = "";
	    if (Attribute.AcceptFile)
		    fileTypeStr += "file ";

	    if (Attribute.AcceptFolder)
		    if (Attribute.AcceptFile)
			    fileTypeStr += " or a ";
		    fileTypeStr += "folder";
		    
	    var labelPosition = GetElementPosition(fieldRect);
	    EditorGUI.LabelField(labelPosition, label.text + ":" + $"\t(Drag n' Drop a {fileTypeStr})");
		    
	    var textAreaPosition = GetElementPosition(fieldRect);
	    EditorGUI.TextArea(textAreaPosition, string.IsNullOrEmpty(Attribute.FieldPath) ? $"Drag n' Drop a {fileTypeStr}" : Attribute.FieldPath);

		var dropArea = fieldRect;
		var isMouseInDropArea = dropArea.Contains(Event.current.mousePosition);
		
	    if (Event.current.type == EventType.DragUpdated)
		{
			var doesPathMeetsCriteria = true;
			var path = DragAndDrop.paths[0];
			var pathAttributes = File.GetAttributes(path);
			var isPathAFolder = (pathAttributes & FileAttributes.Directory) == FileAttributes.Directory;
			
			if (!Attribute.AcceptFile &&  !isPathAFolder)
					doesPathMeetsCriteria = false;
			if (!Attribute.AcceptFolder && isPathAFolder)
					doesPathMeetsCriteria = false;
			
			DragAndDrop.visualMode =  isMouseInDropArea && doesPathMeetsCriteria
				? DragAndDropVisualMode.Copy
				: DragAndDropVisualMode.Rejected;
			
			if (DragAndDrop.visualMode != DragAndDropVisualMode.Copy)
			{
				// If the event is not in the drop area then we pass it to other properties that might want to
				// use it
				Event.PopEvent(Event.current);
			}
			else
			{
				// If inside the drop area make sure to use it so other properties can not.
				Event.current.Use();
			}
		}
	    
		else if (Event.current.type == EventType.DragPerform)
	    {
		    if (!isMouseInDropArea) return;
		    
			DragAndDrop.AcceptDrag();
			
			var path = DragAndDrop.paths[0];
			Attribute.FieldPath = path;
			property.stringValue = path;
			Event.current.Use();
	    }
	    EditorGUI.EndProperty();
	    
    }
 }
#endif
