using System;
using System.Collections.Generic;
using DifferentMethods.Univents;

namespace FSM
{
    [System.Serializable]
    public class ActionReference
    {
        public int guid;
        public bool enabled = true;
        [NonSerialized] public ActionList actionlist;
        public void Load(Dictionary<int, ActionList> actionMap)
        {
            if (guid != 0)
                actionMap.TryGetValue(guid, out actionlist);
        }

        public void Invoke()
        {
            if (guid != 0 && actionlist != null) actionlist.Invoke();
        }
    }
}