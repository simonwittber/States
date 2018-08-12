using UnityEditor;
using UnityEngine;

namespace FSM
{
    public class PredicateEditorWindow : EditorWindow
    {
        [SerializeField] SerializedProperty property;

        void OnGUI()
        {
            if (property != null)
                using (var cc = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("predicateName"));
                    EditorGUILayout.PropertyField(property.FindPropertyRelative("predicateList"), true);
                    if (cc.changed)
                    {
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }
            if (Event.current.keyCode == KeyCode.Escape)
                Close();
        }

        public void Load(FiniteStateMachine target, FSMState state, int guid)
        {
            var serializedObject = new SerializedObject(target);
            var predicates = serializedObject.FindProperty("predicates");
            for (var i = 0; i < predicates.arraySize; i++)
            {
                var p = predicates.GetArrayElementAtIndex(i);
                if (p.FindPropertyRelative("guid").intValue == guid)
                {
                    property = p;
                    break;
                }
            }
        }
    }
}