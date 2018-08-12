using System;
using UnityEngine;

namespace FSM
{
    [System.Serializable]
    public class FSMTransition
    {
        public int targetStateGuid;
        public bool enabled = true;
        public float delay = 0;
        [NonSerialized] public float startTime;
        public PredicateReference predicate = new PredicateReference();
        public ActionReference onEnterAction = new ActionReference();
        public ActionReference onStayAction = new ActionReference();
        public ActionReference onExitAction = new ActionReference();
        [NonSerialized] public FSMState targetState;
        public Rect rect;
    }
}