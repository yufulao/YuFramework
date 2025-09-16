using UnityEngine;

namespace Yu
{
    public class State2c2:IFsmComponentState<HfsmSample>
    {
        public void OnEnter(HfsmSample owner, params object[] objs)
        {
            GameLog.Info("OnEnter2c2");
        }

        public void OnUpdate(HfsmSample owner, params object[] objs)
        {
            GameLog.Info("OnUpdate2c2");
        }

        public void OnExit(HfsmSample owner)
        {
            GameLog.Info("OnExit2c2");
        }
    }
}