using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;


[CustomPropertyDrawer(typeof(DragAndDropPathAttribute))]
public class DragAndDropPathDrawer : PropertyDrawer
{
	private DragAndDropPathAttribute Attribute => attribute as DragAndDropPathAttribute;
    // Draw the property inside the given rect
    public override void OnGUI(Rect fieldRect, SerializedProperty property, GUIContent label)
    {
	    Attribute.FieldPath = property.stringValue;
	    EditorGUI.LabelField(fieldRect, label.text + ":" + "\t(Drag n' Drop a file)");
		    
	    GUILayout.TextArea(string.IsNullOrEmpty(Attribute.FieldPath) ? "Drag n' Drop a folder" : Attribute.FieldPath, GUILayout.ExpandHeight(true));

	    GUILayout.Space(5);
	    

	    var dropArea = fieldRect;
	    dropArea.yMax += (fieldRect.yMax - fieldRect.y) + 4;
	    dropArea.y -=  4;
	    var isInDropArea = dropArea.Contains(Event.current.mousePosition);
	    
	    if (Event.current.type == EventType.DragUpdated)
		{
			DragAndDrop.visualMode =  isInDropArea
				? DragAndDropVisualMode.Copy
				: DragAndDropVisualMode.Rejected;
			
			Event.current.Use();
		}
		else if (Event.current.type == EventType.DragPerform)
		{
			if (isInDropArea)
				DragAndDrop.AcceptDrag();
			else
				return;
			
			var path = DragAndDrop.paths[0];
			Attribute.FieldPath = path;
			property.stringValue = path;
		}
    }
 }
