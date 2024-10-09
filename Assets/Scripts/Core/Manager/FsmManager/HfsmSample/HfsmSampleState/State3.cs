using UnityEngine;

namespace Yu
{
    public class State3:IFsmComponentState<HfsmSample>
    {
        public void OnEnter(HfsmSample owner, params object[] objs)
        {
            Debug.Log("OnEnter3");
        }

        public void OnUpdate(HfsmSample owner, params object[] objs)
        {
            Debug.Log("OnUpdate3");
        }

        public void OnExit(HfsmSample owner)
        {
            Debug.Log("OnExit3");
        }
    }
}