using UnityEngine;

namespace Yu
{
    public class State1:IFsmComponentState<HfsmSample>
    {
        public void OnEnter(HfsmSample owner, params object[] objs)
        {
            Debug.Log("OnEnter1");
        }

        public void OnUpdate(HfsmSample owner, params object[] objs)
        {
            Debug.Log("OnUpdate1");
        }

        public void OnExit(HfsmSample owner)
        {
            Debug.Log("OnExit1");
        }
    }
}