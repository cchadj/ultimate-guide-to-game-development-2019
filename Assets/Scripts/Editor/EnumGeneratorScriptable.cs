 #if UNITY_EDITOR
 using System;
 using System.Collections.Generic;
 using System.Collections.ObjectModel;
 using System.Collections.Specialized;
 using UnityEditor;
 using System.IO;
 using UnityEngine;

 
 [CreateAssetMenu(menuName="EnumGenerator")]
 public class EnumGeneratorScriptable: ScriptableObject
 {
     [SerializeField, DragAndDropPath] private string _sourceFolderPath;
     
     private string _targetFolderPath;
     
     [SerializeField] private string _enumName;
     
     [SerializeField] private List<ScriptableObject> _enumEntries;

     private ObservableCollection<ScriptableObject> _enumEntriesObservable;
     private void OnEnable()
     {
         if (_enumName == "")
         {
             _enumName = name.Replace("EnumGenerator", string.Empty);
         }

         if (string.IsNullOrEmpty(_targetFolderPath))
         {
             _targetFolderPath = "Assets/Scripts/Enums/";
         }
//         _enumEntriesObservable = new ObservableCollection<ScriptableObject>(_enumEntries);
//         _enumEntriesObservable.CollectionChanged += EnumEntriesChanged;
         
     }
     
     private void OnDisable()
     {
//         _enumEntriesObservable.CollectionChanged -= EnumEntriesChanged;
     }

     private void EnumEntriesChanged(object sender, NotifyCollectionChangedEventArgs e)
     {
         Debug.Log("Am I ever called?");
         GenerateEnum();
     }
     #if UNITY_EDITOR
     
     public static List<T> FindAssetsByType<T>(string[] folders = null) where T : UnityEngine.Object
     {
         var assets = new List<T>();
         var guids = AssetDatabase.FindAssets($"t:{typeof(T)}", folders);
         
         foreach (var guid in guids)
         {
             var assetPath = AssetDatabase.GUIDToAssetPath( guid );
             var asset = AssetDatabase.LoadAssetAtPath<T>( assetPath );
             
             if( asset != null )
             {
                 assets.Add(asset);
             }
         }
         return assets;
     }
 
     [ContextMenu("GenerateEnum")]
     internal string GenerateEnum()
     {
         //The folder Scripts/Enums/ is expected to exist
         var filePathAndName =  _targetFolderPath + _enumName + ".cs";
         
         var info = new DirectoryInfo(_sourceFolderPath);
         var fileInfo = info.GetFiles();

         _enumEntries = FindAssetsByType<ScriptableObject>(new []{_sourceFolderPath});

         using ( var streamWriter = new StreamWriter( filePathAndName ) )
         {
             streamWriter.WriteLine( "public enum " + _enumName );
             streamWriter.WriteLine( "{" );
             foreach (var enumEntry in _enumEntries)
             {
                 var enumEntryName = (enumEntry as IEnumEntry)?.EnumEntryName ?? enumEntry.name;

                 streamWriter.WriteLine("\t" + enumEntryName + ",");
             }

             streamWriter.WriteLine( "}" );
         }
         AssetDatabase.Refresh();
         return filePathAndName;
     }
     #endif
     
     [ContextMenu("Ok")]
     private void Print()
     {
        Debug.Log(_sourceFolderPath);
     }
 }
 #endif