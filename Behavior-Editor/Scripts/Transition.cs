using System.Collections.Generic;

namespace SFIBehavior
{
    [System.Serializable]
    public class Transition
    {
        public int id;
        public Condition condition;
        public State targetState;
        public bool disable;

        //
        // public bool isMultiTransition;
        // public List<State> targetStates = new List<State>();
        //  public MultiCondition MultiCondition;
    }
}