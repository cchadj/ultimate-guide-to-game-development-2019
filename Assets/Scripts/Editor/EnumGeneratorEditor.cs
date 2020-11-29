using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnumGeneratorScriptable))]
public class EnumGeneratorScriptableEditor : Editor {
    
    private EnumGeneratorScriptable Target => target as EnumGeneratorScriptable;

    private string _filePathAndName;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Generate/Update Enum"))
        {
           var filePathAndName = Target.GenerateEnum();
           Debug.Log("Created enum: '" + filePathAndName + "'");
           _filePathAndName = filePathAndName;
        }

        if (!string.IsNullOrEmpty(_filePathAndName))
        {
            GUILayout.TextArea("Created enum: '" + _filePathAndName + "'");
        }
    }
}
