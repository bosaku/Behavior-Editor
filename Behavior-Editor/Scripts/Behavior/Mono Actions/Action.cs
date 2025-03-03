using UnityEngine;

namespace SFIBehavior
{
    public abstract class Action : ScriptableObject
    {
        public abstract void Execute();
    }
}
