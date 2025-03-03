using System;
using UnityEngine;

namespace SFIBehavior
{
    public abstract class Condition : ScriptableObject
    {
		public string description;

        public abstract bool CheckCondition(StateManager state);
       // public abstract int CheckMultiCondition(StateManager state);

        //public abstract Type GetMultiConditionInfo();

    }
}
