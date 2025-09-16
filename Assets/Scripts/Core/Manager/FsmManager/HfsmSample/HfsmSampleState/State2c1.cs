using UnityEngine;

namespace Yu
{
    public class State2c1:IFsmComponentState<HfsmSample>
    {
        public void OnEnter(HfsmSample owner, params object[] objs)
        {
            GameLog.Info("OnEnter2c1");
        }

        public void OnUpdate(HfsmSample owner, params object[] objs)
        {
            GameLog.Info("OnUpdate2c1");
        }

        public void OnExit(HfsmSample owner)
        {
            GameLog.Info("OnExit2c1");
        }
    }
}