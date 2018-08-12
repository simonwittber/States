using System.Collections.Generic;
using DifferentMethods.Univents;
using UnityEngine;

namespace FSM
{
    public class FiniteStateMachine : MonoBehaviour, ISerializationCallbackReceiver
    {
        public List<FSMAction> actions = new List<FSMAction>();
        public List<FSMPredicate> predicates = new List<FSMPredicate>();

        public List<FSMState> states = new List<FSMState>();
        [HideInInspector] [SerializeField] int nextGUID = 1;

        public FSMState ActiveState { get; private set; }
        public FSMTransition ActiveTransition { get; private set; }

        public void OnAfterDeserialize()
        {
            var actionMap = new Dictionary<int, ActionList>();
            var predicateMap = new Dictionary<int, PredicateList>();
            var stateMap = new Dictionary<int, FSMState>();
            foreach (var a in actions)
                actionMap[a.guid] = a.actionList;
            foreach (var p in predicates)
                predicateMap[p.guid] = p.predicateList;
            foreach (var s in states)
                stateMap[s.guid] = s;
            foreach (var s in states)
            {
                s.onEnterAction.Load(actionMap);
                s.onStayAction.Load(actionMap);
                s.onExitAction.Load(actionMap);
                foreach (var t in s.transitions)
                {
                    t.predicate.Load(predicateMap);
                    t.onEnterAction.Load(actionMap);
                    t.onExitAction.Load(actionMap);
                    t.onExitAction.Load(actionMap);
                    if (t.targetStateGuid != 0) t.targetState = stateMap[t.targetStateGuid];
                }
            }
            ActiveState = states.Count > 0 ? states[0] : null;
        }

        public void OnBeforeSerialize()
        {

        }

        public int NextGUID() => nextGUID++;

        void Start()
        {
            if (ActiveState != null)
                ActiveState.onEnterAction.Invoke();
        }

        void Update()
        {
            if (ActiveState != null)
                Execute();
        }

        public void Execute()
        {
            if (ActiveTransition != null)
            {
                if ((Time.time - ActiveTransition.startTime) <= ActiveTransition.delay)
                {
                    ActiveTransition.onStayAction.Invoke();
                    return;
                }
                else
                {
                    ActiveTransition.onExitAction.Invoke();
                    ActiveState = ActiveTransition.targetState;
                    ActiveTransition = null;
                    ActiveState.onEnterAction.Invoke();
                }
            }
            var transitions = ActiveState.transitions;
            var newState = false;
            for (int i = 0, count = transitions.Count; i < count; i++)
            {
                var t = transitions[i];
                if (t.enabled && t.predicate.Invoke())
                {
                    newState = true;
                    ActiveState.onExitAction.Invoke();
                    ActiveState = null;
                    ActiveTransition = t;
                    ActiveTransition.onEnterAction.Invoke();
                    if (t.delay > 0)
                    {
                        t.startTime = Time.time;
                        ActiveTransition = t;
                        return;
                    }
                    else
                    {
                        ActiveTransition.onStayAction.Invoke();
                        ActiveTransition.onExitAction.Invoke();
                        ActiveState = ActiveTransition.targetState;
                        ActiveTransition = null;
                        ActiveState.onEnterAction.Invoke();
                        return;
                    }
                }
            }
            if (!newState)
                ActiveState.onStayAction.Invoke();
        }

        public void Log(string msg)
        {
            Debug.Log($"<color=orange>{ActiveState.name}: {msg}</color>", this);
        }
    }
}