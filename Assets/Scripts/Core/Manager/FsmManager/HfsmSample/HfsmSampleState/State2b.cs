using UnityEngine;

namespace Yu
{
    public class State2b:IFsmComponentState<HfsmSample>
    {
        public void OnEnter(HfsmSample owner, params object[] objs)
        {
            GameLog.Info("OnEnter2b");
        }

        public void OnUpdate(HfsmSample owner, params object[] objs)
        {
            GameLog.Info("OnUpdate2b");
        }

        public void OnExit(HfsmSample owner)
        {
            GameLog.Info("OnExit2b");
        }
    }
}