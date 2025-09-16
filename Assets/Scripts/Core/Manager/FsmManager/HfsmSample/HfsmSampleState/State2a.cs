using UnityEngine;

namespace Yu
{
    public class State2a:IFsmComponentState<HfsmSample>
    {
        public void OnEnter(HfsmSample owner, params object[] objs)
        {
            GameLog.Info("OnEnter2a");
        }

        public void OnUpdate(HfsmSample owner, params object[] objs)
        {
            GameLog.Info("OnUpdate2a");
        }

        public void OnExit(HfsmSample owner)
        {
            GameLog.Info("OnExit2a");
        }
    }
}