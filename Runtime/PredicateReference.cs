using System;
using System.Collections.Generic;
using DifferentMethods.Univents;

namespace FSM
{
    [System.Serializable]
    public class PredicateReference
    {
        public int guid;
        [NonSerialized] public PredicateList predicateList;
        public void Load(Dictionary<int, PredicateList> predicateMap)
        {
            if (guid != 0)
                predicateMap.TryGetValue(guid, out predicateList);
        }
        public bool Invoke()
        {
            if (guid != 0 && predicateList != null)
            {
                predicateList.Invoke();
                return predicateList.Result;
            }
            return false;
        }
    }
}