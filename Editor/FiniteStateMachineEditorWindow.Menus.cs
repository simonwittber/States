using System;
using System.Collections.Generic;
using DifferentMethods.Extensions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Linq;
using DifferentMethods.Univents;

namespace FSM
{
    public partial class FiniteStateMachineEditorWindow
    {
        void ShowContextMenu(FSMState state, ActionReference actionRef)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Edit"), false, () =>
            {
                var w = GetWindow<ActionEditorWindow>();
                w.ShowUtility();
                w.Load(target, state, actionRef);
            });
            menu.AddItem(new GUIContent("Delete"), false, () =>
            {
                Undo.RegisterCompleteObjectUndo(target, "Remove Action");
                actionRef.guid = 0;
            });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Enabled?"), actionRef.enabled, () =>
            {
                Undo.RegisterCompleteObjectUndo(target, "Toggle Action");
                actionRef.enabled = !actionRef.enabled;
            });
            menu.ShowAsContext();
        }

        void ShowTransitionMenu(FSMState state)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("New Transition"), false, () =>
            {
                Undo.RecordObject(target, "New Transition");
                var p = new FSMPredicate() { guid = target.NextGUID() };
                target.predicates.Insert(0, p);
                var t = new FSMTransition();
                t.predicate.guid = p.guid;
                state.transitions.Add(t);
                OpenPredicateEditor(state.backgroundRect, state, p.guid);
            });
            foreach (var p in target.predicates)
                menu.AddItem(new GUIContent($"Add Transition/{p.predicateName}"), false, AddTransition(state, p));
            menu.ShowAsContext();
        }

        bool ContextMenuRequested(Rect rect)
        {
            var ev = Event.current;
            return ev.type == EventType.MouseUp && rect.Contains(ev.mousePosition) && ev.button == 1;
        }

        void ShowContextMenu(FSMState state, FSMTransition transition)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Edit"), false, () =>
            {
                var w = GetWindow<PredicateEditorWindow>();
                w.ShowUtility();
                w.Load(target, state, transition.predicate.guid);
            });
            menu.AddItem(new GUIContent("Delete"), false, () =>
            {
                Undo.RegisterCompleteObjectUndo(target, "Remove Transition");
                state.transitions.Remove(transition);
            });
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Enabled?"), transition.enabled, () =>
            {
                Undo.RegisterCompleteObjectUndo(target, "Toggle Transition");
                transition.enabled = !transition.enabled;
            });
            menu.AddItem(new GUIContent("On Enter Action/None"), false, () => transition.onEnterAction.guid = 0);
            menu.AddItem(new GUIContent("On Stay Action/None"), false, () => transition.onStayAction.guid = 0);
            menu.AddItem(new GUIContent("On Exit Action/None"), false, () => transition.onExitAction.guid = 0);
            foreach (var a in target.actions)
            {
                menu.AddItem(new GUIContent($"On Enter Action/{a.actionName}"), false, SetActionRef(transition.onEnterAction, a.guid));
                menu.AddItem(new GUIContent($"On Stay Action/{a.actionName}"), false, SetActionRef(transition.onStayAction, a.guid));
                menu.AddItem(new GUIContent($"On Exit Action/{a.actionName}"), false, SetActionRef(transition.onExitAction, a.guid));
            }
            menu.ShowAsContext();
        }

        void ShowContextMenu(FSMState state)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("New Action"), false, () =>
            {
                Undo.RecordObject(target, "New Action");
                var action = new FSMAction() { actionName = "New Action", guid = target.NextGUID() };
                target.actions.Insert(0, action);
                OpenActionEditor(state.backgroundRect, state, action.guid);
            });
            menu.AddItem(new GUIContent("On Enter Action/None"), false, () => state.onEnterAction.guid = 0);
            menu.AddItem(new GUIContent("On Stay Action/None"), false, () => state.onStayAction.guid = 0);
            menu.AddItem(new GUIContent("On Exit Action/None"), false, () => state.onExitAction.guid = 0);
            foreach (var a in target.actions)
            {
                menu.AddItem(new GUIContent($"On Enter Action/{a.actionName}"), false, SetActionRef(state.onEnterAction, a.guid));
                menu.AddItem(new GUIContent($"On Stay Action/{a.actionName}"), false, SetActionRef(state.onStayAction, a.guid));
                menu.AddItem(new GUIContent($"On Exit Action/{a.actionName}"), false, SetActionRef(state.onExitAction, a.guid));
            }
            menu.ShowAsContext();
        }

        GenericMenu.MenuFunction AddTransition(FSMState state, FSMPredicate p)
        {
            return () =>
            {
                Undo.RegisterCompleteObjectUndo(target, "Add Transition");
                var t = new FSMTransition();
                t.predicate.guid = p.guid;
                state.transitions.Add(t);
            };
        }

        GenericMenu.MenuFunction SetActionRef(ActionReference actionRef, int guid)
        {
            return () =>
            {
                Undo.RegisterCompleteObjectUndo(target, "Set Action");
                actionRef.guid = guid;
            };
        }

    }
}