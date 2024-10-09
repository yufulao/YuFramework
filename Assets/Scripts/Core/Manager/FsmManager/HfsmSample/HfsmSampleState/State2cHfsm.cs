using UnityEngine;

namespace Yu
{
    public class State2cHfsm:HfsmComponent<HfsmSample,string>
    {
        public State2cHfsm(HfsmSample owner) : base(owner)
        {
        }

        public override void OnEnter(HfsmSample owner, params object[] objs)
        {
            Debug.Log("OnEnter2c");
            base.OnEnter(owner, objs);
        }

        public override void OnUpdate(HfsmSample owner, params object[] objs)
        {
            Debug.Log("OnUpdate2c");
            base.OnUpdate(owner, objs);
        }

        public override void OnExit(HfsmSample owner)
        {
            Debug.Log("OnExit2c");
            base.OnExit(owner);
        }
    }
}