using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    [System.Serializable]
    public class FSMState
    {
        public string name;
        [HideInInspector] public int guid;
        public ActionReference onEnterAction = new ActionReference();
        public ActionReference onStayAction = new ActionReference();
        public ActionReference onExitAction = new ActionReference();
        public List<FSMTransition> transitions = new List<FSMTransition>();
        public Rect rect;
        public Rect backgroundRect;
    }
}