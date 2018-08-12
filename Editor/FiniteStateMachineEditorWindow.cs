using System;
using System.Collections.Generic;
using DifferentMethods.Extensions;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Linq;

namespace FSM
{
    public partial class FiniteStateMachineEditorWindow : ExtendedEditorWindow<FiniteStateMachine>
    {
        [NonSerialized] FSMTransition hotTransition = null;
        [NonSerialized] FSMState hotState = null;
        [NonSerialized] FSMState dragState = null;
        [NonSerialized] FSMState selectedState = null;
        Dictionary<int, FSMState> stateMap = new Dictionary<int, FSMState>();
        Dictionary<int, FSMAction> actionMap = new Dictionary<int, FSMAction>();
        Dictionary<int, FSMPredicate> predicateMap = new Dictionary<int, FSMPredicate>();


        [DidReloadScripts]
        static void Register() => RegisterEditor(typeof(FiniteStateMachine), typeof(FiniteStateMachineEditorWindow));

        public override void Load(FiniteStateMachine asset)
        {
        }

        void Connect(FSMTransition transition, FSMState state)
        {
            if (transition != null) hotTransition = transition;
            if (state != null) hotState = state;
            if (hotTransition != null && hotState != null)
            {
                hotTransition.targetStateGuid = hotState.guid;
                hotTransition = null;
                hotState = null;
            }
        }

        void OpenActionEditor(Rect rect, FSMState state, int guid)
        {
            var w = GetWindow<ActionEditorWindow>();
            w.Load(target, state, guid);
            rect.position += this.position.position - scrollPosition;
            rect.y += 28;
            w.titleContent = new GUIContent("Edit Action");
            w.ShowAsDropDown(rect, new Vector2(rect.width, 256));
        }

        void OpenPredicateEditor(Rect rect, FSMState state, int guid)
        {
            var w = GetWindow<PredicateEditorWindow>();
            w.Load(target, state, guid);
            rect.position += this.position.position - scrollPosition;
            rect.y += 28;
            w.titleContent = new GUIContent("Edit Predicate");
            w.ShowAsDropDown(rect, new Vector2(rect.width, 256));
        }
    }
}