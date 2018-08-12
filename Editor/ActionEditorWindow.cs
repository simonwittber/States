using UnityEditor;
using UnityEngine;

namespace FSM
{
    public class ActionEditorWindow : EditorWindow
    {
        [SerializeField] SerializedProperty property;

        void OnGUI()
        {
            if (property == null)
            {
                EditorApplication.delayCall += Close;
                return;
            }
            else
                using (var cc = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("actionName"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("actionList"), true);
                    if (cc.changed)
                    {
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            if (Event.current.keyCode == KeyCode.Escape)
                Close();
        }

        public void Load(FiniteStateMachine target, FSMState state, ActionReference actionRef)
        {
            var serializedObject = new SerializedObject(target);
            var actions = serializedObject.FindProperty("actions");
            for (var i = 0; i < actions.arraySize; i++)
            {
                var p = actions.GetArrayElementAtIndex(i);
                if (p.FindPropertyRelative("guid").intValue == actionRef.guid)
                {
                    property = p;
                    break;
                }
            }
        }

        public void Load(FiniteStateMachine target, FSMState state, int guid)
        {
            var serializedObject = new SerializedObject(target);
            var actions = serializedObject.FindProperty("actions");
            for (var i = 0; i < actions.arraySize; i++)
            {
                var p = actions.GetArrayElementAtIndex(i);
                if (p.FindPropertyRelative("guid").intValue == guid)
                {
                    property = p;
                    break;
                }
            }
        }
    }
}