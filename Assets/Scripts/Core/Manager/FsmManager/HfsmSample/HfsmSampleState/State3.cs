using UnityEngine;

namespace Yu
{
    public class State3:IFsmComponentState<HfsmSample>
    {
        public void OnEnter(HfsmSample owner, params object[] objs)
        {
            GameLog.Info("OnEnter3");
        }

        public void OnUpdate(HfsmSample owner, params object[] objs)
        {
            GameLog.Info("OnUpdate3");
        }

        public void OnExit(HfsmSample owner)
        {
            GameLog.Info("OnExit3");
        }
    }
}