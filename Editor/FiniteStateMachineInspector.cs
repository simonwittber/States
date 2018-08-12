using UnityEditor;
using UnityEngine;

namespace FSM
{
    [CustomEditor(typeof(FiniteStateMachine))]
    public class FiniteStateMachineInspector : Editor
    {
        int hotAction;
        int hotPredicate;

        public override void OnInspectorGUI()
        {
            var fsm = target as FiniteStateMachine;
            if (GUILayout.Button("Edit Graph"))
            {
                var w = EditorWindow.GetWindow<FiniteStateMachineEditorWindow>();
                w.Show();
                w.Load(target);
            }
            var ev = Event.current;
            // base.OnInspectorGUI();
            var indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            using (var cc = new EditorGUI.ChangeCheckScope())
            {
                {
                    EditorGUILayout.LabelField("Actions");
                    var actions = serializedObject.FindProperty("actions");
                    if (GUILayout.Button("New Action"))
                    {
                        Undo.RecordObject(fsm, "New Action");
                        fsm.actions.Insert(0, new FSMAction() { guid = fsm.NextGUID() });
                        serializedObject.Update();
                    }
                    var deleteIndex = -1;
                    for (var i = 0; i < actions.arraySize; i++)
                    {
                        GUILayout.BeginVertical("box");
                        var p = actions.GetArrayElementAtIndex(i);
                        EditorGUILayout.PropertyField(p.FindPropertyRelative("actionName"), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 64));
                        var rect = GUILayoutUtility.GetLastRect();
                        EditorGUILayout.PropertyField(p.FindPropertyRelative("actionList"), true);
                        var button = new Rect(EditorGUIUtility.currentViewWidth - 24, rect.y, 20, 20);
                        GUI.color = Color.red;
                        if (GUI.Button(button, new GUIContent("", "Delete"), EditorStyles.radioButton))
                            deleteIndex = i;
                        GUI.color = Color.white;
                        GUILayout.EndVertical();
                    }
                    if (deleteIndex >= 0)
                        actions.DeleteArrayElementAtIndex(deleteIndex);
                }
                GUILayout.Space(16);
                {
                    EditorGUILayout.LabelField("Predicates");
                    var predicates = serializedObject.FindProperty("predicates");
                    if (GUILayout.Button("New Predicate"))
                    {
                        Undo.RecordObject(fsm, "New Predicate");
                        fsm.predicates.Insert(0, new FSMPredicate() { guid = fsm.NextGUID() });
                        serializedObject.Update();
                    }
                    var deleteIndex = -1;
                    for (var i = 0; i < predicates.arraySize; i++)
                    {
                        GUILayout.BeginVertical("box");
                        var p = predicates.GetArrayElementAtIndex(i);
                        EditorGUILayout.PropertyField(p.FindPropertyRelative("predicateName"), GUILayout.MaxWidth(EditorGUIUtility.currentViewWidth - 64));
                        var rect = GUILayoutUtility.GetLastRect();
                        EditorGUILayout.PropertyField(p.FindPropertyRelative("predicateList"), true);
                        var button = new Rect(EditorGUIUtility.currentViewWidth - 24, rect.y, 20, 20);
                        GUI.color = Color.red;
                        if (GUI.Button(button, new GUIContent("", "Delete"), EditorStyles.radioButton))
                            deleteIndex = i;
                        GUI.color = Color.white;
                        GUILayout.EndVertical();
                    }
                    if (deleteIndex >= 0)
                        predicates.DeleteArrayElementAtIndex(deleteIndex);
                }
                if (cc.changed)
                {
                    serializedObject.ApplyModifiedProperties();
                }
            }
            EditorGUI.indentLevel = indentLevel;
        }
    }
}