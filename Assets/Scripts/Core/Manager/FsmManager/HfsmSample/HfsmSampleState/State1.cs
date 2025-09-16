using UnityEngine;

namespace Yu
{
    public class State1:IFsmComponentState<HfsmSample>
    {
        public void OnEnter(HfsmSample owner, params object[] objs)
        {
            GameLog.Info("OnEnter1");
        }

        public void OnUpdate(HfsmSample owner, params object[] objs)
        {
            GameLog.Info("OnUpdate1");
        }

        public void OnExit(HfsmSample owner)
        {
            GameLog.Info("OnExit1");
        }
    }
}