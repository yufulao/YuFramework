using UnityEngine;

namespace Yu
{
    public class State2Hfsm : HfsmComponent<HfsmSample, string>
    {
        public override void OnEnter(HfsmSample owner, params object[] objs)
        {
            GameLog.Info("OnEnter2");
            base.OnEnter(owner, objs);
        }

        public override void OnUpdate(HfsmSample owner, params object[] objs)
        {
            GameLog.Info("OnUpdate2");
            base.OnUpdate(owner, objs);
        }

        public override void OnExit(HfsmSample owner)
        {
            GameLog.Info("OnExit2");
            base.OnExit(owner);
        }

        public State2Hfsm(HfsmSample owner) : base(owner)
        {
        }
    }
}