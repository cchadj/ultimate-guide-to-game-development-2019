using System.Collections.Generic;
using ModestTree;
using UnityEditor;
using UnityEngine;

public partial class EventListener : MonoBehaviour
{
#if UNITY_EDITOR
    [CustomEditor(typeof(EventListener))]
    public class EventListenerEditor : Editor
    {
        private EventListener Target => (EventListener) target;
        private List<int> _onEnabledSelectedMethodIndices;

        private void OnEnable()
        {
            Target.Cache();
            
            if (!Target.IsListenerObjectSet || !Target.IsEventObjectSet)
                return;
            
            _onEnabledSelectedMethodIndices = new List<int>();
            for (var i = 0; i < Target.SelectedMethodIndices.Count; i++)
            {
                var selectedIndex = 0;
                if (i < Target.SelectedMethodNames.Count)
                {
                    var methodName = Target.SelectedMethodNames?[i];
                    selectedIndex = Target.MethodNames.IndexOf(methodName);
                }
                else
                {
                    selectedIndex = -1;
                }

                if (selectedIndex < 0)
                    continue;
                _onEnabledSelectedMethodIndices.Add(selectedIndex);
            }

            Target.SelectedMethodIndices = _onEnabledSelectedMethodIndices;
        }

        public bool EventSelection()
        {
            EditorGUILayout.Space();

            var areEventsNotInitialised = Target.EventObjectEventNames.IsEmpty();
            if (areEventsNotInitialised)
            {
                var events = Target.CacheEventObjectEvents();

                var noEventsFound = events != null && events.Count == 0;
                if (noEventsFound)
                {
                    EditorGUILayout.HelpBox($"No suitable events found in event object <{Target.EventEmitterObject.name}>.", MessageType.Info);
                    return false;
                }
            }

            var isEmitterIsGameEvent = Target.EventEmitterObject is GameEvent ||
                                     Target.EventEmitterObject is GameEventWithArguments;
            if (!isEmitterIsGameEvent)
            {
                EditorGUILayout.LabelField("Select event to listen to:");
                Target._selectedEventIndex =
                    EditorGUILayout.Popup(Target._selectedEventIndex, Target.EventObjectEventNames);
            }
            
            if (Target.IsEventWithArgumentsSelected)
                HandleGameEventWithArguments();
            
            return true;
        }

        private void HandleGameEventWithArguments()
        {
            if (Target.MockEventArgumentsScriptable == null)
            {
                EditorGUILayout.HelpBox("Please make sure that a mock <ScriptableObject> asset is set " +
                                        "as arguments on the selected event.", MessageType.Warning);
            }
            else
            {
                Target._eventArguments = EditorGUILayout.ObjectField("Event Scriptable Object",
                    Target._eventArguments, Target.SelectedEventArgumentType, false) as ScriptableObject;
                
                if (Target._eventArguments == null)
                {
                    EditorGUILayout.HelpBox("GameEvent with arguments selected." +
                                            $" Please provide a ScriptableObject of type <{Target.SelectedEventArgumentType}>. ",
                        MessageType.Warning);                    
                }
            }
        }

        private bool ListenerComponentSelection()
        {
            var isListenerComponentsNotInitialised = Target.ListenerComponentNames == null || Target.ListenerComponentNames.IsEmpty();  
            if (isListenerComponentsNotInitialised)
                Target.CacheListenerComponents();

            var noListenerComponentsFoundExceptTransform = Target.ListenerComponentNames.Length <= 1;
            if (noListenerComponentsFoundExceptTransform)
                return false;
            
            var selectedComponentIndex = -1;
            if (!string.IsNullOrEmpty(Target.SelectedComponentName))
                selectedComponentIndex = Target._componentNames.IndexOf(Target.SelectedComponentName);

            var selectedComponentNoLongerExists = selectedComponentIndex < 0; 
            if (selectedComponentNoLongerExists)
                selectedComponentIndex = 0;
            EditorGUILayout.LabelField("Select listeners component:");
            Target.SelectedComponentIndex = EditorGUILayout.Popup(selectedComponentIndex, Target.ListenerComponentNames);

            return true;
        }

        public bool ListenerMethodSelection()
        {
            if (Target.MethodNames?.Length == 0)
            {
                Target.Cache();
                if (Target.MethodNames?.Length == 0)
                {
                    EditorGUILayout.HelpBox("No suitable methods found. " +
                                            "This may happen because no method in the selected component has the right arguments" +
                                            " to handle the event or the access is not public.", MessageType.Warning);
                    return false;
                }
            }
            
            var selectedMethodIndicesListProperty = serializedObject.FindProperty(nameof(Target._selectedMethodIndices));
            var hasValueChanged = Show(selectedMethodIndicesListProperty);

            if (hasValueChanged)
            {
                EditorUtility.SetDirty(target);
            }

            return true;
        }
    public override void OnInspectorGUI()
        {
            Target.EventEmitterObject = EditorGUILayout.ObjectField("Event Scriptable Object",
                Target.EventEmitterObject, typeof(ScriptableObject), true) as ScriptableObject;

            Target.ListenerGameObject = EditorGUILayout.ObjectField(label:"Listener Game Object", Target.ListenerGameObject,
                typeof(GameObject), true) as GameObject;

            if (!Target.IsListenerObjectSet || !Target.IsEventObjectSet)
            {
                EditorGUILayout.HelpBox("Please select an event object and an event listener object.", MessageType.Info);
                return;
            }

            var areEventsFound = EventSelection();
            if (!areEventsFound)
                return;

            var areComponentsFound = ListenerComponentSelection();
            if (!areComponentsFound)
                return;

            var areListenerMethodsFound = ListenerMethodSelection();
            if (!areListenerMethodsFound)
                return;
        }

        public bool Show(SerializedProperty list)
        {
            var hasValueChanged = false;
            EditorGUILayout.LabelField("Select listener methods:");
            EditorGUI.indentLevel += 1;
            list.isExpanded = true;
            if (list.isExpanded)
            {
                for (var i = 0; i < list.arraySize; i++)
                {
                    Target._selectedMethodIndices[i] = EditorGUILayout.Popup(Target._selectedMethodIndices[i], Target.MethodNames);
                }
                Target.CacheSelectedMethodNames();
            }

            if (GUILayout.Button("+"))
            {
                Target.SelectedMethodIndices.Add(0);
                hasValueChanged = true;
            }

            if (GUILayout.Button("-") && Target._selectedMethodIndices.Count >= 1)
            {
                Target._selectedMethodIndices.RemoveAt(Target._selectedMethodIndices.Count - 1);
                hasValueChanged = true;
            }

            if (hasValueChanged)
            {
                Target.CacheSelectedComponentMethods();
            }
            return hasValueChanged;
        }
    }
#endif
    
    
}
