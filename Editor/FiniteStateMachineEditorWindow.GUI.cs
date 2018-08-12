using System;
using System.Collections.Generic;
using DifferentMethods.Extensions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Linq;

namespace FSM
{
    public partial class FiniteStateMachineEditorWindow
    {
        protected override void OnBeforeGUI(FiniteStateMachine asset)
        {
            EditorApplication.delayCall += Repaint;

            for (var i = 0; i < target.states.Count; i++)
            {
                var state = target.states[i];
                if (!actionMap.ContainsKey(state.onEnterAction.guid))
                    state.onEnterAction.guid = 0;
                if (!actionMap.ContainsKey(state.onStayAction.guid))
                    state.onStayAction.guid = 0;
                if (!actionMap.ContainsKey(state.onExitAction.guid))
                    state.onExitAction.guid = 0;
                var cleanup = new List<FSMTransition>();
                for (var j = 0; j < state.transitions.Count; j++)
                {
                    var t = state.transitions[j];
                    if (!stateMap.ContainsKey(t.targetStateGuid))
                        t.targetStateGuid = 0;
                    if (!predicateMap.ContainsKey(t.predicate.guid))
                        cleanup.Add(t);
                    if (!actionMap.ContainsKey(t.onEnterAction.guid))
                        t.onEnterAction.guid = 0;
                    if (!actionMap.ContainsKey(t.onStayAction.guid))
                        t.onStayAction.guid = 0;
                    if (!actionMap.ContainsKey(t.onExitAction.guid))
                        t.onExitAction.guid = 0;
                }
                foreach (var t in cleanup)
                    state.transitions.Remove(t);
            }
            actionMap.Clear();
            for (var i = 0; i < target.actions.Count; i++)
                actionMap[target.actions[i].guid] = target.actions[i];
            predicateMap.Clear();
            for (var i = 0; i < target.predicates.Count; i++)
                predicateMap[target.predicates[i].guid] = target.predicates[i];
            stateMap.Clear();
            for (var i = 0; i < target.states.Count; i++)
                stateMap[target.states[i].guid] = target.states[i];
        }

        protected override Rect OnScrollGUI(Rect visibleRect)
        {
            using (var cc = new EditorGUI.ChangeCheckScope())
            {
                var row = new Rect(0, 16, position.width - 16, 16);
                dropZones.Clear();
                for (var i = 0; i < target.states.Count; i++)
                {
                    var color = i % 2 == 0 ? new Color(0.3f, 0.3f, 0.3f, 1f) : new Color(0.4f, 0.4f, 0.4f, 1f);
                    var bg = target.states[i].backgroundRect;
                    if (selectedState == target.states[i])
                        EditorGUI.DrawRect(bg.Grow(3), Color.white);
                    EditorGUI.DrawRect(bg.Grow(2), color);
                    dropZones.Add(new DropZone(bg, i, lastZone: false));
                    if (i == target.states.Count - 1)
                        dropZones.Add(new DropZone(bg, i + 1, lastZone: true));
                }

                for (var i = 0; i < target.states.Count; i++)
                {
                    var color = i % 2 == 0 ? new Color(0.5f, 0.5f, 0.5f, 1) : new Color(0.75f, 0.75f, 0.75f, 1);
                    row = DrawState(row, target.states[i], color);
                }
                for (var i = 0; i < target.states.Count; i++)
                {
                    for (var j = 0; j < target.states[i].transitions.Count; j++)
                    {
                        var t = target.states[i].transitions[j];
                        if (t.targetStateGuid > 0)
                        {
                            var start = t.rect.center;
                            var end = stateMap[t.targetStateGuid].rect.center;
                            var color = Color.yellow * (t.enabled ? 0.75f : 0.25f);
                            if (selectedState == target.states[i])
                                color = Color.yellow * 1.25f;
                            Handles.DrawBezier(start, end, start + Vector2.right * 200, end + Vector2.left * 200, Color.white * 0.35f, null, 7);
                            Handles.DrawBezier(start, end, start + Vector2.right * 200, end + Vector2.left * 200, color, null, 3);
                        }
                    }
                }
                if (hotState != null)
                {
                    var start = Event.current.mousePosition;
                    var end = hotState.rect.center;
                    Handles.DrawBezier(start, end, start + Vector2.right * 200, end + Vector2.left * 200, Color.white * 0.5f, null, 3);
                    Repaint();
                }
                if (hotTransition != null)
                {
                    var start = hotTransition.rect.center;
                    var end = Event.current.mousePosition;
                    Handles.DrawBezier(start, end, start + Vector2.right * 200, end + Vector2.left * 200, Color.white * 0.5f, null, 3);
                    Repaint();
                }
                row.y += 16;
                var rect = row;
                rect.x = rect.width = (rect.width / 4);
                if (GUI.Button(rect, "New State"))
                {
                    target.states.Add(new FSMState() { guid = target.NextGUID() });
                }
                if (cc.changed)
                {
                    EditorUtility.SetDirty(target);
                }
                HandleDragAndDropEvents();
                HandleKeyEvents();
                HandleCommandEvents();
                return new Rect(0, 0, row.width, row.yMax);
            }
        }

        Rect DrawState(Rect row, FSMState state, Color color)
        {
            row.y += 16;
            var top = row.min.y;
            var rectA = row;
            rectA.width = (row.width - 32) / 4;
            var rectB = rectA;
            rectB.x += rectA.width + 8;
            var rectC = rectB;
            rectC.x += rectB.width + 8;
            {
                var button = new Rect(rectB.xMin, rectB.y, 16, rectB.height);
                GUI.color = Color.grey;
                if (GUI.Button(button, GUIContent.none, EditorStyles.radioButton))
                    Connect(null, state);
                GUI.color = Color.white;
                state.rect = button;
                button.x += button.width;
                if (GUI.Button(button, GUIContent.none, EditorStyles.radioButton))
                    ShowContextMenu(state);
                var label = rectB;
                label.x = button.xMax;
                label.xMax = rectB.xMax;
                state.name = GUI.TextField(label, state.name);
            }
            if (state.onEnterAction.guid != 0)
            {
                rectB.y = rectB.yMax + 4;
                ActionLabel(rectB, $"On Enter: {actionMap[state.onEnterAction.guid].actionName}");
                if (ContextMenuRequested(rectB))
                    ShowContextMenu(state, state.onEnterAction);
                if (DoubleClicked(rectB))
                    OpenActionEditor(rectB, state, state.onEnterAction.guid);

            }
            if (state.onStayAction.guid != 0)
            {
                rectB.y = rectB.yMax + 4;
                ActionLabel(rectB, $"On Stay: {actionMap[state.onStayAction.guid].actionName}");
                if (ContextMenuRequested(rectB))
                    ShowContextMenu(state, state.onStayAction);
                if (DoubleClicked(rectB))
                    OpenActionEditor(rectB, state, state.onStayAction.guid);
            }
            if (state.onExitAction.guid != 0)
            {
                rectB.y = rectB.yMax + 4;
                ActionLabel(rectB, $"On Exit: {actionMap[state.onExitAction.guid].actionName}");
                if (ContextMenuRequested(rectB))
                    ShowContextMenu(state, state.onExitAction);
                if (DoubleClicked(rectB))
                    OpenActionEditor(rectB, state, state.onExitAction.guid);
            }

            var rect = rectC;
            rect.width = 16;
            if (GUI.Button(rect, GUIContent.none, EditorStyles.radioButton))
                ShowTransitionMenu(state);
            rect.xMin += 16;
            rect.xMax = rectC.xMax;
            EditorGUI.LabelField(rect, "Transitions");
            rectC.y = rect.yMax + 4;
            for (var i = 0; i < state.transitions.Count; i++)
            {
                rectC = DrawTransition(rectC, state, state.transitions[i], color);
                var contextRect = new Rect(rectC.x, rectC.y, rectC.width, 16);
                if (ContextMenuRequested(contextRect))
                    ShowContextMenu(state, state.transitions[i]);
                if (DoubleClicked(contextRect))
                    OpenPredicateEditor(contextRect, state, state.transitions[i].predicate.guid);
                if (i != state.transitions.Count - 1) rectC.y = rectC.yMax + 4;
            }
            var buttonRect = rectC;
            buttonRect.width -= 16;


            var bottom = Mathf.Max(rectB.max.y, rectC.max.y);
            var bgr = Rect.MinMaxRect(rectB.min.x, top, rectC.max.x, bottom);
            state.backgroundRect = bgr;
            if (Event.current.type == EventType.MouseUp && bgr.Contains(Event.current.mousePosition))
            {
                selectedState = state;
                Repaint();
            }
            row.y = Mathf.Max(rectA.max.y, rectB.max.y, rectC.max.y);
            return row;
        }

        Rect DrawTransition(Rect rect, FSMState state, FSMTransition transition, Color color)
        {
            var label = rect;
            label.width -= 16;
            var connector = new Rect(label.xMax, label.y, 16, label.height);
            if (transition.predicate.guid != 0)
            {
                ActionLabel(rect, $"{predicateMap[transition.predicate.guid].predicateName}", enabled: transition.enabled);
            }
            transition.rect = connector;
            GUI.color = Color.grey;
            if (GUI.Button(connector, GUIContent.none, EditorStyles.radioButton))
                Connect(transition, null);
            GUI.color = Color.white;
            var firstRect = rect;
            rect.xMin += 16;
            rect.xMax -= 16;
            if (transition.onEnterAction.guid != 0)
            {
                rect.y = rect.yMax + 2;
                ActionLabel(rect, $"On Enter: {actionMap[transition.onEnterAction.guid].actionName}");
                if (DoubleClicked(rect))
                    OpenActionEditor(rect, state, transition.onEnterAction.guid);
            }
            if (transition.onStayAction.guid != 0)
            {
                rect.y = rect.yMax + 2;
                ActionLabel(rect, $"On Stay: {actionMap[transition.onStayAction.guid].actionName}");
                if (DoubleClicked(rect))
                    OpenActionEditor(rect, state, transition.onStayAction.guid);
            }
            if (transition.onExitAction.guid != 0)
            {
                rect.y = rect.yMax + 2;
                ActionLabel(rect, $"On Exit: {actionMap[transition.onExitAction.guid].actionName}");
                if (DoubleClicked(rect))
                    OpenActionEditor(rect, state, transition.onExitAction.guid);
            }
            return Rect.MinMaxRect(firstRect.min.x, firstRect.min.y, firstRect.max.x, rect.max.y);
        }

        void ActionLabel(Rect rect, string text, bool enabled = true)
        {
            var box = new GUIStyle(GUI.skin.box);
            box.alignment = TextAnchor.MiddleLeft;
            if (!enabled)
            {
                box.fontStyle = FontStyle.Italic;
                box.normal.textColor *= 0.25f;
            }
            if (rect.Contains(Event.current.mousePosition))
            {
                box.normal.textColor = Color.white;
                EditorGUI.DrawRect(rect.Grow(1), Color.white * 0.5f);
            }
            GUI.Label(rect, text, box);
        }

        bool DoubleClicked(Rect rect)
        {
            var ev = Event.current;
            return (ev.button == 0 && ev.clickCount == 2 && rect.Contains(ev.mousePosition));
        }
    }
}